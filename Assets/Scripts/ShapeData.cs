using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    [CreateAssetMenu(fileName = "Shape Data", menuName = "Cubit World/Shape Data", order = 0)]
    public class ShapeData : ScriptableObject, IPlaceableData
    {
        public Dictionary<Vector3Int, Cubit> ShapeBlueprint = new();
        public Mesh Mesh;
        public Material[] Materials;
        public Sprite InventoryIcon;
        
        public Sprite Icon => InventoryIcon;
    }
}