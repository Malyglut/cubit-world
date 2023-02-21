using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;

namespace Malyglut.CubitWorld
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField]
        private GameEvent _marbleInventoryUpdate;

        [FormerlySerializedAs("_shapeInventoryAdded"),FormerlySerializedAs("_shapeInventoryUpdate"), SerializeField]
        private GameEvent _shapeAddedToInventory;

        [FormerlySerializedAs("_shapeInventoryRemoved"),SerializeField]
        private GameEvent _shapeRemovedFromInventory;

        [SerializeField]
        private GameSettings _gameSettings;

        private readonly Dictionary<CubitData, int> _marbles = new();
        private readonly List<ShapeData> _shapes = new();

        public void AddMarbles(CubitData cubitData, int amount)
        {
            if (!HasSpaceInInventory())
            {
                return;
            }
            
            if (!_marbles.ContainsKey(cubitData) || amount <= 0)
            {
                _marbles.Add(cubitData, 0);
            }

            _marbles[cubitData] += amount;

            _marbleInventoryUpdate.Raise(new MarbleCount { Data = cubitData, Count = _marbles[cubitData] });
        }

        private bool HasSpaceInInventory()
        {
            var marbleSlots = _marbles.Count(marble => marble.Value > 0);
            var shapeSlots = _shapes.Count;

            var hasSpace = marbleSlots + shapeSlots < _gameSettings.InventoryCapacity;

            if (!hasSpace)
            {
                Debug.Log("Player has no more space in inventory.");
            }
            
            return hasSpace;
        }

        public void SubtractMarbles(CubitData cubitData, int amount)
        {
            if (!_marbles.ContainsKey(cubitData) || amount <= 0)
            {
                return;
            }

            _marbles[cubitData] -= amount;
            _marbleInventoryUpdate.Raise(new MarbleCount { Data = cubitData, Count = _marbles[cubitData] });
        }

        public void AddShape(ShapeData shapeData)
        {
            _shapes.Add(shapeData);
            _shapeAddedToInventory.Raise(shapeData);
        }

        public void RemoveShape(ShapeData shapeData)
        {
            _shapes.Remove(shapeData);
            _shapeRemovedFromInventory.Raise(shapeData);
        }

        public bool HasShape(ShapeData shapeData)
        {
            return _shapes.Contains(shapeData);
        }

        public int MarbleCount(CubitData cubitData)
        {
            return _marbles.ContainsKey(cubitData) ? _marbles[cubitData] : 0;
        }
    }
}