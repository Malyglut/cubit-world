using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld.Data
{
    [CreateAssetMenu(fileName = "Cubit Data", menuName = "Cubit World/Cubit Data", order = 0)]
    public class CubitData : ScriptableObject, IPlaceableData
    {
        [SerializeField]
        private string _name = "NAME NOT SET";

        [SerializeField]
        private Color _color = Color.white;

        [FormerlySerializedAs("_icon"),SerializeField]
        private Sprite _inventoryIcon;

        public Color Color => _color;
        public string Name => _name;
        public Sprite Icon => _inventoryIcon;
    }
}