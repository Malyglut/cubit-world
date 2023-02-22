using Malyglut.CubitWorld.Utilties;
using UnityEngine;

namespace Malyglut.CubitWorld.UserInterface
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject _contents;
        
        [SerializeField]
        private GameEvent _loadingStarted;
        
        [SerializeField]
        private GameEvent _loadingFinished;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
            _loadingStarted.Subscribe(Show);
            _loadingFinished.Subscribe(Hide);
        }

        private void Hide()
        {
            _contents.gameObject.SetActive(false);
        }

        private void Show()
        {
            _contents.gameObject.SetActive(true);
        }
    }
}