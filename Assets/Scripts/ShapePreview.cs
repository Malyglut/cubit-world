using System;
using System.Collections.Generic;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class ShapePreview : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _renderer;

        [SerializeField]
        private MeshFilter _filter;

        [SerializeField]
        private GameSettings _gameSettings;

        [SerializeField, Range(0f,1f)]
        private float _previewAlpha = .3f;

        private void Awake()
        {
            transform.localScale = Vector3.one * _gameSettings.CubeSize;
        }

        public void UpdateVisual(Mesh mesh, Material[] materials)
        {
            _filter.mesh = mesh;

            var alphaMaterials = new List<Material>(materials);

            foreach (var material in alphaMaterials)
            {
                var color = material.color;
                color.a = _previewAlpha;
                material.color = color;
            }
            
            _renderer.materials = alphaMaterials.ToArray();
        }
    }
}