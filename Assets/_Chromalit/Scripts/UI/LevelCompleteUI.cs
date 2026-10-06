using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Chromalit.UI
{
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private GameObject nextLevelButton;
        [SerializeField] private TextMeshProUGUI nextLevelButtonText;

        [Header("Level Berikutnya")]
        [Tooltip("Nama scene tujuan. Kosongkan kalau ini level terakhir.")]
        [SerializeField] private string nextSceneName = "Insanity";
        [SerializeField] private string nextLevelLabel = "LANJUT KE INSANITY";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        public static bool IsOpen { get; private set; }

        private void Awake()
        {
            IsOpen = false;
            if (panel != null) panel.SetActive(false);
        }

        public void Show()
        {
            IsOpen = true;
            panel.SetActive(true);
            Time.timeScale = 0f;

            if (titleText != null) titleText.text = "LEVEL SELESAI!";

            bool hasNext = !string.IsNullOrEmpty(nextSceneName);

            if (nextLevelButton != null) nextLevelButton.SetActive(hasNext);
            if (nextLevelButtonText != null) nextLevelButtonText.text = nextLevelLabel;

            if (messageText != null)
            {
                messageText.text = hasNext
                    ? "Siap lanjut ke tantangan berikutnya?"
                    : "Selamat! Kamu sudah menyelesaikan semua level.";
            }
        }

        public void LoadNextLevel()
        {
            Close();
            SceneManager.LoadScene(nextSceneName);
        }

        public void RestartLevel()
        {
            Close();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMainMenu()
        {
            Close();
            SceneManager.LoadScene(mainMenuSceneName);
        }

        private void Close()
        {
            IsOpen = false;
            Time.timeScale = 1f;
        }
    }
}