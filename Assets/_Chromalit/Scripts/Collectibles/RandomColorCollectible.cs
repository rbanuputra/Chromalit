using UnityEngine;
using Chromalit.Color;

namespace Chromalit.Collectibles
{
    [RequireComponent(typeof(Collider2D))]
    public class RandomColorCollectible : MonoBehaviour
    {
        [Header("Warna yang mungkin keluar")]
        [SerializeField] private ColorData[] possibleColors;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private float colorChangeInterval = 0.3f; // kecepatan ganti warna visual

        [Header("ID")]
        [SerializeField] private string uniqueId;

        private bool _collected;
        private float _colorTimer;
        private int _currentColorIndex;

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = System.Guid.NewGuid().ToString();
        }

        private void Awake()
        {
            if (iconRenderer == null)
                iconRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_collected || possibleColors == null || possibleColors.Length == 0) return;

            // Visual: ganti-ganti warna biar keliatan "random"
            _colorTimer += Time.deltaTime;
            if (_colorTimer >= colorChangeInterval)
            {
                _colorTimer = 0f;
                _currentColorIndex = (_currentColorIndex + 1) % possibleColors.Length;
                if (iconRenderer != null && possibleColors[_currentColorIndex] != null)
                {
                    iconRenderer.color = possibleColors[_currentColorIndex].displayColor;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;

            ColorInventory inventory = other.GetComponent<ColorInventory>();
            if (inventory == null) return;

            // Pilih warna random
            ColorData randomColor = possibleColors[Random.Range(0, possibleColors.Length)];

            if (inventory.TryAdd(randomColor))
            {
                _collected = true;
                Debug.Log($"Random collectible: dapat warna {randomColor.colorName}!");
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log($"Inventory penuh! Tidak bisa mengambil {randomColor.colorName}");
            }
        }

        public void ResetCollectible()
        {
            _collected = false;
            gameObject.SetActive(true);
        }

        public string UniqueId => uniqueId;
        public bool IsCollected => _collected;
    }
}