using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Chromalit.UI
{
    [RequireComponent(typeof(Button))]
    public class MenuButton : MonoBehaviour,
        IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler,
        ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        [SerializeField] private Image background;
        [SerializeField] private Image accentBar;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject arrow;

        [Header("Colors")]
        [SerializeField] private UnityEngine.Color accentColor = new UnityEngine.Color(0.91f, 0.30f, 0.24f);
        [SerializeField] private UnityEngine.Color baseColor = new UnityEngine.Color(0.14f, 0.14f, 0.23f, 1f);
        [SerializeField, Range(0f, 1f)] private float hoverTint = 0.3f;

        [Header("Animation")]
        [SerializeField] private float hoverScale = 1.05f;
        [SerializeField] private float pressScale = 0.95f;
        [SerializeField] private float contentShift = 12f;
        [SerializeField] private float speed = 14f;

        [Header("Audio (opsional)")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip hoverClip;
        [SerializeField] private AudioClip clickClip;

        private Button _button;
        private CanvasGroup _group;
        private RectTransform _accentRect;
        private RectTransform _buttonRect;
        private float _accentBaseWidth;
        private Vector2 _contentBase;
        private bool _selected;
        private bool _pressed;
        private float _introDelay;
        private bool _introPlaying;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.transition = Selectable.Transition.None;
            _button.onClick.AddListener(() => Play(clickClip));

            _group = GetComponent<CanvasGroup>();
            if (_group == null) _group = gameObject.AddComponent<CanvasGroup>();

            if (content != null) _contentBase = content.anchoredPosition;
            if (accentBar != null)
            {
                accentBar.color = accentColor;
                _accentRect = accentBar.rectTransform;
                _accentBaseWidth = _accentRect.sizeDelta.x;
            }
            _buttonRect = (RectTransform)transform;
            if (background != null) background.color = baseColor;
            if (arrow != null) arrow.SetActive(false);
        }

        private void OnEnable()
        {
            _pressed = false;
            transform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            _selected = false;
            _pressed = false;
            if (arrow != null) arrow.SetActive(false);
            if (content != null) content.anchoredPosition = _contentBase;
            if (background != null) background.color = baseColor;
        }

        /// <summary>Dipanggil PanelAnimator supaya tombol muncul berurutan.</summary>
        public void PlayIntro(float delay)
        {
            _introDelay = delay;
            _introPlaying = true;
            _group.alpha = 0f;
            transform.localScale = Vector3.one * 0.85f;
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;

            if (_introPlaying)
            {
                if (_introDelay > 0f) { _introDelay -= dt; return; }
                _group.alpha = Mathf.MoveTowards(_group.alpha, 1f, dt * 6f);
                if (_group.alpha >= 1f) _introPlaying = false;
            }

            bool active = _selected && _button.interactable;
            float k = 1f - Mathf.Exp(-speed * dt);

            float targetScale = _pressed ? pressScale : (active ? hoverScale : 1f);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, k);

            if (background != null)
            {
                UnityEngine.Color target = active
                    ? UnityEngine.Color.Lerp(baseColor, accentColor, hoverTint)
                    : baseColor;
                background.color = UnityEngine.Color.Lerp(background.color, target, k);
            }

            if (_accentRect != null)
            {
                float targetWidth = active ? _buttonRect.rect.width : _accentBaseWidth;
                Vector2 sd = _accentRect.sizeDelta;
                sd.x = Mathf.Lerp(sd.x, targetWidth, k);
                _accentRect.sizeDelta = sd;
            }

            if (content != null)
            {
                Vector2 targetPos = _contentBase + (active ? Vector2.right * contentShift : Vector2.zero);
                content.anchoredPosition = Vector2.Lerp(content.anchoredPosition, targetPos, k);
            }

            if (arrow != null && arrow.activeSelf != active) arrow.SetActive(active);
        }

        public void OnPointerEnter(PointerEventData e)
        {
            // Hover mouse = select, jadi mouse dan keyboard selalu sinkron
            if (_button.interactable && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerDown(PointerEventData e) => _pressed = true;
        public void OnPointerUp(PointerEventData e) => _pressed = false;

        public void OnSelect(BaseEventData e)
        {
            _selected = true;
            Play(hoverClip);
        }

        public void OnDeselect(BaseEventData e)
        {
            _selected = false;
            _pressed = false;
        }

        private void Play(AudioClip clip)
        {
            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        }
    }
}