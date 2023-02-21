using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Malyglut.CubitWorld
{
    public class InventorySlot : MonoBehaviour, IPointerDownHandler
    {
        public event Action<InventorySlot> OnClick;
        
        [SerializeField]
        private TextMeshProUGUI _count;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private GameObject _contentsObject;

        public IPlaceableData Data { get; private set; }

        public void Refresh(IPlaceableData data, int count)
        {
            Data = data;
            _contentsObject.SetActive(Data != null && count>0);
            
            if (Data == null)
            {
                return;
            }

            _icon.sprite = data.Icon;
            
            if(data is CubitData cubitData)
            {
                RefreshForCubit(cubitData);
            }
            
            RefreshCount(count);
        }

        private void RefreshForCubit(CubitData cubitData)
        {
            _icon.color = cubitData.Color;
        }

        public void RefreshCount(int count)
        {
            _count.SetText(count.ToString("00"));

            if (count <= 0)
            {
                _contentsObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick.Invoke(this);
        }
    }
}