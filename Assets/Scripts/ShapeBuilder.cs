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
        private Cubit _cubitPrefab;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private Transform _cubitsParent;

        [SerializeField]
        private Transform _shapeContainer;

        [SerializeField]
        private float _rotationSpeed = 25f;

        private float _cubitSize;
        private Vector3 _dragStartMousePosition = Vector3.negativeInfinity;
        private Quaternion _initialRotation;

        private void Awake()
        {
            _cubitSize = 1f / _gameSettings.CubitsPerCubeAxis;
            _initialRotation = _shapeContainer.rotation;
        }

        private void Update()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (Input.GetMouseButtonDown(1))
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, 10000f, _shapePreviewLayer))
                {
                    // Debug.Log(_cubitsParent.InverseTransformPoint(hit.point).ToString("F8"));

                    var cubitPosition = CubitPosition(hit.point);
                    var cubit = Instantiate(_cubitPrefab, cubitPosition, _cubitsParent.rotation, _cubitsParent);
                    cubit.transform.localScale = Vector3.one * _cubitSize;
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                _dragStartMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2))
            {
                var mouseX = Input.GetAxisRaw("Mouse X");

                mouseX = mouseX != 0f ? Mathf.Sign(mouseX) : 0f;

                var directionVector = new Vector3(0f, mouseX, 0f);

                if (directionVector.magnitude > 0f)
                {
                    directionVector *= Time.deltaTime * _rotationSpeed;
                    _shapeContainer.Rotate(directionVector);
                }
            }
        }

        private void OnDrawGizmos()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 10000f, _shapePreviewLayer))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(hit.point, .05f);
                Vector3 point;

                var targetCubit = hit.transform.GetComponent<Cubit>();
                if (targetCubit != null)
                {
                    var localNormal = _cubitsParent.InverseTransformDirection(hit.normal);
                    point = CubitPosition(targetCubit.transform.position + hit.normal * _cubitSize);
                }
                else
                {
                    point = CubitPosition(hit.point);
                }

                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(point, .05f);
                Gizmos.DrawWireCube(point, Vector3.one * _cubitSize);
            }
        }

        private Vector3 CubitPosition(Vector3 position)
        {
            var localPosition = _cubitsParent.InverseTransformPoint(position);
            //nasty hack to avoid Cubits being placed inside one another when raycasting against a Cubit due to floating point errors
            // localPosition -= Vector3.one * 0.0000025f;

            // var x = GridIndex(localPosition.x, _cubitSize);
            // var y = GridIndex(localPosition.y, _cubitSize);
            // var z = GridIndex(localPosition.z, _cubitSize);

            var x = Mathf.Round(localPosition.x / _cubitSize) * _cubitSize;
            var y = Mathf.Round(localPosition.y / _cubitSize) * _cubitSize;
            var z = Mathf.Round(localPosition.z / _cubitSize) * _cubitSize;

            x = Mathf.Clamp(x, -_cubitSize, _cubitSize);
            y = Mathf.Clamp(y, -_cubitSize, _cubitSize);
            z = Mathf.Clamp(z, -_cubitSize, _cubitSize);

            var point = new Vector3(x, y, z);

            return _cubitsParent.TransformPoint(point);
        }
    }
}