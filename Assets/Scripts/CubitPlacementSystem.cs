using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class CubitPlacementSystem : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private CubeGrid _grid;

        [SerializeField]
        private Cubit _cubitPrefab;

        [SerializeField]
        private CubitPreview _placementPreview;

        public bool HasValidPlacementPosition => _placementPreview.gameObject.activeSelf;

        private void Awake()
        {
            HidePreview();
        }

        public void PlaceCubit(CubitData cubitData)
        {
            var cubitPosition = _placementPreview.transform.position;
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
            _placementPreview.gameObject.SetActive(false);
        }

        public void UpdatePreview(Cubit targetCubit, Vector3 placementDirection)
        {
            UpdatePreviewInternal(targetCubit.transform.position + placementDirection * _gameSettings.CubitSize);
        }

        public void UpdatePreview(Vector3 position)
        {
            UpdatePreviewInternal(_grid.WorldPositionToCubitPosition(position));
        }

        private void UpdatePreviewInternal(Vector3 position)
        {
            _placementPreview.gameObject.SetActive(true);
            _placementPreview.transform.position = position;
        }

        public void UpdatePreview(CubitData cubitData)
        {
            _placementPreview.gameObject.SetActive(cubitData != null);

            if (cubitData != null)
            {
                _placementPreview.UpdateColor(cubitData.Color);
            }
        }
    }
}