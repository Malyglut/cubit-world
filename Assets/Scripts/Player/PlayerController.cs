using Cinemachine;
using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.UserInterface;
using Malyglut.CubitWorld.Utilties;
using Malyglut.CubitWorld.World;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld.Player
{
    public class PlayerController : MonoBehaviour
    {
        [FormerlySerializedAs("_dynamicCamera"),SerializeField]
        private CinemachineVirtualCamera _virtualCamera;
        
        [SerializeField]
        private LayerMask _cubitsLayer;

        [SerializeField]
        private LayerMask _planeLayer;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _moveSpeed = 2f;

        [SerializeField]
        private PlacementSystem _placement;

        [SerializeField]
        private PlayerInventory _inventory;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private CubeGrid _grid;

        [SerializeField, Range(1f, 25f)]
        private float _interactionRange = 5f;

        [SerializeField]
        private Reticle _reticle;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _hotbarSelection;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _inventoryOpened;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _inventoryClosed;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _pauseScreenOpened;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _pauseScreenClosed;

        private bool _destroyingCube;
        private Cube _targetCube;
        private IPlaceableData _selectedPlaceableData;
        private bool _isSuspended;
        private float _cameraVerticalSpeed;
        private float _cameraHorizontalSpeed;
        private CinemachinePOV _cameraPov;

        private void Awake()
        {
            _cameraPov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
            _cameraVerticalSpeed = _cameraPov.m_VerticalAxis.m_MaxSpeed; 
            _cameraHorizontalSpeed = _cameraPov.m_HorizontalAxis.m_MaxSpeed; 
            
            Unsuspend();

            _hotbarSelection.Subscribe(UpdateSelectedPlaceable);
            _inventoryOpened.Subscribe(Suspend);
            _inventoryClosed.Subscribe(Unsuspend);
            _pauseScreenOpened.Subscribe(Suspend);
            _pauseScreenClosed.Subscribe(Unsuspend);
        }

        private void OnDestroy()
        {
            _hotbarSelection.Unsubscribe(UpdateSelectedPlaceable);
            _inventoryOpened.Unsubscribe(Suspend);
            _inventoryClosed.Unsubscribe(Unsuspend);
            _pauseScreenOpened.Unsubscribe(Suspend);
            _pauseScreenClosed.Unsubscribe(Unsuspend);
        }

        private void Suspend()
        {
            _isSuspended = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _placement.HidePreview();
            
            //best i could come up with to disable camera rotation when pause screen is visible, its too late
            _cameraPov.m_VerticalAxis.m_MaxSpeed = 0f;
            _cameraPov.m_HorizontalAxis.m_MaxSpeed = 0f;

            if (_targetCube != null)
            {
                StopDestroyingCube();
            }
        }

        private void Unsuspend()
        {
            _isSuspended = false;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _cameraPov.m_VerticalAxis.m_MaxSpeed = _cameraVerticalSpeed;
            _cameraPov.m_HorizontalAxis.m_MaxSpeed = _cameraHorizontalSpeed;
        }

        private void UpdateSelectedPlaceable(object placeableDataObject)
        {
            _selectedPlaceableData = (IPlaceableData)placeableDataObject;
            _placement.UpdatePreviewVisual(_selectedPlaceableData);
        }

        private void Update()
        {
            if (_isSuspended)
            {
                return;
            }

            UpdateReticle();
            UpdatePreview();
            ProcessInput();
            HandleCubeDestructionProgress();
        }

        private void UpdateReticle()
        {
            _reticle.SetInactive();
            
            var raycastHit = RaycastCubits();

            if (raycastHit.HasValue)
            {
                _reticle.SetActive();
            }
            else
            {
                raycastHit = RaycastPlane();

                if (raycastHit.HasValue)
                {
                    _reticle.SetActive();
                }
            }
        }

        private void UpdatePreview()
        {
            if (_selectedPlaceableData != null)
            {
                var raycastHit = RaycastCubits();

                if (raycastHit.HasValue)
                {
                    var hit = raycastHit.Value;
                    var targetCubit = hit.transform.GetComponentInParent<Cubit>();
                    _placement.UpdatePreviewPosition(_selectedPlaceableData, transform.position, targetCubit,
                        hit.normal);
                }
                else
                {
                    raycastHit = RaycastPlane();

                    if (raycastHit.HasValue)
                    {
                        var hit = raycastHit.Value;

                        _placement.UpdatePreviewPosition(_selectedPlaceableData, transform.position, hit.point, hit.normal);
                    }
                    else
                    {
                        _placement.HidePreview();
                    }
                }
            }
        }

        private void ProcessInput()
        {
            ProcessMovementInput();
            ProcessPlacementInput();
            ProcessDestructionInput();
        }

        private void ProcessMovementInput()
        {
            var forwardSpeed = Input.GetAxis("Vertical") * _moveSpeed;
            var sideSpeed = Input.GetAxis("Horizontal") * _moveSpeed;
            var upSpeed = (Input.GetKey(KeyCode.Space) ? 1f : 0f) * _moveSpeed;

            var cameraTransform = _camera.transform;

            var moveVector = cameraTransform.forward * forwardSpeed + cameraTransform.right * sideSpeed +
                             Vector3.up * upSpeed;

            transform.position += moveVector * Time.deltaTime;
        }

        private void ProcessDestructionInput()
        {
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

        private void ProcessPlacementInput()
        {
            if (_selectedPlaceableData != null && _placement.HasValidPlacementPosition && Input.GetMouseButtonDown(1))
            {
                if (_selectedPlaceableData is CubitData cubitData)
                {
                    PlaceCubit(cubitData);
                }

                if (_selectedPlaceableData is ShapeData shapeData)
                {
                    PlaceShape(shapeData);
                }
            }
        }

        private void PlaceShape(ShapeData shapeData)
        {
            if (_inventory.HasShape(shapeData))
            {
                _placement.PlaceShape(shapeData);
                _inventory.RemoveShape(shapeData);
            }
        }

        private void PlaceCubit(CubitData cubitData)
        {
            if (_inventory.MarbleCount(cubitData) > 0)
            {
                _placement.PlaceCubit(cubitData);
                _inventory.SubtractMarbles(cubitData, 1);
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

            if (raycastHit.HasValue)
            {
                var hit = raycastHit.Value;
                var cube = hit.transform.GetComponentInParent<Cubit>().Cube;
                
                if(cube!=null && cube != _targetCube)
                {
                    StopDestroyingCube();
                }
            }
            else
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
            
            _placement.EnablePreview();
        }

        private void HandleCubeDestroyed(Cube cube)
        {
            var cubitsReward = cube.CubitsReward;

            foreach (var cubitData in cubitsReward.Keys)
            {
                _inventory.AddMarbles(cubitData, cubitsReward[cubitData]);
            }

            _destroyingCube = false;

            _targetCube.OnDestroy -= HandleCubeDestroyed;
            _targetCube = null;
            
            _placement.EnablePreview();
        }

        private RaycastHit? RaycastCubits()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            return Physics.Raycast(ray, out var hit, _interactionRange, _cubitsLayer) ? hit : null;
        }

        private RaycastHit? RaycastPlane()
        {
            var ray = new Ray(_camera.transform.position, _camera.transform.forward);
            return Physics.Raycast(ray, out var hit, _interactionRange, _planeLayer) ? hit : null;
        }

        private void StartDestroyingCube(Cube cube)
        {
            _destroyingCube = true;

            _targetCube = cube;

            _targetCube.OnDestroy += HandleCubeDestroyed;
            _targetCube.StartDestruction();
            
            _placement.DisablePreview();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _interactionRange);

            var raycastHit = RaycastCubits();

            if (raycastHit.HasValue)
            {
                var hit = raycastHit.Value;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, .2f);

                var normalRayLength = 2f;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(hit.point, hit.point + hit.normal * normalRayLength);
                DrawCubeAndCubit(hit.point);
            }
            else
            {
                raycastHit = RaycastPlane();

                if (raycastHit.HasValue)
                {
                    var hit = raycastHit.Value;
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(hit.point, .2f);

                    Gizmos.color = Color.yellow;
                    DrawCubeAndCubit(hit.point);
                }
            }
        }

        private void DrawCubeAndCubit(Vector3 rayHitPosition)
        {
            var cubePosition = _grid.WorldPositionToCubePosition(rayHitPosition);
            var cubitPosition = _grid.WorldPositionToCubitPosition(rayHitPosition);

            Gizmos.DrawSphere(cubePosition, .2f);
            Gizmos.DrawWireCube(cubePosition, Vector3.one * _gameSettings.CubeSize);
            Gizmos.DrawWireCube(cubitPosition, Vector3.one * _gameSettings.CubitSize);
        }
    }
}