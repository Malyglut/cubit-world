using System.Collections.Generic;
using Malyglut.CubitWorld.World;
using UnityEngine;

namespace Malyglut.CubitWorld.Data
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