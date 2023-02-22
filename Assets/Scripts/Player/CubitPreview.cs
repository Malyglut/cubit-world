using UnityEngine;

namespace Malyglut.CubitWorld.Player
{
    public class CubitPreview : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        [SerializeField, Range(0f,1f)]
        private float _colorAlpha = .75f;

        public void UpdateVisual(Color color)
        {
            color.a = _colorAlpha;
            _renderer.sharedMaterial.color = color;
        }
    }
}