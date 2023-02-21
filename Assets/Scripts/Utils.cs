using UnityEngine;

namespace Malyglut.CubitWorld
{
    public static class Utils
    {
        public static Vector3Int GridIndex(Vector3 position, float cellSize)
        {
            var x = Mathf.RoundToInt(position.x / cellSize);
            var y = Mathf.RoundToInt(position.y / cellSize);
            var z = Mathf.RoundToInt(position.z / cellSize);

            return new Vector3Int(x, y, z);
        }
    }
}