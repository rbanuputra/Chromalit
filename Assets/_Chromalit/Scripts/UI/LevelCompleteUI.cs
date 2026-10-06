using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

namespace Chromalit.UI
{
    public class LevelCompleteUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private PanelAnimator panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private GameObject nextLevelButton;
        [SerializeField] private TextMeshProUGUI nextLevelButtonText;
        [SerializeField] private GameObject restartButton;

        [Header("Level Berikutnya")]
        [Tooltip("Nama scene tujuan. Kosongkan kalau ini level terakhir.")]
        [SerializeField] private string nextSceneName = "Insanity";
        [SerializeField] private string nextLevelLabel = "LANJUT KE INSANITY";
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        public static bool IsOpen { get; private set; }

        private void Awake()
        {
            IsOpen = false;
            if (panel != null) panel.HideInstant();
        }

        public void Show()
        {
            IsOpen = true;
            bool hasNext = !string.IsNullOrEmpty(nextSceneName);

            if (titleText != null) titleText.text = "LEVEL SELESAI!";
            if (nextLevelButton != null) nextLevelButton.SetActive(hasNext);
            if (nextLevelButtonText != null) nextLevelButtonText.text = nextLevelLabel;

            if (messageText != null)
            {
                messageText.text = hasNext
                    ? "Siap lanjut ke tantangan berikutnya?"
                    : "Selamat! Semua level sudah kamu taklukkan.";
            }

            if (timeText != null)
            {
                float t = Time.timeSinceLevelLoad;
                timeText.text = $"Waktu  {(int)(t / 60f):00}:{(int)(t % 60f):00}";
            }

            Time.timeScale = 0f;
            panel.Show();

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(hasNext ? nextLevelButton : restartButton);
            }
        }

        public void LoadNextLevel() { Close(); SceneManager.LoadScene(nextSceneName); }
        public void RestartLevel() { Close(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
        public void GoToMainMenu() { Close(); SceneManager.LoadScene(mainMenuSceneName); }

        private void Close()
        {
            IsOpen = false;
            Time.timeScale = 1f;
        }
    }
}