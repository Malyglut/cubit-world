using Malyglut.CubitWorld.Utilties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Malyglut.CubitWorld.UserInterface
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField]
        private Button _playButton;
        
        [SerializeField]
        private Button _quitButton;

        [SerializeField]
        private GameEvent _loadingStarted;
        
        [SerializeField]
        private GameEvent _loadingFinished;

        private void Awake()
        {
            _playButton.onClick.AddListener(StartGame);
            _quitButton.onClick.AddListener(Application.Quit);
            
            _loadingFinished.Raise();
        }

        private void StartGame()
        {
            _loadingStarted.Raise();
            SceneManager.LoadScene(1);
        }
    }
}