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
        [FormerlySerializedAs("_camera"),SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [FormerlySerializedAs("_shapePreview"),SerializeField]
        private ShapeBuilder _shapeBuilder;

        [SerializeField]
        private GameObject _interface;

        [SerializeField]
        private HotbarSlot _slotPrefab;

        [SerializeField]
        private Vector2Int _inventroyDimensions = Vector2Int.one;

        [SerializeField]
        private GridLayoutGroup _gridLayout;

        [SerializeField]
        private Transform _slotsParent;

        [SerializeField]
        private Hotbar _hotbar;

        [SerializeField]
        private GameObject _selection;
        
        [SerializeField]
        private GameEvent _marbleInventoryUpdate;

        private bool _isShown;
        private CubitData _selectedCubit;

        private void Awake()
        {
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = _inventroyDimensions.y;

            var slotCount = _inventroyDimensions.x * _inventroyDimensions.y;

            for (int i = 0; i < slotCount; i++)
            {
                var slot = Instantiate(_slotPrefab, _slotsParent);
                slot.Refresh(null, 0);

                slot.OnClick += HandleSlotClick;
            }

            _hotbar.OnSlotClick += HandleSlotClick;
            
            _marbleInventoryUpdate.Subscribe(HandleInventoryUpdate);

            Hide();
        }

        private void HandleInventoryUpdate(object marbleCountObject)
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

        private void HandleSlotClick(HotbarSlot slot)
        {
            UpdateSelectedCubit(slot.Data);

            if (slot.Data == null)
            {
                return;
            }

            _selection.transform.position = slot.transform.position;
        }

        private void UpdateSelectedCubit(CubitData cubitData)
        {
            _selectedCubit = cubitData;
            _selection.SetActive(cubitData != null);
            _shapeBuilder.ChangeCubit(cubitData);
        }

        [Button]
        public void Show()
        {
            _isShown = true;
            _virtualCamera.Priority = 1000;
            _shapeBuilder.gameObject.SetActive(true);
            _interface.SetActive(true);
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _hotbar.HideSelection();
            
            _shapeBuilder.ChangeCubit(null);
            _selection.SetActive(false);
        }

        [Button]
        public void Hide()
        {
            _isShown = false;
            _virtualCamera.Priority = -1;
            _shapeBuilder.gameObject.SetActive(false);
            _interface.SetActive(false);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _hotbar.ShowSelection();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (_isShown)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }
    }
}