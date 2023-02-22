using System;
using System.Collections.Generic;
using Malyglut.CubitWorld.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Malyglut.CubitWorld.World
{
    public class CubeGrid : MonoBehaviour
    {
        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private Cube _cubePrefab;

        [SerializeField]
        private Vector3Int _dimensions = Vector3Int.one;

        private readonly Dictionary<Vector3, Cube> _cubes = new();
        private readonly List<Vector3Int> _cubeSpatialIndices = new();

        private Cube this[Vector3 position]
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
                    newCube.Initialize(_gameSettings.CubitSize, _gameSettings.CubitCellSize);
                    AddCube(newCube);
                }

                return _cubes[position];
            }
        }

        private bool IsValidGridPosition(Vector3 position)
        {
            var cubeSize = _gameSettings.CubeSize;

            return position.x % cubeSize == 0 && Math.Abs(position.y % cubeSize - cubeSize * .5f) < .00025f &&
                   position.z % cubeSize == 0;
        }

        private void Awake()
        {
            CalculateCubeSpatialIndices();
            // GenerateRandomCubes();
        }

        [Button]
        private void GenerateRandomCubes()
        {
            foreach (var (_, cube) in _cubes)
            {
                Destroy(cube.gameObject);
            }
            
            _cubes.Clear();
            
            for (int x = 0; x < _dimensions.x; x++)
            {
                for (int y = 0; y < _dimensions.y; y++)
                {
                    for (int z = 0; z < _dimensions.z; z++)
                    {
                        var shapeBlueprint = GenerateRandomShapeBlueprint();
                        var worldPosition = new Vector3(x, y, z) * _gameSettings.CubeSize;
                        var cube = WorldPositionToCube(worldPosition);
                        cube.Build(shapeBlueprint);
                    }
                }
            }
        }

        private void CalculateCubeSpatialIndices()
        {
            var value = _gameSettings.CubitsPerCubeAxis % 2;

            for (var x = -value; x <= value; x++)
            {
                for (var y = -value; y <= value; y++)
                {
                    for (var z = -value; z <= value; z++)
                    {
                        _cubeSpatialIndices.Add(new Vector3Int(x, y, z));
                    }
                }
            }
        }

        private void AddCube(Cube cube)
        {
            var cubePosition = cube.transform.position;

            cube.name = $"Cube [{cubePosition.x}, {cubePosition.y}, {cubePosition.z}]";
            _cubes.Add(cubePosition, cube);

            cube.OnDestroy += HandleCubeDestroyed;
        }

        private void HandleCubeDestroyed(Cube cube)
        {
            if (!_cubes.ContainsKey(cube.transform.position))
            {
                return;
            }

            _cubes.Remove(cube.transform.position);
            cube.OnDestroy -= HandleCubeDestroyed;
        }

        [Button]
        private void CombineCubeMeshes()
        {
            foreach (var cube in _cubes.Values)
            {
                cube.CombineMeshes();
            }
        }

        public Vector3 WorldPositionToCubePosition(Vector3 position)
        {
            return GridPosition(position, _gameSettings.CubeSize);
        }

        public Vector3 WorldPositionToCubitPosition(Vector3 position)
        {
            //offset the cubit position so that cubit at the bottom of the cube has an index of -1 
            // position.y -= _gameSettings.CubitSize;
            return GridPosition(position, _gameSettings.CubitSize);
        }

        private Vector3 GridPosition(Vector3 position, float granularity)
        {
            position.y -= _gameSettings.CubeSize * .5f;

            var x = Mathf.Round(position.x / granularity) * granularity;
            var y = Mathf.Round(position.y / granularity) * granularity;
            var z = Mathf.Round(position.z / granularity) * granularity;

            var gridPosition = new Vector3(x, y, z);
            gridPosition.y += _gameSettings.CubeSize * .5f;

            return gridPosition;
        }

        public Cube WorldPositionToCube(Vector3 worldPosition)
        {
            return this[WorldPositionToCubePosition(worldPosition)];
        }

        public bool CubeExists(Vector3 cubePosition)
        {
            return _cubes.ContainsKey(cubePosition);
        }

        private Dictionary<Vector3Int, CubitData> GenerateRandomShapeBlueprint()
        {
            var indicesCopy = new List<Vector3Int>(_cubeSpatialIndices);
            var shapeBlueprint = new Dictionary<Vector3Int, CubitData>();

            var maxCubits = (int)Mathf.Pow(_gameSettings.CubitsPerCubeAxis, 3);
            var cubitCount = Random.Range(1, maxCubits + 1);

            for (int i = 0; i < cubitCount; i++)
            {
                var idx = Random.Range(0, indicesCopy.Count);
                var cubitData = _gameSettings.CubitDatabase.RandomCubitData(); 
                shapeBlueprint.Add(indicesCopy[idx], cubitData);
                indicesCopy.RemoveAt(idx);
            }

            return shapeBlueprint;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var centerPosition = transform.position;
            centerPosition.x += _dimensions.x * .5f * _gameSettings.CubeSize;
            centerPosition.y += _dimensions.y * .5f * _gameSettings.CubeSize;
            centerPosition.z += _dimensions.z * .5f * _gameSettings.CubeSize;
            
            // centerPosition = WorldPositionToCubePosition(centerPosition);

            Gizmos.DrawWireCube(centerPosition,
                new Vector3(_dimensions.x, _dimensions.y, _dimensions.z) * _gameSettings.CubeSize);
        }
    }
}