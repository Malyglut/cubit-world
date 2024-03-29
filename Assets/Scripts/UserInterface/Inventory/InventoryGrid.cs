﻿using Malyglut.CubitWorld.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Malyglut.CubitWorld.UserInterface.Inventory
{
    public class InventoryGrid : InventorySlotCollection
    {
        [SerializeField]
        private InventorySlot _slotPrefab;

        [SerializeField]
        private GridLayoutGroup _gridLayout;

        [SerializeField]
        private Transform _slotsParent;
        
        [SerializeField]
        private GameSettings _gameSettings;

        public void Initialize()
        {
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            var inventoryDimensions = _gameSettings.InventoryDimensions;
            _gridLayout.constraintCount = inventoryDimensions.y;

            var slotCount = inventoryDimensions.x * inventoryDimensions.y;

            for (int i = 0; i < slotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotsParent);
                slot.Refresh(null, 0);

                _slots.Add(slot);
            }
        }

        protected override void UpdateIfSelected(InventorySlot slot)
        {
            //do nothing
        }

        protected override void ClearSlot(InventorySlot slot)
        {
            //do nothing
        }
    }
}