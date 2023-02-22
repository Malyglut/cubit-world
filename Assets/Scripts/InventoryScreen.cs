using Cinemachine;
using UnityEngine;

namespace Malyglut.CubitWorld
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

        [SerializeField]
        private GameEvent _marbleInventoryUpdate;

        [SerializeField]
        private GameEvent _shapeAddedToInventory;

        [SerializeField]
        private GameEvent _inventoryOpened;

        [SerializeField]
        private GameEvent _inventoryClosed;

        [SerializeField]
        private GameEvent _shapeRemovedFromInventory;

        [SerializeField]
        private GameEvent _slotClicked;

        private InventorySlot _selectedSlot;
        private bool _isShown;

        private void Start()
        {
            _grid.Initialize();

            _marbleInventoryUpdate.Subscribe(UpdateMarbles);
            _shapeAddedToInventory.Subscribe(AddShape);
            _shapeRemovedFromInventory.Subscribe(RemoveShape);
            _slotClicked.Subscribe(HandleSlotClick);


            Close();
        }

        private void RemoveShape(object shapeDataObject)
        {
            var shapeData = (ShapeData)shapeDataObject;

            if (_hotbar.HasShape(shapeData))
            {
                _hotbar.RemoveShape(shapeData);
            }
        }

        private void AddShape(object shapeDataObject)
        {
            var shapeData = (ShapeData)shapeDataObject;

            if (_hotbar.HasEmptySlots)
            {
                _hotbar.AddShape(shapeData);
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
                UpdateSelectedSlot(null);
            }

            if (_hotbar.HasEmptySlots || _hotbar.HasMarble(marbleCount.Data))
            {
                _hotbar.RefreshMarbles(marbleCount.Data, marbleCount.Count);
            }
        }

        private void HandleSlotClick(object slotObject)
        {
            var slot = (InventorySlot)slotObject;

            if (_selectedSlot != null)
            {
                _selectedSlot.Deselect();
            }

            _selectedSlot = slot;

            if (slot.Data is CubitData cubitData)
            {
                UpdateSelectedSlot(cubitData);
            }
            else
            {
                UpdateSelectedSlot(null);
            }

            if (slot.Data == null)
            {
                return;
            }

            _selectedSlot.Select();
        }

        private void UpdateSelectedSlot(CubitData cubitData)
        {
            _shapeBuilder.ChangeCubit(cubitData);
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