using Malyglut.CubitWorld.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Malyglut.CubitWorld.World
{
    public class Cubit : MonoBehaviour
    {

        [SerializeField]
        private CubitVisual _visual;

        public CubitData Data { get; private set; }
        public Cube Cube { get; private set; }

        public Mesh Mesh => _visual.Mesh;
        public Material Material => _visual.Material;

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

        //triggered from animation
        public void FinishPlayingAnimation()
        {
            _visual.DisableAnimator();
        }
    }
}