using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Hotbar : MonoBehaviour
    {
        [SerializeField]
        private InventorySlot _slotPrefab;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private GameEvent _hotbarSelection;

        [SerializeField]
        private GameSettings _gameSettings;

        private readonly List<InventorySlot> _slots = new();
        private int _selectedSlotIdx = -1;
        private bool _inventoryOpen;

        public bool HasEmptySlots => _slots.Any(slot => slot.Data == null);

        public bool HasMarble(CubitData cubitData)
        {
            return _slots.Any(slot => slot.Data is CubitData slotCubit && slotCubit == cubitData);
        }

        private void Awake()
        {
            for (var i = 0; i < _gameSettings.HotbarSlotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotsParent);
                slot.Refresh(null, 0);

                _slots.Add(slot);
            }

            StartCoroutine(UpdateSelectionOnStart());
        }

        private IEnumerator UpdateSelectionOnStart()
        {
            yield return new WaitForEndOfFrame();
            SelectSlot(0);
        }

        public void RefreshMarbles(CubitData cubitData, int amount)
        {
            var marbleSlot = _slots
                .FirstOrDefault(slot => slot.Data == cubitData);

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
            var firstEmptySlot = _slots.First(slot => slot.Data == null);

            firstEmptySlot.Refresh(shapeData, 1);
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
            slot.Refresh(null, 0);
            UpdateIfSelected(slot);
        }

        private void TryPlaceMarbleInEmptySlot(CubitData cubitData, int amount)
        {
            if (!HasEmptySlots)
            {
                return;
            }

            var firstEmptySlot = _slots.First(slot => slot.Data == null);

            firstEmptySlot.Refresh(cubitData, amount);

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
                slotIdx = _gameSettings.HotbarSlotCount - 1;
            }

            if (slotIdx >= _gameSettings.HotbarSlotCount)
            {
                slotIdx = 0;
            }


            DeselectCurrentSlot();

            var selectedSlot = _slots[slotIdx];
            selectedSlot.Select();
            _selectedSlotIdx = slotIdx;

            _hotbarSelection.Raise(selectedSlot.Data);
        }

        private void DeselectCurrentSlot()
        {
            if (_selectedSlotIdx >= 0 && _selectedSlotIdx < _slots.Count)
            {
                _slots[_selectedSlotIdx].Deselect();
            }
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
            SelectCurrentSlot();
            SelectSlot(_selectedSlotIdx);
        }

        private void SelectCurrentSlot()
        {
            if (_selectedSlotIdx >= 0 && _selectedSlotIdx < _slots.Count)
            {
                _slots[_selectedSlotIdx].Select();
            }
        }

        public void HideSelection()
        {
            _inventoryOpen = true;
            DeselectCurrentSlot();
        }

        public bool HasShape(ShapeData shapeData)
        {
            return _slots.Any(slot => slot.Data == shapeData);
        }

        public void RemoveShape(ShapeData shapeData)
        {
            var shapeSlot = _slots.First(slot => slot.Data == shapeData);
            ClearSlot(shapeSlot);
        }
    }
}