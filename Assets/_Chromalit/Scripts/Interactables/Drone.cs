using UnityEngine;
using Chromalit.Player;
using Chromalit.Core;
using Chromalit.UI;

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

        [Header("Prompt")]
        [SerializeField] private InteractPrompt prompt;

        private bool _isControlled;
        private GameObject _player;
        private PlayerColorState _playerColorState;
        private Rigidbody2D _playerRb;
        private Rigidbody2D _droneRb;

        private void Awake()
        {
            _droneRb = GetComponent<Rigidbody2D>();
            if (droneRenderer == null)
                droneRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (_isControlled)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                _droneRb.linearVelocity = new Vector2(h, v).normalized * flySpeed;

                if (prompt != null) prompt.Show("F", "Keluar", PromptState.Ready);

                if (Input.GetKeyDown(KeyCode.F))
                    ExitDrone();
                return;
            }

            bool inRange = CheckPlayerInRange();
            if (!inRange)
            {
                if (prompt != null) prompt.Hide();
                return;
            }

            bool canEnter = !requireYellow
                || (_playerColorState != null && _playerColorState.CanActivateElectricPanel());

            if (prompt != null)
            {
                if (canEnter) prompt.Show("E", "Kendalikan Drone", PromptState.Ready);
                else prompt.Show("", "Butuh warna Kuning", PromptState.Locked);
            }

            if (canEnter && Input.GetKeyDown(KeyCode.E))
                EnterDrone();
        }

        private bool CheckPlayerInRange()
        {
            if (_player == null)
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p == null) return false;
                _player = p;
                _playerColorState = p.GetComponent<PlayerColorState>();
                _playerRb = p.GetComponent<Rigidbody2D>();
            }

            if (!_player.activeInHierarchy) return false;
            return Vector2.Distance(transform.position, _player.transform.position) <= detectRadius;
        }

        private void EnterDrone()
        {
            _isControlled = true;
            if (prompt != null) prompt.Press();

            var cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
            if (cam != null) cam.SetTarget(transform);

            _player.SetActive(false);

            _droneRb.gravityScale = 0f;
            _droneRb.linearVelocity = Vector2.zero;

            if (droneRenderer != null)
                droneRenderer.color = UnityEngine.Color.yellow;
        }

        private void ExitDrone()
        {
            _isControlled = false;
            _droneRb.linearVelocity = Vector2.zero;

            _player.transform.position = transform.position + Vector3.down * 1f;
            _player.SetActive(true);
            if (_playerRb != null) _playerRb.linearVelocity = Vector2.zero;

            var cam = Camera.main != null ? Camera.main.GetComponent<CameraFollow>() : null;
            if (cam != null) cam.SetTarget(_player.transform);

            if (droneRenderer != null)
                droneRenderer.color = UnityEngine.Color.white;

            if (prompt != null) prompt.Hide();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }

        public void ResetDrone()
        {
            if (_isControlled && _player != null) ExitDrone();
            _isControlled = false;
            _droneRb.linearVelocity = Vector2.zero;
            if (droneRenderer != null) droneRenderer.color = UnityEngine.Color.white;
            if (prompt != null) prompt.Hide();
        }
    }
}