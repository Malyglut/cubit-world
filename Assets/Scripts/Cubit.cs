using Sirenix.OdinInspector;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Cubit : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private CubitData _initializationData;

        [SerializeField]
        private GameObject _visual;
        
        [SerializeField]
        private GameObject _collision;

        public CubitData Data { get; private set; }

        public Mesh Mesh => _meshFilter.mesh;
        public Material Material => _renderer.sharedMaterial;

        private void Awake()
        {
            if (_initializationData != null)
            {
                Initialize(_initializationData);
            }
        }

        [Button]
        public void Initialize(CubitData data)
        {
            if (data == null)
            {
                Debug.LogError("Null data passed to Cubit.");
                return;
            }
            
            Data = data;

            name = $"{Data.Name} [{transform.localPosition}]";
            _renderer.material.color = Data.Color;
        }

        public void DisableVisuals()
        {
            _visual.gameObject.SetActive(false);
        }
    }
}