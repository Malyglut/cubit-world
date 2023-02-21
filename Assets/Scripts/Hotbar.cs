using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Hotbar : MonoBehaviour
    {
        public event Action<HotbarSlot> OnSlotClick;
        
        [SerializeField]
        private HotbarSlot _slotPrefab;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private Transform _selection;

        [SerializeField]
        private int _slotCount = 9;
        
        // [FormerlySerializedAs("_marbleAddedToInventory"),SerializeField]
        // private GameEvent _marbleInventoryUpdate;

        [SerializeField]
        private GameEvent _hotbarSelection;

        private readonly List<HotbarSlot> _slots = new();
        private Dictionary<HotbarSlot, CubitData> _slotsData = new();
        private int _selectedSlotIdx;
        private bool _inventoryOpen;

        public bool HasEmptySlots => _slotsData.Any(slot => slot.Value == null);

        public bool HasMarble(CubitData cubitData)
        {
            return _slotsData.Any(slotData => slotData.Value == cubitData);
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
            //
            // _marbleInventoryUpdate.Subscribe(HandleMarbleUpdate);

            StartCoroutine(UpdateSelectionOnStart());
        }

        private void HandleSlotClick(HotbarSlot slot)
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
            var marbleSlot = _slotsData.FirstOrDefault(slotData => slotData.Value == cubitData).Key;
            
            if (marbleSlot != null)
            {
                UpdateMarbleSlot(marbleSlot, cubitData, amount);
                return;
            }
            
            TryPlaceMarbleInEmptySlot(cubitData, amount);
        }

        private void UpdateMarbleSlot(HotbarSlot slot, CubitData cubitData, int amount)
        {
            if (amount > 0)
            {
                slot.RefreshCount(amount);                
            }
            else
            {
                _slotsData[slot] = null;
                slot.Refresh(null, 0);
                UpdateIfSelected(slot);
            }
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
    }
}