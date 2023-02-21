using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Cubit World/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        private float _cubitSize = 1f;
        
        [SerializeField, Range(1,9)]
        private int _cubitsPerCubeAxis = 3;

        public float CubitSize => _cubitSize;

        public float CubeSize => _cubitSize * _cubitsPerCubeAxis;

        public int CubitsPerCubeAxis => _cubitsPerCubeAxis;

        public float CubitCellSize => 1f / _cubitsPerCubeAxis;
    }
}