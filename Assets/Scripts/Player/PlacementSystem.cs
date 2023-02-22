using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.World;
using UnityEngine;

namespace Malyglut.CubitWorld.Player
{
    public class PlacementSystem : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private CubeGrid _grid;

        [SerializeField]
        private Cubit _cubitPrefab;

        [SerializeField]
        private CubitPreview _cubitPreview;

        [SerializeField]
        private ShapePreview _shapePreview;

        [SerializeField]
        private GameObject _targetCubePreview;

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
            _targetCubePreview.SetActive(false);
        }

        public void UpdatePreviewPosition(IPlaceableData selectedPlaceableData, Vector3 playerPosition, Cubit targetCubit,
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
                    UpdateShapePreview(playerPosition, cubePosition);
                }
            }
        }

        public void UpdatePreviewPosition(IPlaceableData selectedPlaceableData, Vector3 playerPosition,
            Vector3 targetPosition, Vector3 placementDirection)
        {
            if (selectedPlaceableData is CubitData)
            {
                UpdateCubitPreview(_grid.WorldPositionToCubitPosition(targetPosition+placementDirection*_gameSettings.CubitSize*.5f));
            }

            if (selectedPlaceableData is ShapeData)
            {
                var cubePosition = _grid.WorldPositionToCubePosition(targetPosition);

                if (_grid.CubeExists(cubePosition))
                {
                    _shapePreview.gameObject.SetActive(false);
                    _targetCubePreview.SetActive(false);
                }
                else
                {
                    UpdateShapePreview(playerPosition, cubePosition);
                }
            }
        }

        private void UpdateCubitPreview(Vector3 position)
        {
            _cubitPreview.gameObject.SetActive(true);
            _cubitPreview.transform.position = position;

           UpdateTargetCubePreview(_grid.WorldPositionToCubePosition(position));
        }

        private void UpdateShapePreview(Vector3 playerPosition, Vector3 position)
        {
            const int rotationStep = 90;
            
            var direction = position - playerPosition;

            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            targetRotation.eulerAngles = new Vector3(
                0f,
                Mathf.Round(targetRotation.eulerAngles.y / rotationStep) * rotationStep,
                0f
            );

            _shapePreview.gameObject.SetActive(true);
            _shapePreview.transform.position = position;
            _shapePreview.transform.rotation = targetRotation;

            UpdateTargetCubePreview(position);
        }

        private void UpdateTargetCubePreview(Vector3 position)
        {
            _targetCubePreview.gameObject.SetActive(true);
            _targetCubePreview.transform.position = position;
        }

        public void UpdatePreviewVisual(IPlaceableData placeableData)
        {
            _cubitPreview.gameObject.SetActive(placeableData is CubitData);
            _shapePreview.gameObject.SetActive(placeableData is ShapeData);
            _targetCubePreview.SetActive(placeableData != null);

            if (placeableData is CubitData cubitData)
            {
                _cubitPreview.UpdateVisual(cubitData.Color);
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

            cube.Build(shapeData.ShapeBlueprint);
            
            // foreach (var (positionIdx, cubit) in shapeData.ShapeBlueprint)
            // {
            //     var localPosition = (Vector3)positionIdx * _gameSettings.CubitCellSize;
            //
            //     var newCubit = Instantiate(_cubitPrefab);
            //     newCubit.transform.localScale = Vector3.one * _gameSettings.CubitSize;
            //
            //     newCubit.Initialize(cubit.Data, cube);
            //     cube.Add(newCubit);
            //
            //     newCubit.transform.localPosition = localPosition;
            // }

            cube.transform.rotation = _shapePreview.transform.rotation;
        }
    }
}