using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld.Data
{
    public class ShapeData : IPlaceableData
    {
        public Dictionary<Vector3Int, CubitData> ShapeBlueprint = new();
        public Mesh Mesh;
        public Material[] Materials;
        public Sprite InventoryIcon;
        
        public Sprite Icon => InventoryIcon;
    }
}