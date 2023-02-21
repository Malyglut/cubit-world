using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Malyglut.CubitWorld
{
    public class InventoryScreen : MonoBehaviour
    {
        [FormerlySerializedAs("_camera"), SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [FormerlySerializedAs("_shapePreview"), SerializeField]
        private ShapeBuilder _shapeBuilder;

        [SerializeField]
        private GameObject _interface;

        [SerializeField]
        private InventorySlot _slotPrefab;

        [SerializeField]
        private Vector2Int _inventroyDimensions = Vector2Int.one;

        [SerializeField]
        private GridLayoutGroup _gridLayout;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private Hotbar _hotbar;

        [SerializeField]
        private GameEvent _marbleInventoryUpdate;

        [FormerlySerializedAs("_shapeInventoryUpdate"), SerializeField]
        private GameEvent _shapeAddedToInventory;

        private bool _isShown;
        private CubitData _selectedCubit;

        [SerializeField]
        private GameEvent _inventoryOpened;

        [SerializeField]
        private GameEvent _inventoryClosed;

        [SerializeField]
        private GameEvent _shapeRemovedFromInventory;

        [SerializeField]
        private GameEvent _slotClicked;

        private InventorySlot _selectedSlot;

        private void Start()
        {
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = _inventroyDimensions.y;

            var slotCount = _inventroyDimensions.x * _inventroyDimensions.y;

            for (int i = 0; i < slotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotsParent);
                slot.Refresh(null, 0);
            }

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

            if (marbleCount.Data == _selectedCubit && marbleCount.Count <= 0)
            {
                UpdateSelectedCubit(null);
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
                UpdateSelectedCubit(cubitData);
            }

            if (slot.Data == null)
            {
                return;
            }

            _selectedSlot.Select();
        }

        private void UpdateSelectedCubit(CubitData cubitData)
        {
            _selectedCubit = cubitData;
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