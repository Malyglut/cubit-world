using Malyglut.CubitWorld.Data;
using Malyglut.CubitWorld.Utilties;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Malyglut.CubitWorld.UserInterface.Inventory
{
    public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerClickHandler
    {
        [SerializeField]
        private InventorySlotVisual _visual;

        [SerializeField]
        private GameObject _selection;

        [SerializeField]
        private GameObject _dragOverlay;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _slotClicked;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _slotDragBegin;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _slotDragEnd;

        [SerializeField, FoldoutGroup("Events")]
        private GameEvent _slotPointerEnter;

        public IPlaceableData Data { get; private set; }

        private void Awake()
        {
            Deselect();
            _dragOverlay.SetActive(false);
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
            _dragOverlay.SetActive(true);
            _slotDragBegin.Raise(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragOverlay.SetActive(false);
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