using System.Collections;
using UnityEngine;

namespace Chromalit.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private PanelAnimator mainPanel;
        [SerializeField] private PanelAnimator levelSelectPanel;

        private bool _inLevelSelect;

        private void Start()
        {
            levelSelectPanel.HideInstant();
            mainPanel.Show();
        }

        private void Update()
        {
            if (_inLevelSelect && Input.GetKeyDown(KeyCode.Escape))
                ShowMainMenu();
        }

        public void ShowMainMenu()
        {
            _inLevelSelect = false;
            StartCoroutine(Switch(levelSelectPanel, mainPanel));
        }

        public void ShowLevelSelect()
        {
            _inLevelSelect = true;
            StartCoroutine(Switch(mainPanel, levelSelectPanel));
        }

        private IEnumerator Switch(PanelAnimator from, PanelAnimator to)
        {
            from.Hide();
            yield return new WaitForSecondsRealtime(0.12f);
            to.Show();
        }
    }
}