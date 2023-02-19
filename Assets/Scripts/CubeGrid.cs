using System;
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

        [SerializeField]
        private GameObject _plane;

        [SerializeField]
        private Vector3Int _dimensions = Vector3Int.one;

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

            return position.x % cubeSize == 0 && Math.Abs(position.y % cubeSize - cubeSize*.5f) < .00025f && position.z % cubeSize == 0;
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
            position.y -= _gameSettings.CubitSize * .5f;
            return GridPosition(position, _gameSettings.CubitSize);
        }

        private Vector3 GridPosition(Vector3 position, float granularity)
        {
            var x = GridIndex(position.x, granularity);
            var y = GridIndex(position.y, granularity);
            var z = GridIndex(position.z, granularity);

            var gridPosition = new Vector3(x, y, z) * granularity;
            gridPosition.y += _gameSettings.CubeSize * .5f;
            
            return gridPosition;
        }

        private int GridIndex(float positionComponent, float granularity)
        {
            var size = granularity;
            var halfSize = size * .5f;

            var idx = (int)(positionComponent / size);
            var remainder = positionComponent % size;

            idx += (int)(remainder / halfSize);

            return idx;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            var center = WorldPositionToCubePosition(transform.position);
            
            Gizmos.DrawWireCube(center,
                new Vector3(_dimensions.x, _dimensions.y, _dimensions.z) * _gameSettings.CubeSize);
        }

        public Cube WorldPositionToCube(Vector3 worldPosition)
        {
            return this[WorldPositionToCubePosition(worldPosition)];
        }
    }
}