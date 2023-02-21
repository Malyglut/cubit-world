using System;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class InventoryDragController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInventory _inventory;

        [SerializeField]
        private InventorySlotVisual _draggedObject;

        [SerializeField]
        private GameEvent _slotDragBegin;

        [SerializeField]
        private GameEvent _slotDragEnd;

        [SerializeField]
        private GameEvent _slotPointerEnter;

        private bool _isDragging;
        private InventorySlot _source;
        private InventorySlot _target;

        private void Awake()
        {
            _slotDragBegin.Subscribe(StartDragging);
            _slotDragEnd.Subscribe(StopDragging);
            _slotPointerEnter.Subscribe(UpdateTarget);
            
            _draggedObject.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_isDragging)
            {
                return;
            }

            var dragPosition = Input.mousePosition;

            transform.InverseTransformPoint(dragPosition);
            dragPosition.z = 0f;
            _draggedObject.transform.localPosition = dragPosition;
        }

        private void UpdateTarget(object slotObject)
        {
            if (!_isDragging)
            {
                return;
            }

            _target = (InventorySlot)slotObject;
        }

        private void StopDragging(object slotObject)
        {
            if (!_isDragging)
            {
                return;
            }

            _isDragging = false;
            _draggedObject.gameObject.SetActive(false);

            FinalizeDrag();
        }

        private void FinalizeDrag()
        {
            if (_target == null)
            {
                return;
            }
            
            var sourceData = _source.Data;
            var targetData = _target.Data;

            _source.Refresh(targetData, GetMarbleCount(targetData));
            _target.Refresh(sourceData, GetMarbleCount(sourceData));
        }

        private void StartDragging(object slotObject)
        {
            _source = (InventorySlot)slotObject;
            _target = null;
            _isDragging = true;

            _draggedObject.Refresh(_source.Data, GetMarbleCount(_source.Data));
            _draggedObject.gameObject.SetActive(true);
        }

        private int GetMarbleCount(IPlaceableData placeableData)
        {
            return placeableData is CubitData cubitData ? _inventory.MarbleCount(cubitData) : 0;
        }
    }
}