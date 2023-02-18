using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Cubit Data", menuName = "Cubit World/Cubit Data", order = 0)]
    public class CubitData : ScriptableObject
    {
        [SerializeField]
        private string _name = "NAME NOT SET";

        [SerializeField]
        private Color _color = Color.white;

        public Color Color => _color;
        public string Name => _name;
    }
}