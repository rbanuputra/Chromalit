using UnityEngine;
using Chromalit.Color;
using Chromalit.Player;
using Chromalit.Traps;
using Chromalit.Collectibles;

namespace Chromalit.Core
{
    public class CheckpointSystem : MonoBehaviour
    {
        [System.Serializable]
        public struct LevelSnapshot
        {
            public ColorInventory.InventorySnapshot inventory;
            public Vector3 playerPosition;
            public string[] collectedItemIds;
            public string[] explodedBombIds;
        }

        private LevelSnapshot _lastCheckpoint;
        private bool _hasCheckpoint;

        // Cached
        private PlayerColorState _colorState;
        private ColorInventory _inventory;

        private void Awake()
        {
            _colorState = GetComponent<PlayerColorState>();
            _inventory = GetComponent<ColorInventory>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && _hasCheckpoint)
            {
                Restore();
            }
        }

        /// <summary>
        /// Simpan snapshot saat player sentuh checkpoint.
        /// </summary>
        public void SaveCheckpoint()
        {
            // Snapshot inventory
            _lastCheckpoint.inventory = _inventory.TakeSnapshot();

            // Snapshot posisi player
            _lastCheckpoint.playerPosition = transform.position;

            // Snapshot collectible yang sudah diambil
            ColorCollectible[] allCollectibles = FindObjectsByType<ColorCollectible>(FindObjectsSortMode.None);
            var collectedIds = new System.Collections.Generic.List<string>();
            foreach (var c in allCollectibles)
            {
                if (c.IsCollected) collectedIds.Add(c.UniqueId);
            }
            _lastCheckpoint.collectedItemIds = collectedIds.ToArray();

            // Snapshot bom yang sudah meledak
            PaintBomb[] allBombs = FindObjectsByType<PaintBomb>(FindObjectsSortMode.None);
            var explodedIds = new System.Collections.Generic.List<string>();
            foreach (var b in allBombs)
            {
                if (b.IsExploded) explodedIds.Add(b.UniqueId);
            }
            _lastCheckpoint.explodedBombIds = explodedIds.ToArray();

            _hasCheckpoint = true;
            Debug.Log("Checkpoint saved!");
        }

        /// <summary>
        /// Kembalikan semua kondisi ke checkpoint terakhir.
        /// </summary>
        public void Restore()
        {
            if (!_hasCheckpoint) return;

            // Restore inventory
            _inventory.RestoreSnapshot(_lastCheckpoint.inventory);

            // Restore posisi
            transform.position = _lastCheckpoint.playerPosition;

            // Reset warna ke Putih
            _colorState.Neutralize();

            // Restore collectible — yang belum diambil saat checkpoint, munculkan kembali
            ColorCollectible[] allCollectibles = FindObjectsByType<ColorCollectible>(FindObjectsSortMode.None);
            foreach (var c in allCollectibles)
            {
                bool wasCollectedAtCheckpoint = System.Array.Exists(
                    _lastCheckpoint.collectedItemIds, id => id == c.UniqueId
                );

                if (wasCollectedAtCheckpoint)
                {
                    // Sudah diambil sebelum checkpoint, tetap hilang
                    c.gameObject.SetActive(false);
                }
                else
                {
                    // Belum diambil saat checkpoint, munculkan kembali
                    c.ResetCollectible();
                }
            }

            // Restore bom — yang belum meledak saat checkpoint, munculkan kembali
            PaintBomb[] allBombs = FindObjectsByType<PaintBomb>(FindObjectsSortMode.None);
            foreach (var b in allBombs)
            {
                bool wasExplodedAtCheckpoint = System.Array.Exists(
                    _lastCheckpoint.explodedBombIds, id => id == b.UniqueId
                );

                if (wasExplodedAtCheckpoint)
                {
                    b.gameObject.SetActive(false);
                }
                else
                {
                    b.ResetBomb();
                }
            }

            Debug.Log("Restored to checkpoint!");
        }

        public bool HasCheckpoint => _hasCheckpoint;
    }
}