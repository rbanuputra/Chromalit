using UnityEngine;
using UnityEngine.EventSystems;

namespace Chromalit.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform window;       // kotak panel yang di-scale
        [SerializeField] private GameObject firstSelected;   // tombol yang terpilih pertama
        [SerializeField] private float showDuration = 0.3f;
        [SerializeField] private float hideDuration = 0.15f;
        [SerializeField] private float buttonStagger = 0.06f;

        private CanvasGroup _group;
        private float _t;
        private bool _showing;
        private bool _hiding;

        private CanvasGroup Group => _group != null ? _group : (_group = GetComponent<CanvasGroup>());

        public void Show()
        {
            gameObject.SetActive(true);
            _showing = true;
            _hiding = false;
            _t = 0f;

            Group.alpha = 0f;
            Group.interactable = true;
            Group.blocksRaycasts = true;
            if (window != null) window.localScale = Vector3.one * 0.9f;

            MenuButton[] buttons = GetComponentsInChildren<MenuButton>(true);
            for (int i = 0; i < buttons.Length; i++)
                buttons[i].PlayIntro(showDuration * 0.5f + i * buttonStagger);

            if (EventSystem.current != null && firstSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelected);
            }
        }

        public void Hide()
        {
            if (!gameObject.activeSelf) return;
            _hiding = true;
            _showing = false;
            _t = 0f;
            Group.interactable = false;
            Group.blocksRaycasts = false;
        }

        public void HideInstant()
        {
            _showing = false;
            _hiding = false;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;

            if (_showing)
            {
                _t += dt;
                float p = Mathf.Clamp01(_t / showDuration);
                Group.alpha = p;
                if (window != null)
                    window.localScale = Vector3.one * Mathf.LerpUnclamped(0.9f, 1f, EaseOutBack(p));
                if (p >= 1f) _showing = false;
            }
            else if (_hiding)
            {
                _t += dt;
                float p = Mathf.Clamp01(_t / hideDuration);
                Group.alpha = 1f - p;
                if (window != null)
                    window.localScale = Vector3.one * Mathf.Lerp(1f, 0.95f, p);
                if (p >= 1f)
                {
                    _hiding = false;
                    gameObject.SetActive(false);
                }
            }
        }

        private static float EaseOutBack(float x)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
        }
    }
}