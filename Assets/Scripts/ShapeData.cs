using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class ShapeData : IPlaceableData
    {
        public Dictionary<Vector3Int, Cubit> ShapeBlueprint = new();
        public Mesh Mesh;
        public Material[] Materials;
        public Sprite InventoryIcon;
        
        public Sprite Icon => InventoryIcon;
    }
}