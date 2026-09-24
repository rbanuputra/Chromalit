using UnityEngine;
using Chromalit.Color;
using Chromalit.Player;

namespace Chromalit.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public class PaintBomb : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private ColorData bombColor;

        [Header("ID (untuk checkpoint tracking)")]
        [SerializeField] private string uniqueId;

        private bool _exploded;

        private void Reset()
        {
            // Auto-generate unique ID saat component ditambahkan
            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = System.Guid.NewGuid().ToString();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_exploded) return;

            PlayerColorState colorState = other.GetComponent<PlayerColorState>();
            if (colorState == null) return;

            // Apply warna dari bom (permanen, bukan collectible)
            colorState.ApplyColor(bombColor, ColorSource.Bomb);

            _exploded = true;
            // TODO: mainkan VFX percikan cat
            gameObject.SetActive(false);
        }

        // Dipanggil saat restore checkpoint
        public void ResetBomb()
        {
            _exploded = false;
            gameObject.SetActive(true);
        }

        public string UniqueId => uniqueId;
        public bool IsExploded => _exploded;
    }
}