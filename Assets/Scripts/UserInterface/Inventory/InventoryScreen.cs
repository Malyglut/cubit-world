using Cinemachine;
using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.ShapeCreation;
using Malyglut.CubitWorld.Utilties;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Malyglut.CubitWorld.UserInterface.Inventory
{
    public class InventoryScreen : MonoBehaviour
    {
        [SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [SerializeField]
        private ShapeBuilder _shapeBuilder;

        [SerializeField]
        private InventoryGrid _grid;

        [SerializeField]
        private GameObject _interface;

        [SerializeField]
        private Hotbar _hotbar;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _marbleInventoryUpdate;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _shapeAddedToInventory;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _inventoryOpened;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _inventoryClosed;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _shapeRemovedFromInventory;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _slotClicked;
        
        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _slotDragFinalized;

        private InventorySlot _selectedSlot;
        private bool _isShown;

        private void Start()
        {
            _grid.Initialize();
            _hotbar.Initialize();

            _marbleInventoryUpdate.Subscribe(UpdateMarbles);
            _shapeAddedToInventory.Subscribe(AddShape);
            _shapeRemovedFromInventory.Subscribe(RemoveShape);
            _slotClicked.Subscribe(HandleSlotClick);
            _slotDragFinalized.Subscribe(RefreshSelectedSlot);

            Close();
        }

        private void OnDestroy()
        {
            _marbleInventoryUpdate.Unsubscribe(UpdateMarbles);
            _shapeAddedToInventory.Unsubscribe(AddShape);
            _shapeRemovedFromInventory.Unsubscribe(RemoveShape);
            _slotClicked.Unsubscribe(HandleSlotClick);
            _slotDragFinalized.Unsubscribe(RefreshSelectedSlot);
        }

        private void RefreshSelectedSlot()
        {
            if (_selectedSlot != null && (_selectedSlot.Data == null || _selectedSlot.Data is ShapeData))
            {
                _selectedSlot.Deselect();
                _selectedSlot = null;
            }
            
            UpdateSelectedSlot(_selectedSlot);
        }

        private void RemoveShape(object shapeDataObject)
        {
            var shapeData = (ShapeData)shapeDataObject;

            //can only be placed from hotbar so remove from hotbar
            if (_hotbar.HasShape(shapeData))
            {
                _hotbar.RemoveShape(shapeData);
            }
        }

        private void AddShape(object shapeDataObject)
        {
            var shapeData = (ShapeData)shapeDataObject;

            //prioritize adding to hotbar
            if (_hotbar.HasEmptySlots)
            {
                _hotbar.AddShape(shapeData);
            }
            else
            {
                _grid.AddShape(shapeData);
            }
        }

        private void UpdateMarbles(object marbleCountObject)
        {
            var marbleCount = (MarbleCount)marbleCountObject;

            if (marbleCount.Data == null)
            {
                return;
            }

            if (_selectedSlot!= null && _selectedSlot.Data == marbleCount.Data && marbleCount.Count <= 0)
            {
                _selectedSlot.Deselect();
                _selectedSlot = null;
                UpdateSelectedSlot(null);
            }

            //check inventory first
            if (_grid.HasMarble(marbleCount.Data))
            {
                _grid.RefreshMarbles(marbleCount.Data, marbleCount.Count);
            }
            //prioritize adding to hotbar
            else if (_hotbar.HasEmptySlots || _hotbar.HasMarble(marbleCount.Data))
            {
                _hotbar.RefreshMarbles(marbleCount.Data, marbleCount.Count);
            }
            //if hotbar is full, add to inventory
            else
            {
                _grid.RefreshMarbles(marbleCount.Data, marbleCount.Count);
            }
        }

        private void HandleSlotClick(object slotObject)
        {
            var slot = (InventorySlot)slotObject;

            if (_selectedSlot != null)
            {
                _selectedSlot.Deselect();
            }

            UpdateSelectedSlot(slot);
        }

        private void UpdateSelectedSlot(InventorySlot slot)
        {
            _selectedSlot = slot;
            var slotData = _selectedSlot != null ? _selectedSlot.Data : null;

            if (slotData == null)
            {
                _shapeBuilder.ChangeCubit(null);
            }
            else if(slotData is CubitData cubitData)
            {
                _shapeBuilder.ChangeCubit(cubitData);
            }

            if (slotData == null || slotData is not CubitData)
            {
                return;
            }

            _selectedSlot.Select();
        }

        private void Open()
        {
            _isShown = true;
            _virtualCamera.Priority = 1000;
            _shapeBuilder.gameObject.SetActive(true);
            _interface.SetActive(true);

            _hotbar.HideSelection();
            _shapeBuilder.ChangeCubit(null);

            _inventoryOpened.Raise();
        }

        private void Close()
        {
            _isShown = false;
            _virtualCamera.Priority = -1;
            _shapeBuilder.gameObject.SetActive(false);
            _interface.SetActive(false);

            _hotbar.ShowSelection();

            if (_selectedSlot != null)
            {
                _selectedSlot.Deselect();
                _selectedSlot = null;
            }

            _inventoryClosed.Raise();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isShown)
                {
                    Close();
                }
                else
                {
                    Open();
                }
            }
        }
    }
}