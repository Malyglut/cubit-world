using UnityEngine;

namespace Malyglut.CubitWorld.World
{
    public class CubitVisual : MonoBehaviour
    {
        [SerializeField]
        private Renderer _cubitRenderer;
        
        [SerializeField]
        private Renderer _marbleRenderer;

        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private Animator _animator;
        
        [SerializeField]
        private AnimationClip _placement;

        public Mesh Mesh => _meshFilter.mesh;
        public Material Material => _cubitRenderer.sharedMaterial;

        private void Awake()
        {
            DisableAnimator();
        }

        public void UpdateColor(Color color)
        {
            _cubitRenderer.material.color = color;
            _marbleRenderer.material.color = color;
        }
        
        public void DisableAnimator()
        {
            _animator.enabled = false;
        }
        
        public void PlayPlacementAnimation()
        {
            _animator.enabled = true;
            _animator.Play(_placement.name);
        }

        public void UpdateChildScale()
        {
            _cubitRenderer.transform.localScale = Vector3.one;
            _marbleRenderer.transform.localScale = Vector3.one;
        }
    }
}