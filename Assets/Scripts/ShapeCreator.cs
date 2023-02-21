using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class ShapeCreator : MonoBehaviour
    {
        [SerializeField]
        private Cubit _cubitPrefab;

        [SerializeField]
        private Cube _cube;

        [SerializeField]
        private GameSettings _gameSettings;

        private List<Cubit> _cubits = new();

        public ShapeData BuildShape(Dictionary<Vector3Int, Cubit> shapeBlueprint)
        {
            foreach (var cubit in _cubits)
            {
                Destroy(cubit.gameObject);
            }
            
            _cubits.Clear();
            _cube.ResetState();
            
            foreach (var (positionIdx, cubit) in shapeBlueprint)
            {
                var localPosition = (Vector3)positionIdx * _gameSettings.CubitCellSize; 
                
                var newCubit = Instantiate(_cubitPrefab);
                newCubit.transform.localScale = Vector3.one * _gameSettings.CubitSize;

                newCubit.Initialize(cubit.Data, _cube);
                _cube.Add(newCubit);

                newCubit.transform.localPosition = localPosition;
                
                _cubits.Add(newCubit);
            }

            _cube.CombineMeshes();

            var shapeData = new ShapeData
            {
                ShapeBlueprint = new Dictionary<Vector3Int, Cubit>(shapeBlueprint),
                Mesh = _cube.Mesh,
                Materials = _cube.Materials
            };

            return shapeData;
        }
    }
}