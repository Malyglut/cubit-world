using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class InventoryScreen : MonoBehaviour
    {
        [FormerlySerializedAs("_camera"),SerializeField]
        private CinemachineVirtualCamera _virtualCamera;

        [SerializeField]
        private GameObject _shapePreview;

        [SerializeField]
        private GameObject _interface;

        private bool _isShown;

        private void Awake()
        {
            Hide();
        }

        [Button]
        public void Show()
        {
            _isShown = true;
            _virtualCamera.Priority = 1000;
            _shapePreview.SetActive(true);
            _interface.SetActive(true);
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        [Button]
        public void Hide()
        {
            _isShown = false;
            _virtualCamera.Priority = -1;
            _shapePreview.SetActive(false);
            _interface.SetActive(false);
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
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