using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class PlayerInventory : MonoBehaviour
    {
        [FormerlySerializedAs("_marbleAddedToInventory"), SerializeField]
        private GameEvent _marbleInventoryUpdate;

        private readonly Dictionary<CubitData, int> _marbles = new();

        public void Add(CubitData cubitData, int amount)
        {
            if (!_marbles.ContainsKey(cubitData) || amount <= 0)
            {
                _marbles.Add(cubitData, 0);
            }

            _marbles[cubitData] += amount;

            Debug.Log($"Player awarded {amount} x {cubitData.Name} marble");

            _marbleInventoryUpdate.Raise(new MarbleCount { Data = cubitData, Count = _marbles[cubitData] });
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
    }
}