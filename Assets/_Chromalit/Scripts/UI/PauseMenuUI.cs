using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chromalit.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;

        private bool _isPaused;

        private void Start()
        {
            pausePanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        public void Pause()
        {
            _isPaused = true;
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void Resume()
        {
            _isPaused = false;
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}