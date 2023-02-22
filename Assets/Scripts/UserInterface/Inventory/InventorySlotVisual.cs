using Malyglut.CubitWorld.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Malyglut.CubitWorld.UserInterface.Inventory
{
    public class InventorySlotVisual : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _count;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private GameObject _contentsObject;

        public void Refresh(IPlaceableData data, int count)
        {
            _contentsObject.SetActive(data != null);
            
            if (data == null)
            {
                return;
            }

            _icon.sprite = data.Icon;
            
            if(data is CubitData cubitData)
            {
                RefreshForCubit(cubitData, count);
            }
            else if (data is ShapeData shapeData)
            {
                RefreshForShape(shapeData);
            }
        }
        
        private void RefreshForShape(ShapeData shapeData)
        {
            _count.gameObject.SetActive(false);
            _icon.color = Color.white;
        }

        private void RefreshForCubit(CubitData cubitData, int count)
        {
            _icon.color = cubitData.Color;
            _count.gameObject.SetActive(true);
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
    }
}