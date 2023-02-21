using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private CubeGrid _grid;

        [SerializeField]
        private Cubit _cubitPrefab;

        [FormerlySerializedAs("_placementPreview"), SerializeField]
        private CubitPreview _cubitPreview;

        [SerializeField]
        private ShapePreview _shapePreview;

        public bool HasValidPlacementPosition => _cubitPreview.gameObject.activeSelf || _shapePreview.gameObject.activeSelf;

        private void Awake()
        {
            HidePreview();
        }

        public void PlaceCubit(CubitData cubitData)
        {
            var cubitPosition = _cubitPreview.transform.position;
            var cube = _grid.WorldPositionToCube(cubitPosition);
            SpawnCubit(cubitPosition, cube, cubitData);
        }

        private void SpawnCubit(Vector3 cubitPosition, Cube parentCube, CubitData cubitData)
        {
            var newCubit = Instantiate(_cubitPrefab, cubitPosition, Quaternion.identity);
            newCubit.transform.localScale = Vector3.one * _gameSettings.CubitSize;

            newCubit.Initialize(cubitData, parentCube);
            newCubit.PlayPlacementAnimation();
            parentCube.Add(newCubit);
        }

        public void HidePreview()
        {
            _cubitPreview.gameObject.SetActive(false);
            _shapePreview.gameObject.SetActive(false);
        }

        public void UpdatePreviewPosition(IPlaceableData selectedPlaceableData, Cubit targetCubit,
            Vector3 placementDirection)
        {
            if (selectedPlaceableData is CubitData)
            {
                UpdateCubitPreview(targetCubit.transform.position + placementDirection * _gameSettings.CubitSize);
            }

            if (selectedPlaceableData is ShapeData)
            {
                var cubePosition = targetCubit.Cube.transform.position + placementDirection * _gameSettings.CubeSize;

                if (_grid.CubeExists(cubePosition))
                {
                    _shapePreview.gameObject.SetActive(false);
                }
                else
                {
                    UpdateShapePreview(cubePosition);
                }
            }
        }

        public void UpdatePreviewPosition(IPlaceableData selectedPlaceableData, Vector3 position)
        {
            if (selectedPlaceableData is CubitData)
            {
                UpdateCubitPreview(_grid.WorldPositionToCubitPosition(position));
            }

            if (selectedPlaceableData is ShapeData)
            {
                var cubePosition = _grid.WorldPositionToCubePosition(position);

                if (_grid.CubeExists(cubePosition))
                {
                    _shapePreview.gameObject.SetActive(false);
                }
                else
                {
                    UpdateShapePreview(cubePosition);
                }
            }
        }

        private void UpdateCubitPreview(Vector3 position)
        {
            _cubitPreview.gameObject.SetActive(true);
            _cubitPreview.transform.position = position;
        }

        private void UpdateShapePreview(Vector3 position)
        {
            _shapePreview.gameObject.SetActive(true);
            _shapePreview.transform.position = position;
        }

        public void UpdatePreviewVisual(IPlaceableData placeableData)
        {
            _cubitPreview.gameObject.SetActive(placeableData is CubitData);
            _shapePreview.gameObject.SetActive(placeableData is ShapeData);

            if (placeableData is CubitData cubitData)
            {
                _cubitPreview.UpdateColor(cubitData.Color);
            }

            if (placeableData is ShapeData shapeData)
            {
                _shapePreview.UpdateVisual(shapeData.Mesh, shapeData.Materials);
            }
        }

        public void PlaceShape(ShapeData shapeData)
        {
            var shapePosition = _shapePreview.transform.position;
            var cube = _grid.WorldPositionToCube(shapePosition);

            foreach (var (positionIdx, cubit) in shapeData.ShapeBlueprint)
            {
                var localPosition = (Vector3)positionIdx * _gameSettings.CubitCellSize;

                var newCubit = Instantiate(_cubitPrefab);
                newCubit.transform.localScale = Vector3.one * _gameSettings.CubitSize;

                newCubit.Initialize(cubit.Data, cube);
                cube.Add(newCubit);

                newCubit.transform.localPosition = localPosition;
            }
        }
        //
        // private Dictionary<Vector3Int, Cubit> RotateShape(Vector3 referencePosition, Vector3 targetPosition,Dictionary<Vector3Int, Cubit> blueprint)
        // {
        //     var rotationModifier = ShapeRotationModifier(referencePosition, targetPosition);
        //
        //     if (rotationModifier == Vector3Int.one)
        //     {
        //         return blueprint;
        //     }
        //
        //     var rotatedBlueprint = new Dictionary<Vector3Int, Cubit>();
        //     
        //     foreach (var (gridIndex, cubit) in blueprint)
        //     {
        //         rotatedBlueprint.Add(gridIndex*rotationModifier, cubit);
        //     }
        //
        //     return rotatedBlueprint;
        // }
        //
        // private Vector3Int ShapeRotationModifier(Vector3 referencePosition, Vector3 targetPosition)
        // {
        //     var referenceCube = _grid.WorldPositionToCubePosition(referencePosition);
        //     var targetCube = _grid.WorldPositionToCubePosition(targetPosition);
        //
        //     if (referenceCube.z != targetCube.z)
        //     {
        //         return referenceCube.z > targetCube.z ? new Vector3Int(-1, 1, -1) : Vector3Int.one;
        //     }
        //
        //     if (referenceCube.x != targetCube.z)
        //     {
        //         return referenceCube.x > targetCube.z ? Vector3Int.one : new Vector3Int(-1, 1, 1);
        //     }
        //     
        //     return Vector3Int.one;
        // }
    }
}