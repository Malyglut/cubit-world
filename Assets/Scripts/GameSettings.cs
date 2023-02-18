using System;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Cubit World/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
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
                }

                return _cubeMaxExtents;
            }
        }

        public float CubeSize => _cubeSize;

        private float _cubeMaxExtents = float.MinValue;
    }
}