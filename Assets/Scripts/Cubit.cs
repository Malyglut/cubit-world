using Sirenix.OdinInspector;
using UnityEngine;

namespace Malyglut.CubitWorld
{
    public class Cubit : MonoBehaviour
    {
        [SerializeField]
        private CubitData _initializationData;

        [SerializeField]
        private CubitVisual _visual;
        
        [SerializeField]
        private GameObject _collision;

        public CubitData Data { get; private set; }
        public Cube Cube { get; private set; }

        public Mesh Mesh => _visual.Mesh;
        public Material Material => _visual.Material;

        private void Awake()
        {
            var cube = GetComponentInParent<Cube>();
            
            if (_initializationData != null && cube !=null)
            {
                Initialize(_initializationData, cube);
            }
        }
        
        public void Initialize(CubitData data, Cube cube)
        {
            if (data == null)
            {
                Debug.LogError("Null data passed to Cubit.");
                return;
            }
            
            Data = data;
            
            if(cube!=null)
            {
                Cube = cube;
                //takes care of floating point errors after changing parent
                _visual.UpdateChildScale();
                transform.SetParent(Cube.transform, true);
            }
            
            name = $"{Data.Name} [{transform.localPosition}]";
            _visual.UpdateColor(Data.Color);
            
        }

        public void DisableVisuals()
        {
            _visual.gameObject.SetActive(false);
        }

        [Button]
        public void PlayPlacementAnimation()
        {
            _visual.PlayPlacementAnimation();
        }

        public void FinishPlayingAnimation()
        {
            _visual.DisableAnimator();
        }
    }
}