using UnityEngine;

namespace Chromalit.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject levelSelectPanel;

        private void Start()
        {
            ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            mainPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
        }

        public void ShowLevelSelect()
        {
            mainPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
        }
    }
}