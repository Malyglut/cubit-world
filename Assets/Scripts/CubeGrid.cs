using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class CubeGrid : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings;
        
        [SerializeField]
        private Cube _cubePrefab;

        private readonly Dictionary<Vector3, Cube> _cubes = new();

        public Cube this[Vector3 position]
        {
            get
            {
                if (!IsValidGridPosition(position))
                {
                    Debug.LogError($"Invalid grid position {position}");
                    return null;
                }
                
                if (!_cubes.ContainsKey(position))
                {
                    var newCube = Instantiate(_cubePrefab, position, Quaternion.identity, transform);
                    AddCube(newCube);
                }

                return _cubes[position];
            }
        }

        private bool IsValidGridPosition(Vector3 position)
        {
            var cubeSize = _gameSettings.CubeSize;
            
            return position.x % cubeSize == 0 && position.y % cubeSize == 0 && position.z % cubeSize == 0;
        }

        private void Awake()
        {
            var _initialCubes = GetComponentsInChildren<Cube>();
            
            foreach (var cube in _initialCubes)
            {
                AddCube(cube);
            }
        }

        private void AddCube(Cube cube)
        {
            var cubePosition = cube.transform.position;
            
            cube.name = $"Cube [{cubePosition.x}, {cubePosition.y}, {cubePosition.z}]";
            _cubes.Add(cubePosition, cube);
        }

        [Button]
        private void CombineCubeMeshes()
        {
            foreach (var cube in _cubes.Values)
            {
                cube.CombineMeshes();
            }
        }
    }
}