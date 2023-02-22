using System.Collections.Generic;
using System.Linq;
using Malyglut.CubitWorld.Data;
using UnityEngine;

namespace Malyglut.CubitWorld.UserInterface.Inventory
{
    public abstract class InventorySlotCollection : MonoBehaviour
    {
        protected readonly List<InventorySlot> _slots = new();
        
        public bool HasEmptySlots => _slots.Any(slot => slot.Data == null);

        public bool HasMarble(CubitData cubitData)
        {
            return _slots.Any(slot => slot.Data is CubitData slotCubit && slotCubit == cubitData);
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
        
        public void AddShape(ShapeData shapeData)
        {
            var firstEmptySlot = _slots.First(slot => slot.Data == null);

            firstEmptySlot.Refresh(shapeData, 1);
        }

        protected abstract void UpdateIfSelected(InventorySlot slot);
        protected abstract void ClearSlot(InventorySlot slot);
    }
}