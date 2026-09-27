using UnityEngine;
using TMPro;
using Chromalit.Player;

namespace Chromalit.Interactables
{
    public class Drone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float flySpeed = 5f;
        [SerializeField] private float detectRadius = 3f;
        [SerializeField] private bool requireYellow = true;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer droneRenderer;

        private bool _isControlled;
        private bool _playerInRange;
        private GameObject _player;
        private PlayerColorState _playerColorState;
        private PlayerController _playerController;
        private Rigidbody2D _playerRb;
        private Rigidbody2D _droneRb;

        // UI Prompt
        private GameObject _promptUI;
        private TextMeshPro _promptText;

        private void Awake()
        {
            _droneRb = GetComponent<Rigidbody2D>();

            if (droneRenderer == null)
                droneRenderer = GetComponentInChildren<SpriteRenderer>();

            CreatePromptUI();
        }

        private void CreatePromptUI()
        {
            _promptUI = new GameObject("DronePrompt");
            _promptUI.transform.SetParent(transform);
            _promptUI.transform.localPosition = new Vector3(0f, 2f, 0f);

            _promptText = _promptUI.AddComponent<TextMeshPro>();
            _promptText.text = "Tekan E";
            _promptText.fontSize = 3f;
            _promptText.alignment = TextAlignmentOptions.Center;
            _promptText.sortingOrder = 10;

            _promptUI.SetActive(false);
        }

        private void Update()
        {
            if (_isControlled)
            {
                // Gerakin drone pakai WASD
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                Vector2 move = new Vector2(h, v).normalized * flySpeed;
                _droneRb.linearVelocity = move;

                // Tekan F → keluar
                if (Input.GetKeyDown(KeyCode.F))
                {
                    ExitDrone();
                }
                return;
            }

            // Cek radius ke player
            CheckPlayerInRange();

            // Player dalam radius + tekan E → masuk
            if (_playerInRange && _player != null && Input.GetKeyDown(KeyCode.E))
            {
                if (requireYellow && !_playerColorState.CanActivateElectricPanel())
                    return;

                EnterDrone();
            }
        }

        private void CheckPlayerInRange()
        {
            // Cari player
            if (_player == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    _player = playerObj;
                    _playerColorState = playerObj.GetComponent<PlayerColorState>();
                    _playerController = playerObj.GetComponent<PlayerController>();
                    _playerRb = playerObj.GetComponent<Rigidbody2D>();
                }
            }

            if (_player == null || !_player.activeInHierarchy)
            {
                _playerInRange = false;
                _promptUI.SetActive(false);
                return;
            }

            float dist = Vector2.Distance(transform.position, _player.transform.position);
            bool wasInRange = _playerInRange;
            _playerInRange = dist <= detectRadius;

            // Cek apakah boleh masuk (Kuning)
            bool canEnter = !requireYellow || 
                (_playerColorState != null && _playerColorState.CanActivateElectricPanel());

            // Show/hide prompt
            if (_playerInRange && canEnter)
            {
                _promptUI.SetActive(true);
            }
            else
            {
                _promptUI.SetActive(false);
            }
        }

        private void EnterDrone()
        {
            _isControlled = true;
            _promptUI.SetActive(false);

            // Sembunyikan player
            _player.SetActive(false);

            // Setup drone movement
            _droneRb.gravityScale = 0f;
            _droneRb.linearVelocity = Vector2.zero;

            // Visual
            if (droneRenderer != null)
                droneRenderer.color = UnityEngine.Color.yellow;
        }

        private void ExitDrone()
        {
            _isControlled = false;

            // Stop drone
            _droneRb.linearVelocity = Vector2.zero;

            // Munculkan player di bawah drone
            _player.transform.position = transform.position + Vector3.down * 1f;
            _player.SetActive(true);

            // Reset player velocity
            if (_playerRb != null)
                _playerRb.linearVelocity = Vector2.zero;

            // Visual balik normal
            if (droneRenderer != null)
                droneRenderer.color = UnityEngine.Color.white;
        }

        // Gizmo: visualisasi radius di Scene view
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }

        public void ResetDrone()
        {
            if (_isControlled && _player != null)
                ExitDrone();

            _isControlled = false;
            _playerInRange = false;
            _droneRb.linearVelocity = Vector2.zero;
            _promptUI.SetActive(false);

            if (droneRenderer != null)
                droneRenderer.color = UnityEngine.Color.white;
        }
    }
}