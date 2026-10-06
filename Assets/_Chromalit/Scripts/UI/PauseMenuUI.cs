using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chromalit.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private PanelAnimator pausePanel;
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private bool _isPaused;

        private void Start() => pausePanel.HideInstant();

        private void Update()
        {
            if (LevelCompleteUI.IsOpen || HintPanelUI.BlocksInput) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused) Resume();
                else Pause();
            }
        }

        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
            pausePanel.Show();
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            pausePanel.Hide();
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}