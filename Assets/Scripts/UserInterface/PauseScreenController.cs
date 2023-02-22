using Malyglut.CubitWorld.Utilties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Malyglut.CubitWorld.UserInterface
{
    public class PauseScreenController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _contents;
        
        [SerializeField]
        private Button _backToMenuButton;
        
        [SerializeField]
        private Button _quitButton;

        [SerializeField]
        private GameEvent _pauseScreenOpened;
        
        [SerializeField]
        private GameEvent _pauseScreenClosed;
        
        [SerializeField]
        private GameEvent _loadingStarted;

        private bool _isPaused;

        private void Awake()
        {
            _backToMenuButton.onClick.AddListener(BackToMenu);
            _quitButton.onClick.AddListener(Application.Quit);

            _contents.SetActive(false);
        }

        private void BackToMenu()
        {
            Time.timeScale = 1f;
            _loadingStarted.Raise();
            SceneManager.LoadScene(Scenes.MAIN_MENU_SCENE_IDX);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isPaused = !_isPaused;

                _contents.SetActive(_isPaused);

                if (_isPaused)
                {
                    _pauseScreenOpened.Raise();
                }
                else
                {
                    _pauseScreenClosed.Raise();
                }
            }
        }
    }
}