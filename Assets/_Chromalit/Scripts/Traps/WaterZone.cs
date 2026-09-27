using UnityEngine;
using Chromalit.Player;
using Chromalit.Core;

namespace Chromalit.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public class WaterZone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float swimSpeed = 3f;
        [SerializeField] private float swimGravityScale = 0.5f;
        [SerializeField] private bool killNonBlue = false; // true = mati, false = cuma netralin warna

        private PlayerController _player;
        private PlayerColorState _colorState;
        private Rigidbody2D _playerRb;
        private float _originalGravity;
        private bool _playerInWater;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerColorState colorState = other.GetComponent<PlayerColorState>();
            if (colorState == null) return;

            _colorState = colorState;
            _player = other.GetComponent<PlayerController>();
            _playerRb = other.GetComponent<Rigidbody2D>();
            _playerInWater = true;

            // Set inWater buat animator (Roll Water buat Biru)
            _player.SetInWater(true);

            if (colorState.CanSwim())
            {
                // Biru — bisa berenang
                _originalGravity = _playerRb.gravityScale;
                _playerRb.gravityScale = swimGravityScale;
            }
            else
            {
                // Bukan Biru — netralin warna
                colorState.TryNeutralizeByWater();

                if (killNonBlue)
                {
                    // Respawn ke checkpoint
                    CheckpointSystem checkpoint = other.GetComponent<CheckpointSystem>();
                    if (checkpoint != null)
                    {
                        checkpoint.Restore();
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!_playerInWater || _colorState == null) return;

            // Kalau di air dan bisa berenang, ijinkan gerak atas/bawah
            if (_colorState.CanSwim())
            {
                float vertical = Input.GetAxisRaw("Vertical");
                if (Mathf.Abs(vertical) > 0.01f)
                {
                    _playerRb.linearVelocity = new Vector2(
                        _playerRb.linearVelocity.x,
                        vertical * swimSpeed
                    );
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerColorState colorState = other.GetComponent<PlayerColorState>();
            if (colorState == null) return;

            PlayerController player = other.GetComponent<PlayerController>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            // Reset gravity
            if (colorState.CanSwim() && rb != null)
            {
                rb.gravityScale = 3f; // gravity default
            }

            // Reset inWater
            if (player != null)
                player.SetInWater(false);

            _playerInWater = false;
            _colorState = null;
            _player = null;
            _playerRb = null;
        }
    }
}