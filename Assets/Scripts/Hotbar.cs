using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class Hotbar : MonoBehaviour
    {
        [SerializeField]
        private HotbarSlot _slotPrefab;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private Transform _selection;

        [SerializeField]
        private int _slotCount = 9;
        
        [FormerlySerializedAs("_marbleAddedToInventory"),SerializeField]
        private GameEvent _marbleInventoryUpdate;

        [SerializeField]
        private GameEvent _hotbarSelection;

        private readonly List<HotbarSlot> _slots = new();
        private Dictionary<HotbarSlot, CubitData> _slotsData = new();
        private int _selectedSlotIdx;

        private bool HasEmptySlots => _slotsData.Any(slot => slot.Value == null);
        
        private void Awake()
        {
            for (var i = 0; i < _slotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotsParent);
                slot.Refresh(null, 0);

                _slots.Add(slot);
                _slotsData.Add(slot, null);
            }

            _marbleInventoryUpdate.Subscribe(HandleMarbleUpdate);

            StartCoroutine(UpdateSelectionOnStart());
        }

        private IEnumerator UpdateSelectionOnStart()
        {
            yield return new WaitForEndOfFrame();
            SelectSlot(0);
        }

        private void HandleMarbleUpdate(object marbleCountObject)
        {
            var marbleCount = (MarbleCount)marbleCountObject;
            var marbleSlot = _slotsData.FirstOrDefault(slotData => slotData.Value == marbleCount.Data).Key;
            
            if (marbleSlot != null)
            {
                UpdateMarbleSlot(marbleSlot, marbleCount);
                return;
            }
            
            TryPlaceMarbleInEmptySlot(marbleCount);
        }

        private void UpdateMarbleSlot(HotbarSlot slot, MarbleCount marbleCount)
        {
            if (marbleCount.Count > 0)
            {
                slot.RefreshCount(marbleCount.Count);                
            }
            else
            {
                _slotsData[slot] = null;
                slot.Refresh(null, 0);
                UpdateIfSelected(slot);
            }
        }

        private void TryPlaceMarbleInEmptySlot(MarbleCount marbleCount)
        {
            if (!HasEmptySlots)
            {
                return;
            }

            var firstEmptySlot = _slotsData.First(slot => slot.Value == null).Key;

            firstEmptySlot.Refresh(marbleCount.Data, marbleCount.Count);
            _slotsData[firstEmptySlot] = marbleCount.Data;

            UpdateIfSelected(firstEmptySlot);
        }

        private void UpdateIfSelected(HotbarSlot slot)
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
    }
}