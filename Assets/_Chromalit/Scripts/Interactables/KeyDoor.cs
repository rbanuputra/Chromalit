using UnityEngine;
using Chromalit.Player;

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

        private SpriteRenderer _renderer;
        private Collider2D _collider;
        private bool _opened;
        private bool _playerInRange;
        private KeyHolder _nearbyPlayer;

        // UI Prompt
        private GameObject _promptUI;
        private TMPro.TextMeshPro _promptText;

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = System.Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = false; // solid wall

            if (closedSprite != null)
                _renderer.sprite = closedSprite;

            CreatePromptUI();
        }

        private void CreatePromptUI()
        {
            _promptUI = new GameObject("DoorPrompt");
            _promptUI.transform.SetParent(transform);
            _promptUI.transform.localPosition = new Vector3(0f, 2.5f, 0f);

            _promptText = _promptUI.AddComponent<TMPro.TextMeshPro>();
            _promptText.text = "Tekan E (Butuh Kunci)";
            _promptText.fontSize = 3f;
            _promptText.alignment = TMPro.TextAlignmentOptions.Center;
            _promptText.sortingOrder = 10;

            _promptUI.SetActive(false);
        }

        private void Update()
        {
            if (_opened) return;

            CheckPlayerInRange();

            if (_playerInRange && _nearbyPlayer != null && Input.GetKeyDown(KeyCode.E))
            {
                TryOpen();
            }
        }

        private void CheckPlayerInRange()
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null || !playerObj.activeInHierarchy)
            {
                _playerInRange = false;
                _promptUI.SetActive(false);
                return;
            }

            float dist = Vector2.Distance(transform.position, playerObj.transform.position);
            _playerInRange = dist <= detectRadius;

            if (_playerInRange)
            {
                _nearbyPlayer = playerObj.GetComponent<KeyHolder>();

                if (_nearbyPlayer != null && _nearbyPlayer.KeyCount > 0)
                {
                    _promptText.text = "Tekan E (Buka Pintu)";
                    _promptText.color = UnityEngine.Color.green;
                }
                else
                {
                    _promptText.text = "Butuh Kunci!";
                    _promptText.color = UnityEngine.Color.red;
                }

                _promptUI.SetActive(true);
            }
            else
            {
                _nearbyPlayer = null;
                _promptUI.SetActive(false);
            }
        }

        private void TryOpen()
        {
            if (_nearbyPlayer == null) return;

            if (_nearbyPlayer.UseKey())
            {
                Open();
            }
            else
            {
                Debug.Log("Butuh kunci untuk membuka pintu!");
            }
        }

        private void Open()
        {
            _opened = true;
            _collider.enabled = false;
            _promptUI.SetActive(false);

            if (openedSprite != null)
                _renderer.sprite = openedSprite;

            Debug.Log("Pintu terbuka!");
        }

        public void ResetDoor()
        {
            _opened = false;
            _collider.enabled = true;
            _collider.isTrigger = false;

            if (closedSprite != null)
                _renderer.sprite = closedSprite;
        }

        // Gizmo radius di Scene view
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = UnityEngine.Color.cyan;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }

        public string UniqueId => uniqueId;
        public bool IsOpened => _opened;
    }
}