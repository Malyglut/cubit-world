using System;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class ShapeBuilder : MonoBehaviour
    {
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

        private float _cubitSize;
        private Quaternion _initialRotation;
        private CubitData _selectedCubit;
        private bool _rotationInProgress;
        private bool HasValidPlacementPosition => _placementPreview.gameObject.activeSelf;

        private void OnEnable()
        {
            transform.rotation = _initialRotation;
        }

        private void Awake()
        {
            _cubitSize = 1f / _gameSettings.CubitsPerCubeAxis;
            _initialRotation = _shapeContainer.rotation;
            _placementPreview.gameObject.SetActive(false);
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
            var cubit = Instantiate(_cubitPrefab, _placementPreview.transform.position, _cubitsParent.rotation,
                _cubitsParent);
            cubit.transform.localScale = Vector3.one * _cubitSize;

            cubit.Initialize(_selectedCubit, null);
            cubit.PlayPlacementAnimation();

            _playerInventory.SubtractMarbles(_selectedCubit, 1);
        }

        private void DestroyCubit(Cubit cubit)
        {
            Destroy(cubit.gameObject);
            _playerInventory.AddMarbles(cubit.Data, 1);
        }

        private void UpdatePreview()
        {
            if (_selectedCubit != null && !_rotationInProgress)
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, 10000f, _shapePreviewLayer | _cubitsLayer))
                {
                    _placementPreview.gameObject.SetActive(true);

                    var cubitPosition = CubitPosition(hit.point + hit.normal * (_cubitSize * .5f));
                    _placementPreview.transform.position = cubitPosition;
                    _placementPreview.transform.rotation = _cubitsParent.rotation;
                }
                else
                {
                    _placementPreview.gameObject.SetActive(false);
                }
            }
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
                _placementPreview.UpdateColor(_selectedCubit.Color);
            }
        }
    }
}