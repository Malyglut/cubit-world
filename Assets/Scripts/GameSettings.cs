using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Game Settings", menuName = "Cubit World/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField]
        private float _cubitSize = 1f;

        [SerializeField, Range(1, 9)]
        private int _cubitsPerCubeAxis = 3;

        [SerializeField, Range(1, 9)]
        private int _hotbarSlotCount = 9;

        [SerializeField]
        private Vector2Int _inventoryDimensions = Vector2Int.one;

        public float CubitSize => _cubitSize;

        public float CubeSize => _cubitSize * _cubitsPerCubeAxis;

        public int CubitsPerCubeAxis => _cubitsPerCubeAxis;

        public float CubitCellSize => 1f / _cubitsPerCubeAxis;
        public int HotbarSlotCount => _hotbarSlotCount;
        public Vector2Int InventoryDimensions => _inventoryDimensions;

        public int InventoryCapacity => _hotbarSlotCount + _inventoryDimensions.x * _inventoryDimensions.y;
    }
}