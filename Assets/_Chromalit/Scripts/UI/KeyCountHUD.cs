using UnityEngine;
using TMPro;
using Chromalit.Player;

namespace Chromalit.UI
{
    public class KeyCountHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private KeyHolder keyHolder;

        private void OnEnable()
        {
            if (keyHolder != null)
                keyHolder.OnKeyCountChanged += UpdateText;
        }

        private void OnDisable()
        {
            if (keyHolder != null)
                keyHolder.OnKeyCountChanged -= UpdateText;
        }

        private void Start()
        {
            UpdateText(keyHolder != null ? keyHolder.KeyCount : 0);
        }

        private void UpdateText(int count)
        {
            if (keyText != null)
                keyText.text = $"x{count}";
        }
    }
}