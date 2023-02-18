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
            var newCubitPosition = targetCubit.transform.position + placementDirection * _gameSettings.CubitSize;

            var newCubit = Instantiate(_cubitPrefab, newCubitPosition, Quaternion.identity);
            newCubit.transform.localScale = Vector3.one * _gameSettings.CubitSize;

            Cube parentCube;

            
            if (IsCubitInsideCube(targetCube, newCubit))
            {
                parentCube = targetCube;
            }
            else
            {
                var adjacentCubePosition = targetCube.transform.position + placementDirection * _gameSettings.CubeSize;
                parentCube = _grid[adjacentCubePosition];
            }

            newCubit.Initialize(cubitData, parentCube);
            parentCube.Add(newCubit);
        }

        private bool IsCubitInsideCube(Cube targetCube, Cubit cubit)
        {
            var newCubitLocalPosition = targetCube.transform.InverseTransformPoint(cubit.transform.position);

            var isInsideCube = Mathf.Abs(newCubitLocalPosition.x) <= _gameSettings.CubeMaxExtents
                               && Mathf.Abs(newCubitLocalPosition.y) <= _gameSettings.CubeMaxExtents
                               && Mathf.Abs(newCubitLocalPosition.z) <= _gameSettings.CubeMaxExtents;

            return isInsideCube;
        }
    }
}