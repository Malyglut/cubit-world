using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Malyglut.CubitWorld
{
    public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        private InventorySlotVisual _visual;

        [SerializeField]
        private GameEvent _slotClicked;
        
        [SerializeField]
        private GameEvent _slotDragBegin;
        
        [SerializeField]
        private GameEvent _slotDragEnd;
        
        [SerializeField]
        private GameEvent _slotPointerEnter;

        [SerializeField]
        private GameObject _selection;

        public IPlaceableData Data { get; private set; }

        private void Awake()
        {
            Deselect();
        }

        public void Refresh(IPlaceableData data, int count)
        {
            Data = data;
            _visual.Refresh(Data, count);
        }

        public void Select()
        {
            _selection.SetActive(true);
        }

        public void Deselect()
        {
            _selection.SetActive(false);
        }

        public void RefreshCount(int amount)
        {
            _visual.RefreshCount(amount);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _slotDragBegin.Raise(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _slotDragEnd.Raise(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
            //required for OnBeginDrag and OnEndDrag to work
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _slotPointerEnter.Raise(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _slotClicked.Raise(this);
        }
    }
}