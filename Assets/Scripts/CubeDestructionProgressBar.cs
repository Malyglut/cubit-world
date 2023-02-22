using System;
using UnityEngine;
using UnityEngine.UI;

namespace Malyglut.CubitWorld
{
    public class CubeDestructionProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Image _fill;

        [SerializeField]
        private GameObject _barObject;
        
        [SerializeField]
        private GameEvent _cubeDestructionStarted;
        
        [SerializeField]
        private GameEvent _cubeDestructionEnded;

        private Cube _targetCube;

        private void Awake()
        {
            _barObject.SetActive(false);
            
            _cubeDestructionStarted.Subscribe(ShowProgressBar);
            _cubeDestructionEnded.Subscribe(HideProgressBar);
        }

        private void ShowProgressBar(object cubeObject)
        {
            _targetCube = (Cube)cubeObject;
            _fill.fillAmount = 0f;
            _barObject.SetActive(true);
        }

        private void HideProgressBar()
        {
            _targetCube = null;
            _barObject.SetActive(false);
        }

        private void Update()
        {
            if (_targetCube == null)
            {
                return;
            }

            _fill.fillAmount = _targetCube.DestructionProgress;
        }
    }
}