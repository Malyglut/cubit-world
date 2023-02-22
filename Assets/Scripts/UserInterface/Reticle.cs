using UnityEngine;
using UnityEngine.UI;

namespace Malyglut.CubitWorld.UserInterface
{
    public class Reticle : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        
        [SerializeField]
        private Color _activeColor = Color.white;
        
        [SerializeField]
        private Color _inactiveColor = Color.white;

        public void SetInactive()
        {
            _image.color = _inactiveColor;
        }

        public void SetActive()
        {
            _image.color = _activeColor;
        }
    }
}