using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Hotbar : MonoBehaviour
    {
        public event Action<InventorySlot> OnSlotClick;
        
        [SerializeField]
        private InventorySlot _slotPrefab;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private Transform _selection;

        [SerializeField]
        private int _slotCount = 9;
        
        [SerializeField]
        private GameEvent _hotbarSelection;

        private readonly List<InventorySlot> _slots = new();
        private Dictionary<InventorySlot, IPlaceableData> _slotsData = new();
        private int _selectedSlotIdx;
        private bool _inventoryOpen;

        public bool HasEmptySlots => _slotsData.Any(slot => slot.Value == null);

        public bool HasMarble(CubitData cubitData)
        {
            return _slotsData.Any(slotData => slotData.Value is CubitData slotCubit && slotCubit == cubitData);
        }
        
        private void Awake()
        {
            for (var i = 0; i < _slotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotsParent);
                slot.Refresh(null, 0);

                slot.OnClick += HandleSlotClick;

                _slots.Add(slot);
                _slotsData.Add(slot, null);
            }

            StartCoroutine(UpdateSelectionOnStart());
        }

        private void HandleSlotClick(InventorySlot slot)
        {
            OnSlotClick.Invoke(slot);
        }

        private IEnumerator UpdateSelectionOnStart()
        {
            yield return new WaitForEndOfFrame();
            SelectSlot(0);
        }

        public void RefreshMarbles(CubitData cubitData, int amount)
        {
            var marbleSlot = _slotsData.FirstOrDefault(slotData => slotData.Value is CubitData slotCubit && slotCubit == cubitData).Key;
            
            if (marbleSlot != null)
            {
                UpdateMarbleSlot(marbleSlot, amount);
            }
            else
            {
                TryPlaceMarbleInEmptySlot(cubitData, amount);
            }
        }

        public void AddShape(ShapeData shapeData)
        {
            var firstEmptySlot = _slotsData.First(slot => slot.Value == null).Key;

            firstEmptySlot.Refresh(shapeData, 1);
            _slotsData[firstEmptySlot] = shapeData;
        }

        private void UpdateMarbleSlot(InventorySlot slot, int amount)
        {
            if (amount > 0)
            {
                slot.RefreshCount(amount);                
            }
            else
            {
                ClearSlot(slot);
            }
        }

        private void ClearSlot(InventorySlot slot)
        {
            _slotsData[slot] = null;
            slot.Refresh(null, 0);
            UpdateIfSelected(slot);
        }

        private void TryPlaceMarbleInEmptySlot(CubitData cubitData, int amount)
        {
            if (!HasEmptySlots)
            {
                return;
            }

            var firstEmptySlot = _slotsData.First(slot => slot.Value == null).Key;

            firstEmptySlot.Refresh(cubitData, amount);
            _slotsData[firstEmptySlot] = cubitData;

            UpdateIfSelected(firstEmptySlot);
        }

        private void UpdateIfSelected(InventorySlot slot)
        {
            if (_slots[_selectedSlotIdx] == slot)
            {
                SelectSlot(_selectedSlotIdx);
            }
        }

        private void SelectSlot(int slotIdx)
        {
            if (slotIdx < 0)
            {
                slotIdx = _slotCount - 1;
            }

            if (slotIdx >= _slotCount)
            {
                slotIdx = 0;
            }

            var selectedSlot = _slots[slotIdx];
            _selection.position = selectedSlot.transform.position;
            _selectedSlotIdx = slotIdx;
            
            _hotbarSelection.Raise(_slotsData[selectedSlot]);
        }

        private void Update()
        {
            UpdateScrollSelection();
        }

        private void UpdateScrollSelection()
        {
            if (_inventoryOpen)
            {
                return;
            }

            var scrollValue = Input.GetAxisRaw("Mouse ScrollWheel");

            if (scrollValue == 0f)
            {
                return;
            }

            if (scrollValue > 0)
            {
                SelectSlot(_selectedSlotIdx - 1);
            }
            else
            {
                SelectSlot(_selectedSlotIdx + 1);
            }
        }

        public void ShowSelection()
        {
            _inventoryOpen = false;
            _selection.gameObject.SetActive(true);
        }

        public void HideSelection()
        {
            _inventoryOpen = true;
            _selection.gameObject.SetActive(false);
        }

        public bool HasShape(ShapeData shapeData)
        {
            return _slotsData.Any(slotData => slotData.Value is ShapeData shape && shape == shapeData);
        }

        public void RemoveShape(ShapeData shapeData)
        {
            var shapeSlot = _slotsData.First(slotData => slotData.Value is ShapeData shape && shape == shapeData).Key;
            ClearSlot(shapeSlot);
        }
    }
}