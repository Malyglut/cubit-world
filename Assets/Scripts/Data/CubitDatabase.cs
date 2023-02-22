using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld.Data
{
    [CreateAssetMenu(fileName = "Cubit Database", menuName = "Cubit World/Cubit Database", order = 0)]
    public class CubitDatabase : ScriptableObject
    {
        [SerializeField]
        private List<CubitData> _records = new();

        public CubitData this[int idx]
        {
            get
            {
                if (idx < 0 || idx >= _records.Count)
                {
                    return null;
                }

                return _records[idx];
            }
        }

        public CubitData RandomCubitData()
        {
            return _records[Random.Range(0, _records.Count)];
        }
    }
}