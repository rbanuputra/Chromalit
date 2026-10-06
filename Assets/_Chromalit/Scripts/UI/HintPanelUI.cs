using UnityEngine;
using TMPro;

namespace Chromalit.UI
{
    public class HintPanelUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject panel;          // overlay gelap (root panel)
        [SerializeField] private RectTransform scroll;      // gambar gulungan, untuk animasi
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private TextMeshProUGUI closeHintText;

        [Header("Settings")]
        [SerializeField] private bool pauseGame = true;
        [SerializeField] private float popDuration = 0.25f;

        public static bool IsOpen { get; private set; }

        // Mencegah tombol yang sama (E/Esc) langsung memicu aksi lain di frame penutupan
        private static int _closedFrame = -1;
        public static bool BlocksInput => IsOpen || Time.frameCount == _closedFrame;

        private int _openedFrame;
        private float _anim;

        private void Awake()
        {
            IsOpen = false;
            if (panel != null) panel.SetActive(false);
        }

        public void Open(string title, string body)
        {
            if (titleText != null) titleText.text = title;
            if (bodyText != null) bodyText.text = body;
            if (closeHintText != null) closeHintText.text = "Tekan E untuk menutup";

            panel.SetActive(true);
            IsOpen = true;
            _openedFrame = Time.frameCount;
            _anim = 0f;

            if (pauseGame) Time.timeScale = 0f;
        }

        public void Close()
        {
            IsOpen = false;
            _closedFrame = Time.frameCount;
            panel.SetActive(false);

            if (pauseGame) Time.timeScale = 1f;
        }

        private void Update()
        {
            if (!IsOpen) return;

            // Animasi pop-in (pakai unscaled time karena game di-pause)
            if (scroll != null && _anim < 1f)
            {
                _anim = Mathf.Min(1f, _anim + Time.unscaledDeltaTime / popDuration);
                float s = Mathf.LerpUnclamped(0.6f, 1f, EaseOutBack(_anim));
                scroll.localScale = Vector3.one * s;
            }

            // Jangan menutup di frame yang sama saat dibuka
            if (Time.frameCount == _openedFrame) return;

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
    }
}