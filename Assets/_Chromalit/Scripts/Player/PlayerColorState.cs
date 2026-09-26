using System;
using UnityEngine;
using Chromalit.Color;
using Chromalit.Core;

namespace Chromalit.Player
{
    public enum ColorSource
    {
        None,
        Bomb,
        Collectible
    }

    public class PlayerColorState : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private ColorData defaultColor;  // Color_White
        [SerializeField] private GameMode currentMode;

        // State
        private ColorData _currentColor;
        private ColorSource _colorSource;
        private float _countdownTimer;
        private bool _countdownActive;

        // Cached
        private SpriteRenderer _spriteRenderer;
        private PlayerController _playerController;

        // Events — UI subscribe ke ini
        public event Action<ColorData, ColorSource> OnColorChanged;
        public event Action<float, float> OnCountdownTick; // (timeLeft, totalDuration)
        public event Action OnColorExpired;

        // Public getters
        public ColorData CurrentColor => _currentColor;
        public ColorSource CurrentSource => _colorSource;
        public bool IsCountdownActive => _countdownActive;
        public float CountdownTimeLeft => _countdownTimer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Mulai dengan warna default (Putih)
            Neutralize();
        }

        private void Update()
        {
            if (!_countdownActive) return;

            _countdownTimer -= Time.deltaTime;

            // Broadcast tick ke UI (countdown bar)
            OnCountdownTick?.Invoke(_countdownTimer, currentMode.collectibleColorDuration);

            if (_countdownTimer <= 0f)
            {
                _countdownTimer = 0f;
                _countdownActive = false;
                OnColorExpired?.Invoke();
                Neutralize();
            }
        }

        /// <summary>
        /// Ganti warna aktif. Dipanggil oleh PaintBomb atau ColorInventory.
        /// </summary>
        public void ApplyColor(ColorData newColor, ColorSource source)
        {
            if (newColor == null) return;

            // Kalau warna sama + source collectible + countdown jalan → reset countdown
            if (_currentColor == newColor && source == ColorSource.Collectible && _countdownActive)
            {
                _countdownTimer = currentMode.collectibleColorDuration;
                return;
            }

            // Ganti warna
            _currentColor = newColor;
            _colorSource = source;

            // Apply buff/debuff ke PlayerController
            _playerController.SetSpeedModifier(newColor.speedMultiplier);
            _playerController.SetJumpModifier(newColor.jumpMultiplier);

            // Handle countdown
            if (source == ColorSource.Collectible)
            {
                _countdownTimer = currentMode.collectibleColorDuration;
                _countdownActive = true;
            }
            else
            {
                // Bom = permanen, matikan countdown
                _countdownTimer = 0f;
                _countdownActive = false;
            }

            OnColorChanged?.Invoke(_currentColor, _colorSource);
        }

        /// <summary>
        /// Reset ke warna Putih (netral). Dipanggil saat:
        /// - Countdown habis
        /// - Kena air (kalau neutralizedByWater == true)
        /// - Sentuh cleansing station
        /// - Respawn di checkpoint
        /// </summary>
        public void Neutralize()
        {
            _currentColor = defaultColor;
            _colorSource = ColorSource.None;
            _countdownTimer = 0f;
            _countdownActive = false;

            _spriteRenderer.color = UnityEngine.Color.white;
            _playerController.SetSpeedModifier(1f);
            _playerController.SetJumpModifier(1f);

            OnColorChanged?.Invoke(_currentColor, _colorSource);
        }

        /// <summary>
        /// Dipanggil saat player masuk air/genangan.
        /// Cek apakah warna aktif bisa dinetralkan oleh air.
        /// </summary>
        public void TryNeutralizeByWater()
        {
            if (_currentColor != null && _currentColor.neutralizedByWater)
            {
                Neutralize();
            }
        }

        /// <summary>
        /// Cek apakah player punya immunity tertentu.
        /// Dipanggil oleh trap untuk cek apakah player kena damage.
        /// </summary>
        public bool IsImmuneToFire() => _currentColor != null && _currentColor.immuneToFire;
        public bool IsImmuneToPoison() => _currentColor != null && _currentColor.immuneToPoison;
        public bool IsImmuneToElectric() => _currentColor != null && _currentColor.immuneToElectric;
        public bool CanSwim() => _currentColor != null && _currentColor.canSwim;
        public bool CanMeltIce() => _currentColor != null && _currentColor.canMeltIce;
        public bool CanActivateElectricPanel() => _currentColor != null && _currentColor.canActivateElectricPanel;

        /// <summary>
        /// Set mode (dipanggil GameManager saat ganti level/mode)
        /// </summary>
        public void SetGameMode(GameMode mode)
        {
            currentMode = mode;
        }
    }
}