using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class PlayerController : MonoBehaviour
    {
        private const float RAYCAST_MAX_DISTANCE = 500000f;

        [SerializeField]
        private LayerMask _cubitsLayer;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _moveSpeed = 2f;

        [SerializeField]
        private CubitData _selectedCubit;

        [SerializeField]
        private CubitPlacementSystem _placement;

        private PlayerInventory _playerInventory;
        private bool _destroyingCube;
        private Cube _targetCube;

        private void Awake()
        {
            _playerInventory = new PlayerInventory();
        }

        private void Update()
        {
            ProcessInput();
            HandleCubeDestructionProgress();
        }

        private void ProcessInput()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var raycastHit = RaycastCubits();

                if (raycastHit.HasValue)
                {
                    var hit = raycastHit.Value;
                    var targetCubit = hit.transform.GetComponentInParent<Cubit>();
                    _placement.PlaceCubit(targetCubit, hit.normal, _selectedCubit);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                var raycastHit = RaycastCubits();

                if (raycastHit.HasValue)
                {
                    var hit = raycastHit.Value;
                    var targetCubit = hit.transform.GetComponentInParent<Cubit>();
                    StartDestroyingCube(targetCubit.Cube);
                }
            }
        }

        private void HandleCubeDestructionProgress()
        {
            if (!_destroyingCube)
            {
                return;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                StopDestroyingCube();
                return;
            }

            var raycastHit = RaycastCubits();

            if (!raycastHit.HasValue)
            {
                StopDestroyingCube();
            }
        }

        private void StopDestroyingCube()
        {
            _destroyingCube = false;

            _targetCube.OnDestroy -= HandleCubeDestroyed;
            _targetCube.StopDestruction();

            _targetCube = null;
        }

        private void HandleCubeDestroyed(Cube cube)
        {
            var cubitsReward = cube.CubitsReward;
            
            foreach (var cubitData in cubitsReward.Keys)
            {
                _playerInventory.Add(cubitData, cubitsReward[cubitData]);
            }

            _destroyingCube = false;
            
            _targetCube.OnDestroy -= HandleCubeDestroyed;
            _targetCube = null;
        }

        private RaycastHit? RaycastCubits()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            return Physics.Raycast(ray, out var hit, RAYCAST_MAX_DISTANCE, _cubitsLayer) ? hit : null;
        }

        private void StartDestroyingCube(Cube cube)
        {
            _destroyingCube = true;
            
            _targetCube = cube;

            _targetCube.OnDestroy += HandleCubeDestroyed;
            _targetCube.StartDestruction();
        }

        private void OnDrawGizmos()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if (Physics.Raycast(ray, out var hit, RAYCAST_MAX_DISTANCE, _cubitsLayer))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, .2f);

                var normalRayLength = 2f;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal * normalRayLength);
            }
        }
    }
}