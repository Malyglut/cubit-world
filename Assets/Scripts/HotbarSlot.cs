using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Malyglut.CubitWorld
{
    public class HotbarSlot : MonoBehaviour, IPointerDownHandler
    {
        public event Action<HotbarSlot> OnClick;
        
        [SerializeField]
        private TextMeshProUGUI _count;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private GameObject _contentsObject;

        public CubitData Data { get; private set; }

        public void Refresh(CubitData cubitData, int count)
        {
            Data = cubitData;
            _contentsObject.SetActive(Data != null && count>0);
            
            if (Data == null)
            {
                return;
            }
            
            _icon.color = cubitData.Color;
            RefreshCount(count);
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