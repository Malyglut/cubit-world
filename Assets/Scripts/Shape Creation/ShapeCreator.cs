using System;
using System.Collections;
using System.Collections.Generic;
using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.World;
using UnityEngine;

namespace Malyglut.CubitWorld.ShapeCreation
{
    public class ShapeCreator : MonoBehaviour
    {
        public event Action<ShapeData> OnShapeCreated;
        
        [SerializeField]
        private Cubit _cubitPrefab;

        [SerializeField]
        private Cube _cube;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField]
        private ScreenshotCamera _camera;

        private List<Cubit> _cubits = new();

        public void BuildShape(Dictionary<Vector3Int, Cubit> shapeBlueprint)
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
                newCubit.transform.localRotation = Quaternion.identity;

                _cubits.Add(newCubit);
            }

            StartCoroutine(CreateShapeData(shapeBlueprint));
        }

        private IEnumerator CreateShapeData(Dictionary<Vector3Int, Cubit> shapeBlueprint)
        {
            yield return new WaitForEndOfFrame();
            
            var shapeIcon = _camera.TakeScreenshot();

            yield return new WaitForEndOfFrame();
            
            _cube.CombineMeshes();

            var shapeData = new ShapeData
            {
                ShapeBlueprint = new Dictionary<Vector3Int, Cubit>(shapeBlueprint),
                Mesh = _cube.Mesh,
                Materials = _cube.Materials,
                InventoryIcon = shapeIcon
            };

            OnShapeCreated.Invoke(shapeData);
            
            _cube.ResetState();
        }
    }
}