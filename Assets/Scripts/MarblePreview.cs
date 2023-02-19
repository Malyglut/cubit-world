using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class MarblePreview : MonoBehaviour
    {
        [SerializeField]
        private GameObject _previewObject;
        
        [SerializeField]
        private Renderer _renderer;
        
        [SerializeField]
        private GameEvent _hotbarSelection;

        private void Awake()
        {
            _hotbarSelection.Subscribe(UpdatePreview);
        }

        private void UpdatePreview(object cubitDataObject)
        {
            var cubitData = (CubitData)cubitDataObject;

            _previewObject.SetActive(cubitData != null);

            if (cubitData == null)
            {
                return;
            }
            
            _renderer.material.color = cubitData.Color;
        }
    }
}