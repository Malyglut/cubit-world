using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class PlayerInventory
    {
        private Dictionary<CubitData, int> _cubits = new();

        public void Add(CubitData cubitData, int amount)
        {
            if (!_cubits.ContainsKey(cubitData))
            {
                _cubits.Add(cubitData, 0);
            }

            _cubits[cubitData] += amount;
            
            Debug.Log($"Player awarded {amount} x {cubitData.Name}");
        }
    }
}