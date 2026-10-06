using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chromalit.UI
{
    public enum PromptState { Ready, Locked }

    [RequireComponent(typeof(CanvasGroup))]
    public class InteractPrompt : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image panelImage;
        [SerializeField] private GameObject keyCap;
        [SerializeField] private Image keyCapImage;
        [SerializeField] private TextMeshProUGUI keyText;
        [SerializeField] private TextMeshProUGUI labelText;

        [Header("Colors")]
        [SerializeField] private UnityEngine.Color panelColor = new UnityEngine.Color(0.08f, 0.08f, 0.14f, 0.88f);
        [SerializeField] private UnityEngine.Color readyAccent = new UnityEngine.Color(0.95f, 0.77f, 0.06f);
        [SerializeField] private UnityEngine.Color lockedAccent = new UnityEngine.Color(0.91f, 0.30f, 0.24f);
        [SerializeField] private UnityEngine.Color labelColor = UnityEngine.Color.white;
        [SerializeField] private UnityEngine.Color keyTextColor = new UnityEngine.Color(0.1f, 0.1f, 0.15f);

        [Header("Animation")]
        [SerializeField] private float fadeSpeed = 8f;
        [SerializeField] private float bobAmplitude = 0.06f;
        [SerializeField] private float bobSpeed = 3f;
        [SerializeField] private float popStrength = 0.2f;

        private CanvasGroup _group;
        private Vector3 _baseLocalPos;
        private Vector3 _baseScale;
        private float _targetAlpha;
        private float _pop;
        private PromptState _state;
        private bool _visible;

        private void Awake()
        {
            _group = GetComponent<CanvasGroup>();
            _group.alpha = 0f;
            _group.interactable = false;
            _group.blocksRaycasts = false;
            _baseLocalPos = transform.localPosition;
            _baseScale = transform.localScale;

            if (panelImage != null) panelImage.color = panelColor;
            if (keyText != null) keyText.color = keyTextColor;
        }

        /// <summary>Tampilkan prompt. key kosong = tanpa keycap.</summary>
        public void Show(string key, string label, PromptState state = PromptState.Ready)
        {
            if (!_visible) _pop = 1f; // efek pop saat baru muncul
            _visible = true;
            _targetAlpha = 1f;

            bool hasKey = !string.IsNullOrEmpty(key);
            if (keyCap != null) keyCap.SetActive(hasKey);
            if (keyText != null) keyText.text = key;
            if (labelText != null) labelText.text = label;

            SetState(state);
        }

        public void Hide()
        {
            _visible = false;
            _targetAlpha = 0f;
        }

        /// <summary>Efek pop kecil saat tombol ditekan.</summary>
        public void Press()
        {
            _pop = 1f;
        }

        private void SetState(PromptState state)
        {
            _state = state;
            UnityEngine.Color accent = state == PromptState.Ready ? readyAccent : lockedAccent;

            if (keyCapImage != null) keyCapImage.color = accent;
            if (labelText != null) labelText.color = state == PromptState.Ready ? labelColor : accent;
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;
            float t = Time.unscaledTime;

            // Fade
            _group.alpha = Mathf.MoveTowards(_group.alpha, _targetAlpha, fadeSpeed * dt);

            // Bob naik-turun
            float bob = Mathf.Sin(t * bobSpeed) * bobAmplitude;
            transform.localPosition = _baseLocalPos + Vector3.up * bob;

            // Pop + sedikit membesar seiring fade-in
            _pop = Mathf.MoveTowards(_pop, 0f, dt * 4f);
            float appear = Mathf.Lerp(0.85f, 1f, _group.alpha);
            transform.localScale = _baseScale * appear * (1f + popStrength * _pop);

            // Keycap berdenyut halus saat siap ditekan
            if (keyCap != null && keyCap.activeSelf)
            {
                float pulse = _state == PromptState.Ready ? 1f + 0.06f * Mathf.Sin(t * 6f) : 1f;
                keyCap.transform.localScale = Vector3.one * pulse;
            }
        }
    }
}