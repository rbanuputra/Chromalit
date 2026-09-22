using UnityEngine;
using Chromalit.Color;

namespace Chromalit.Collectibles
{
    [RequireComponent(typeof(Collider2D))]
    public class ColorCollectible : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private ColorData collectibleColor;

        [Header("ID (untuk checkpoint tracking)")]
        [SerializeField] private string uniqueId;

        private bool _collected;

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = System.Guid.NewGuid().ToString();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;

            ColorInventory inventory = other.GetComponent<ColorInventory>();
            if (inventory == null) return;

            if (inventory.TryAdd(collectibleColor))
            {
                _collected = true;
                // TODO: mainkan SFX/VFX
                gameObject.SetActive(false);
            }
            else
            {
                // Stack penuh, collectible tetap di level
                // TODO: tampilkan notifikasi "Inventory penuh!"
                Debug.Log($"Inventory penuh! Tidak bisa mengambil {collectibleColor.colorName}");
            }
        }

        // Dipanggil saat restore checkpoint
        public void ResetCollectible()
        {
            _collected = false;
            gameObject.SetActive(true);
        }

        public string UniqueId => uniqueId;
        public bool IsCollected => _collected;
    }
}