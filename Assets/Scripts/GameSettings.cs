using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Cubit World/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        private const float CUBIT_PLACEMENT_TOLERANCE = 0.0000025f;
        
        [SerializeField]
        private float _cubitSize = 1f;
        
        [SerializeField, Range(1,9)]
        private int _cubitsPerCubeAxis = 3;

        public float CubitSize => _cubitSize;

        public float CubeSize => _cubitSize * _cubitsPerCubeAxis;

        public int CubitsPerCubeAxis => _cubitsPerCubeAxis;

        public float CubeMaxExtents
        {
            get
            {
                if (_cubeMaxExtents <= 0)
                {
                    _cubeMaxExtents = _cubitSize / CubeSize;
                    _cubeMaxExtents += CUBIT_PLACEMENT_TOLERANCE;
                }

                return _cubeMaxExtents;
            }
        }
        
        private float _cubeMaxExtents = float.MinValue;
    }
}