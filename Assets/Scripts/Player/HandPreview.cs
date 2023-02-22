using System;
using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.Utilties;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld.Player
{
    public class HandPreview : MonoBehaviour
    {
        [FormerlySerializedAs("_cubitPreview"),SerializeField]
        private CubitPreview _marblePreview;

        [SerializeField]
        private ShapePreview _shapePreview;
        
        [SerializeField]
        private GameObject _previewObject;

        [SerializeField]
        private GameEvent _hotbarSelection;
        
        [SerializeField]
        private GameEvent _inventoryOpened;
        
        [SerializeField]
        private GameEvent _inventoryClosed;

        private void Awake()
        {
            _hotbarSelection.Subscribe(UpdatePreview);
            _inventoryOpened.Subscribe(HidePreview);
            _inventoryClosed.Subscribe(ShowPreview);
        }

        private void OnDestroy()
        {
            _hotbarSelection.Unsubscribe(UpdatePreview);
            _inventoryOpened.Unsubscribe(HidePreview);
            _inventoryClosed.Unsubscribe(ShowPreview);
        }

        private void ShowPreview()
        {
            _previewObject.SetActive(true);
        }

        private void HidePreview()
        {
            _previewObject.SetActive(false);
        }

        private void UpdatePreview(object placeableDataObject)
        {
            _marblePreview.gameObject.SetActive(placeableDataObject is CubitData);
            _shapePreview.gameObject.SetActive(placeableDataObject is ShapeData);
            
            if(placeableDataObject is CubitData cubitData)
            {
                _marblePreview.UpdateVisual(cubitData.Color);
            }
            else if(placeableDataObject is ShapeData shapeData)
            {
                _shapePreview.UpdateVisual(shapeData.Mesh, shapeData.Materials);
            }
        }
    }
}