using UnityEngine;
using Chromalit.Player;
using Chromalit.Core;

namespace Chromalit.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public class LaserTrap : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool electricLaser = true;

        [Header("Blink Pattern")]
        [SerializeField] private bool enableBlink = true;
        [SerializeField] private float onDuration = 2f;    // laser nyala berapa detik
        [SerializeField] private float offDuration = 1.5f;  // laser mati berapa detik
        [SerializeField] private float warningDuration = 0.5f; // kedip sebelum nyala

        [Header("References")]
        [SerializeField] private SpriteRenderer laserRenderer;
        [SerializeField] private Collider2D laserCollider;

        private bool _isOn = true;
        private float _timer;
        private enum LaserState { On, Off, Warning }
        private LaserState _state = LaserState.On;

        private void Awake()
        {
            if (laserRenderer == null)
                laserRenderer = GetComponent<SpriteRenderer>();
            if (laserCollider == null)
                laserCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            if (enableBlink)
            {
                _state = LaserState.On;
                _timer = onDuration;
                SetLaser(true, 1f);
            }
        }

        private void Update()
        {
            if (!enableBlink) return;

            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                switch (_state)
                {
                    case LaserState.On:
                        // Laser mati
                        _state = LaserState.Off;
                        _timer = offDuration;
                        SetLaser(false, 0.1f);
                        break;

                    case LaserState.Off:
                        // Mulai warning (kedip sebelum nyala)
                        _state = LaserState.Warning;
                        _timer = warningDuration;
                        break;

                    case LaserState.Warning:
                        // Laser nyala
                        _state = LaserState.On;
                        _timer = onDuration;
                        SetLaser(true, 1f);
                        break;
                }
            }

            // Efek kedip saat warning
            if (_state == LaserState.Warning)
            {
                float blink = Mathf.PingPong(Time.time * 8f, 1f);
                float alpha = Mathf.Lerp(0.1f, 0.6f, blink);
                SetAlpha(alpha);
                laserCollider.enabled = false; // belum bahaya saat warning
            }
        }

        private void SetLaser(bool on, float alpha)
        {
            _isOn = on;
            laserCollider.enabled = on;
            SetAlpha(alpha);
        }

        private void SetAlpha(float alpha)
        {
            if (laserRenderer == null) return;
            UnityEngine.Color c = laserRenderer.color;
            c.a = alpha;
            laserRenderer.color = c;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isOn) return;

            PlayerColorState colorState = other.GetComponent<PlayerColorState>();
            if (colorState == null) return;

            if (electricLaser && colorState.IsImmuneToElectric())
            {
                return;
            }

            CheckpointSystem checkpoint = other.GetComponent<CheckpointSystem>();
            if (checkpoint != null)
            {
                checkpoint.Restore();
            }
        }
    }
}