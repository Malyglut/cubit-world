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

        public void PlaceCubit(Cubit targetCubit, Vector3 placementDirection, CubitData cubitData)
        {
            var targetCube = targetCubit.Cube;
            var cubitPosition = targetCubit.transform.position + placementDirection * _gameSettings.CubitSize;

            Cube parentCube;
            
            if (IsCubitInsideCube(targetCube, cubitPosition))
            {
                parentCube = targetCube;
            }
            else
            {
                var adjacentCubePosition = targetCube.transform.position + placementDirection * _gameSettings.CubeSize;
                parentCube = _grid[adjacentCubePosition];
            }
            
            SpawnCubit(cubitPosition, parentCube, cubitData);
        }

        private bool IsCubitInsideCube(Cube targetCube, Vector3 cubitPosition)
        {
            var newCubitLocalPosition = targetCube.transform.InverseTransformPoint(cubitPosition);
            
            var isInsideCube =  Mathf.Abs(newCubitLocalPosition.x) <= _gameSettings.CubeMaxExtents
                                && Mathf.Abs(newCubitLocalPosition.y) <= _gameSettings.CubeMaxExtents
                                && Mathf.Abs(newCubitLocalPosition.z) <= _gameSettings.CubeMaxExtents;

            return isInsideCube;
        }

        public void PlaceCubit(Vector3 worldPosition, CubitData cubitData)
        {
            var cube = _grid.WorldPositionToCube(worldPosition);
            var cubitPosition = _grid.WorldPositionToCubitPosition(worldPosition);

            SpawnCubit(cubitPosition, cube, cubitData);
        }

        private void SpawnCubit(Vector3 cubitPosition, Cube parentCube, CubitData cubitData)
        {
            var newCubit = Instantiate(_cubitPrefab, cubitPosition, Quaternion.identity);
            newCubit.transform.localScale = Vector3.one * _gameSettings.CubitSize;

            newCubit.Initialize(cubitData, parentCube);
            parentCube.Add(newCubit);
        }
    }
}