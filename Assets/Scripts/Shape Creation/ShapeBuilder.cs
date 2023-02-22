using System.Collections.Generic;
using System.Linq;
using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.Player;
using Malyglut.CubitWorld.Utilties;
using Malyglut.CubitWorld.World;
using UnityEngine;
using UnityEngine.UI;

namespace Malyglut.CubitWorld.ShapeCreation
{
    public class ShapeBuilder : MonoBehaviour
    {
        private const float SHAPE_EXTENTS_ERROR_TOLERANCE = .000025f;
        private const int MIN_CUBITS_REQUIRED = 2;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private LayerMask _shapePreviewLayer;

        [SerializeField]
        private LayerMask _cubitsLayer;

        [SerializeField]
        private Cubit _cubitPrefab;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private Transform _cubitsParent;

        [SerializeField]
        private Transform _shapeContainer;

        [SerializeField]
        private float _rotationSpeed = 25f;

        [SerializeField]
        private PlayerInventory _playerInventory;

        [SerializeField]
        private CubitPreview _placementPreview;

        [SerializeField]
        private ShapeCreator _shapeCreator;
        
        
        [SerializeField]
        private Button _createShapeButton;
        
        [SerializeField]
        private Button _resetButton;

        private float _cubitSize;
        private Quaternion _initialRotation;
        private CubitData _selectedCubit;
        private bool _rotationInProgress;
        private float _maxShapeExtents;
        private bool HasValidPlacementPosition => _placementPreview.gameObject.activeSelf;

        private Dictionary<Vector3Int, Cubit> _shapeBlueprint = new();

        private void OnEnable()
        {
            transform.rotation = _initialRotation;
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            _createShapeButton.interactable = _shapeBlueprint.Count >= MIN_CUBITS_REQUIRED;
            _resetButton.interactable = _shapeBlueprint.Count > 0;
        }

        private void Awake()
        {
            _cubitSize = 1f / _gameSettings.CubitsPerCubeAxis;
            _initialRotation = _shapeContainer.rotation;
            CalculateMaxShapeExtents();
            
            _createShapeButton.onClick.AddListener(BuildShape);
            _resetButton.onClick.AddListener(ResetShape);

            _placementPreview.gameObject.SetActive(false);

            _shapeCreator.OnShapeCreated += AddShapeToInventory;
        }

        private void ResetShape()
        {
            foreach (var (_,cubit) in _shapeBlueprint)
            {
                _playerInventory.AddMarbles(cubit.Data, 1);
            }
            
            ResetState();
            UpdateButtons();
        }

        private void CalculateMaxShapeExtents()
        {
            var isEven = _gameSettings.CubitsPerCubeAxis % 2 == 0;

            if (isEven)
            {
                var startingPosition = 1f / _gameSettings.CubitsPerCubeAxis;
                var halfCubitSize = _gameSettings.CubitCellSize * .5f;
                var cubitLayers = _gameSettings.CubitsPerCubeAxis / 2;

                _maxShapeExtents = startingPosition + halfCubitSize * (cubitLayers - 1);
            }
            else
            {
                var cubitSize = _gameSettings.CubitCellSize;
                var cubitLayersFromCenter = (int)(_gameSettings.CubitsPerCubeAxis / 2f);
                _maxShapeExtents = cubitSize * cubitLayersFromCenter;
            }

            _maxShapeExtents += SHAPE_EXTENTS_ERROR_TOLERANCE;
        }

        private void Update()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            UpdatePreview();
            HandleRotation();
            ProcessInput();
        }

        private void ProcessInput()
        {
            if (_rotationInProgress)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, 10000f, _cubitsLayer))
                {
                    var cubit = hit.transform.GetComponentInParent<Cubit>();

                    if (cubit == null)
                    {
                        return;
                    }

                    DestroyCubit(cubit);
                }
            }

            if (_selectedCubit == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1) && HasValidPlacementPosition)
            {
                PlaceCubit();
            }
        }

        private void PlaceCubit()
        {
            var cubitPosition = _placementPreview.transform.position;
            var gridIdx = Utils.GridIndex(_cubitsParent.InverseTransformPoint(cubitPosition),
                _gameSettings.CubitCellSize);

            var cubit = Instantiate(_cubitPrefab, cubitPosition, _cubitsParent.rotation,
                _cubitsParent);
            cubit.transform.localScale = Vector3.one * _cubitSize;

            cubit.Initialize(_selectedCubit, null);
            cubit.PlayPlacementAnimation();

            _shapeBlueprint.Add(gridIdx, cubit);

            _playerInventory.SubtractMarbles(_selectedCubit, 1);
            
            UpdateButtons();
        }

        private void DestroyCubit(Cubit cubit)
        {
            var gridIdx = _shapeBlueprint.First(data => data.Value == cubit).Key;

            Destroy(cubit.gameObject);
            _playerInventory.AddMarbles(cubit.Data, 1);
            _shapeBlueprint.Remove(gridIdx);
            
            UpdateButtons();
        }

        private void UpdatePreview()
        {
            if (_selectedCubit != null && !_rotationInProgress)
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, 10000f, _shapePreviewLayer | _cubitsLayer))
                {
                    var cubitPosition = CubitPosition(hit.point + hit.normal * (_cubitSize * .5f));

                    if (IsInsideShape(cubitPosition))
                    {
                        _placementPreview.gameObject.SetActive(true);

                        _placementPreview.transform.position = cubitPosition;
                        _placementPreview.transform.rotation = _cubitsParent.rotation;
                    }
                    else
                    {
                        _placementPreview.gameObject.SetActive(false);
                    }
                }
                else
                {
                    _placementPreview.gameObject.SetActive(false);
                }
            }
        }

        private bool IsInsideShape(Vector3 cubitPosition)
        {
            var localCubitPosition = _cubitsParent.InverseTransformPoint(cubitPosition);

            return Mathf.Abs(localCubitPosition.x) <= _maxShapeExtents
                   && Mathf.Abs(localCubitPosition.y) <= _maxShapeExtents
                   && Mathf.Abs(localCubitPosition.z) <= _maxShapeExtents;
        }

        private void HandleRotation()
        {
            if (Input.GetMouseButtonDown(2))
            {
                _rotationInProgress = true;
            }

            if (Input.GetMouseButton(2))
            {
                //inverted
                var mouseX = Input.GetAxisRaw("Mouse X") * -1f;

                mouseX = mouseX != 0f ? Mathf.Sign(mouseX) : 0f;

                var directionVector = new Vector3(0f, mouseX, 0f);

                if (directionVector.magnitude > 0f)
                {
                    directionVector *= Time.deltaTime * _rotationSpeed;
                    _shapeContainer.Rotate(directionVector);
                }
            }

            if (Input.GetMouseButtonUp(2))
            {
                _rotationInProgress = false;
            }
        }

        private void OnDrawGizmos()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 10000f, _shapePreviewLayer | _cubitsLayer))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, .05f);
                Vector3 point = CubitPosition(hit.point + hit.normal * (_cubitSize * .5f));

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(point, .05f);
                Gizmos.DrawWireCube(point, Vector3.one * _cubitSize);
                Gizmos.DrawLine(hit.point, hit.point + hit.normal * _cubitSize * .5f);
            }
        }

        private Vector3 CubitPosition(Vector3 position)
        {
            var localPosition = _cubitsParent.InverseTransformPoint(position);

            var x = Mathf.Round(localPosition.x / _cubitSize) * _cubitSize;
            var y = Mathf.Round(localPosition.y / _cubitSize) * _cubitSize;
            var z = Mathf.Round(localPosition.z / _cubitSize) * _cubitSize;

            var point = new Vector3(x, y, z);

            return _cubitsParent.TransformPoint(point);
        }

        public void ChangeCubit(CubitData cubitData)
        {
            _selectedCubit = cubitData;
            _placementPreview.gameObject.SetActive(_selectedCubit != null);

            if (_selectedCubit != null)
            {
                _placementPreview.UpdateVisual(_selectedCubit.Color);
            }
        }

        private void BuildShape()
        {
            if (_shapeBlueprint.Count < MIN_CUBITS_REQUIRED)
            {
                return;
            }

            _createShapeButton.interactable = false;
            _resetButton.interactable = false;
            
            _shapeCreator.BuildShape(_shapeBlueprint);
        }

        private void AddShapeToInventory(ShapeData shapeData)
        {
            _playerInventory.AddShape(shapeData);
            ResetState();
        }

        private void ResetState()
        {
            foreach (var shapeIdx in _shapeBlueprint.Keys)
            {
                Destroy(_shapeBlueprint[shapeIdx].gameObject);
            }

            _shapeBlueprint.Clear();
        }
    }
}