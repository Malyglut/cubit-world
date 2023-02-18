using System;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Cubit World/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        private const float CUBIT_PLACEMENT_TOLERANCE = 0.0000025f;
        
        [SerializeField]
        private float _cubitSize = 1f;
        
        [SerializeField]
        private float _cubeSize = 3f;

        public float CubitSize => _cubitSize;

        public float CubeMaxExtents
        {
            get
            {
                if (_cubeMaxExtents <= 0)
                {
                    _cubeMaxExtents = _cubitSize / _cubeSize;
                    _cubeMaxExtents += CUBIT_PLACEMENT_TOLERANCE;
                }

                return _cubeMaxExtents;
            }
        }

        public float CubeSize => _cubeSize;

        private float _cubeMaxExtents = float.MinValue;
    }
}