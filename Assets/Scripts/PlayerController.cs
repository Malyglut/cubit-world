using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _cubitsLayer;
        
        [SerializeField]
        private LayerMask _planeLayer;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _moveSpeed = 2f;

        [FormerlySerializedAs("_selectedCubit"),SerializeField]
        private CubitData _selectedCubitData;

        [SerializeField]
        private CubitPlacementSystem _placement;

        [SerializeField]
        private PlayerInventory _playerInventory;
        
        [SerializeField]
        private GameEvent _hotbarSelection;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private CubeGrid _grid;

        [SerializeField, Range(1f, 25f)]
        private float _interactionRange = 5f;

        private bool _destroyingCube;
        private Cube _targetCube;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _hotbarSelection.Subscribe(HandleMarbleSelected);
        }

        private void HandleMarbleSelected(object cubitDataObject)
        {
            _selectedCubitData = (CubitData)cubitDataObject;
            _placement.UpdatePreview(_selectedCubitData);

            if (_selectedCubitData == null)
            {
                _placement.HidePreview();
            }
        }

        private void Update()
        {
            UpdatePreview();
            ProcessInput();
            HandleCubeDestructionProgress();
        }

        private void UpdatePreview()
        {
            if (_selectedCubitData != null)
            {
                var raycastHit = RaycastCubits();

                if (raycastHit.HasValue)
                {
                    var hit = raycastHit.Value;
                    var targetCubit = hit.transform.GetComponentInParent<Cubit>();
                    _placement.UpdatePreview(targetCubit, hit.normal);
                    // _placement.PlaceCubit(targetCubit, hit.normal, _selectedCubitData);
                    // _playerInventory.SubtractMarbles(_selectedCubitData, 1);
                }
                else
                {
                    raycastHit = RaycastPlane();

                    if (raycastHit.HasValue)
                    {
                        var hit = raycastHit.Value;

                        _placement.UpdatePreview(hit.point);
                        
                        // _placement.PlaceCubit(hit.point, _selectedCubitData);
                        // _playerInventory.SubtractMarbles(_selectedCubitData, 1);
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
            if (_selectedCubitData != null && Input.GetMouseButtonDown(1))
            {
                if (_placement.HasValidPlacementPosition)
                {
                    _placement.PlaceCubit(_selectedCubitData);
                    _playerInventory.SubtractMarbles(_selectedCubitData, 1);
                }
                
                // var raycastHit = RaycastCubits();
                //
                // if (raycastHit.HasValue)
                // {
                //     var hit = raycastHit.Value;
                //     var targetCubit = hit.transform.GetComponentInParent<Cubit>();
                //     _placement.PlaceCubit(targetCubit, hit.normal, _selectedCubitData);
                //     _playerInventory.SubtractMarbles(_selectedCubitData, 1);
                // }
                // else
                // {
                //     raycastHit = RaycastPlane();
                //
                //     if (raycastHit.HasValue)
                //     {
                //         var hit = raycastHit.Value;
                //
                //         _placement.PlaceCubit(hit.point, _selectedCubitData);
                //         _playerInventory.SubtractMarbles(_selectedCubitData, 1);
                //     }
                // }
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
                _playerInventory.AddMarbles(cubitData, cubitsReward[cubitData]);
            }

            _destroyingCube = false;
            
            _targetCube.OnDestroy -= HandleCubeDestroyed;
            _targetCube = null;
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
            }
            else
            {
                raycastHit = RaycastPlane();

                if (raycastHit.HasValue)
                {
                    var hit = raycastHit.Value;
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(hit.point, .2f);

                    var cubePosition = _grid.WorldPositionToCubePosition(hit.point);
                    var cubitPosition = _grid.WorldPositionToCubitPosition(hit.point);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(cubePosition, .2f);
                    Gizmos.DrawWireCube(cubePosition, Vector3.one *_gameSettings.CubeSize);
                    Gizmos.DrawWireCube(cubitPosition, Vector3.one *_gameSettings.CubitSize);
                    
                }
            }
        }
    }
}