using System.Collections;
using System.Linq;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Hotbar : InventorySlotCollection
    {
        [SerializeField]
        private InventorySlot _slotPrefab;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private GameEvent _hotbarSelection;

        [SerializeField]
        private GameSettings _gameSettings;

        private int _selectedSlotIdx = -1;
        private bool _inventoryOpen;

        public void Initialize()
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

        protected override void ClearSlot(InventorySlot slot)
        {
            slot.Refresh(null, 0);
            UpdateIfSelected(slot);
        }

        protected override void UpdateIfSelected(InventorySlot slot)
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