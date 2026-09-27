using UnityEngine;
using Chromalit.Player;

namespace Chromalit.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class IceBlock : MonoBehaviour
    {
        [SerializeField] private string uniqueId;
        [SerializeField] private float meltDuration = 0.5f;

        private Collider2D _collider;
        private SpriteRenderer _renderer;
        private bool _melted;

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = System.Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _renderer = GetComponent<SpriteRenderer>();
            _collider.isTrigger = false; // solid by default
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_melted) return;

            PlayerColorState colorState = collision.gameObject.GetComponent<PlayerColorState>();
            if (colorState == null) return;

            if (colorState.CanMeltIce())
            {
                StartCoroutine(Melt());
            }
        }

        private System.Collections.IEnumerator Melt()
        {
            _melted = true;
            
            // Animasi meleleh — fade out
            float elapsed = 0f;
            UnityEngine.Color startColor = _renderer.color;

            while (elapsed < meltDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / meltDuration);
                _renderer.color = new UnityEngine.Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            _collider.enabled = false;
            _renderer.enabled = false;
        }

        public void ResetBlock()
        {
            _melted = false;
            _collider.enabled = true;
            _collider.isTrigger = false;
            _renderer.enabled = true;
            UnityEngine.Color c = _renderer.color;
            _renderer.color = new UnityEngine.Color(c.r, c.g, c.b, 1f);
        }

        public string UniqueId => uniqueId;
        public bool IsMelted => _melted;
    }
}