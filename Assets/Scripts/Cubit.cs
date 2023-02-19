using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Malyglut.CubitWorld
{
    public class Cubit : MonoBehaviour
    {
        [FormerlySerializedAs("_renderer"),SerializeField]
        private Renderer _cubitRenderer;
        
        [SerializeField]
        private Renderer _marbleRenderer;

        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private CubitData _initializationData;

        [SerializeField]
        private GameObject _visual;
        
        [SerializeField]
        private GameObject _collision;

        [SerializeField]
        private Animator _animator;
        
        [SerializeField]
        private AnimationClip _placement;

        public CubitData Data { get; private set; }
        public Cube Cube { get; private set; }

        public Mesh Mesh => _meshFilter.mesh;
        public Material Material => _cubitRenderer.sharedMaterial;

        private void Awake()
        {
            var cube = GetComponentInParent<Cube>();
            _animator.enabled = false;
            
            if (_initializationData != null && cube !=null)
            {
                Initialize(_initializationData, cube);
            }
        }

        [Button]
        public void Initialize(CubitData data, Cube cube)
        {
            if (data == null || cube == null)
            {
                Debug.LogError("Null data passed to Cubit.");
                return;
            }
            
            Data = data;
            Cube = cube;

            transform.SetParent(Cube.transform);
            
            name = $"{Data.Name} [{transform.localPosition}]";
            _cubitRenderer.material.color = Data.Color;
            _marbleRenderer.material.color = Data.Color;
        }

        public void DisableVisuals()
        {
            _visual.gameObject.SetActive(false);
        }

        [Button]
        public void PlayPlacementAnimation()
        {
            _animator.enabled = true;
            _animator.Play(_placement.name);
        }

        public void FinishPlayingAnimation()
        {
            _animator.enabled = false;
        }
    }
}