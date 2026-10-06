using UnityEngine;
using Chromalit.Player;
using Chromalit.UI;

namespace Chromalit.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class KeyDoor : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite closedSprite;
        [SerializeField] private Sprite openedSprite;

        [Header("Settings")]
        [SerializeField] private float detectRadius = 2f;
        [SerializeField] private string uniqueId;

        [Header("Prompt")]
        [SerializeField] private InteractPrompt prompt;

        private SpriteRenderer _renderer;
        private Collider2D _collider;
        private bool _opened;
        private KeyHolder _nearbyPlayer;
        private Transform _player;

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = System.Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = false;

            if (closedSprite != null)
                _renderer.sprite = closedSprite;
        }

        private void Update()
        {
            if (_opened) return;
            if (HintPanelUI.BlocksInput) { if (prompt != null) prompt.Hide(); return; }

            UpdatePrompt();

            if (_nearbyPlayer != null && Input.GetKeyDown(KeyCode.E))
                TryOpen();
        }

        private void UpdatePrompt()
        {
            if (_player == null)
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null) _player = p.transform;
            }

            bool inRange = _player != null
                && _player.gameObject.activeInHierarchy
                && Vector2.Distance(transform.position, _player.position) <= detectRadius;

            if (!inRange)
            {
                _nearbyPlayer = null;
                if (prompt != null) prompt.Hide();
                return;
            }

            _nearbyPlayer = _player.GetComponent<KeyHolder>();
            if (prompt == null) return;

            if (_nearbyPlayer != null && _nearbyPlayer.KeyCount > 0)
                prompt.Show("E", "Buka Pintu", PromptState.Ready);
            else
                prompt.Show("", "Butuh Kunci", PromptState.Locked);
        }

        private void TryOpen()
        {
            if (_nearbyPlayer == null) return;

            if (_nearbyPlayer.UseKey())
            {
                if (prompt != null) prompt.Press();
                Open();
            }
        }

        private void Open()
        {
            _opened = true;
            _collider.enabled = false;
            if (prompt != null) prompt.Hide();

            if (openedSprite != null)
                _renderer.sprite = openedSprite;

            StartCoroutine(ShowLevelComplete());
        }

        private System.Collections.IEnumerator ShowLevelComplete()
        {
            yield return new WaitForSecondsRealtime(0.6f);

            LevelCompleteUI ui = FindFirstObjectByType<LevelCompleteUI>();
            if (ui != null) ui.Show();
            else Debug.LogWarning("LevelCompleteUI tidak ditemukan di scene!");
        }

        public void ResetDoor()
        {
            _opened = false;
            _collider.enabled = true;
            _collider.isTrigger = false;
            if (closedSprite != null) _renderer.sprite = closedSprite;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = UnityEngine.Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }

        public string UniqueId => uniqueId;
        public bool IsOpened => _opened;
    }
}