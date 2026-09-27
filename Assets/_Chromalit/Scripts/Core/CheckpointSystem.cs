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
        private Vector3 _startPosition;

        // Cached
        private PlayerColorState _colorState;
        private ColorInventory _inventory;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _colorState = GetComponent<PlayerColorState>();
            _inventory = GetComponent<ColorInventory>();
            _rb = GetComponent<Rigidbody2D>();
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Restore();
            }
        }

        /// <summary>
        /// Simpan snapshot saat player sentuh checkpoint.
        /// </summary>
        public void SaveCheckpoint()
        {
            _lastCheckpoint.inventory = _inventory.TakeSnapshot();
            _lastCheckpoint.playerPosition = transform.position;

            ColorCollectible[] allCollectibles = FindObjectsByType<ColorCollectible>(FindObjectsSortMode.None);
            var collectedIds = new System.Collections.Generic.List<string>();
            foreach (var c in allCollectibles)
            {
                if (c.IsCollected) collectedIds.Add(c.UniqueId);
            }
            _lastCheckpoint.collectedItemIds = collectedIds.ToArray();

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
        /// Kalau belum ada checkpoint, respawn ke posisi awal level.
        /// </summary>
        public void Restore()
        {
            // Reset velocity biar player nggak lanjut gerak setelah respawn
            if (_rb != null)
            {
                _rb.linearVelocity = Vector2.zero;
            }

            // Reset warna ke Putih
            _colorState.Neutralize();

            if (!_hasCheckpoint)
            {
                // Belum ada checkpoint — respawn ke posisi awal
                transform.position = _startPosition;
                _inventory.ClearAll();

                // Reset semua collectible dan bom ke kondisi awal
                ResetAllCollectibles();
                ResetAllBombs();

                Debug.Log("No checkpoint — restored to start position!");
                return;
            }

            // Restore inventory
            _inventory.RestoreSnapshot(_lastCheckpoint.inventory);

            // Restore posisi
            transform.position = _lastCheckpoint.playerPosition;

            // Restore collectible
            ColorCollectible[] allCollectibles = FindObjectsByType<ColorCollectible>(FindObjectsSortMode.None);
            foreach (var c in allCollectibles)
            {
                bool wasCollectedAtCheckpoint = System.Array.Exists(
                    _lastCheckpoint.collectedItemIds, id => id == c.UniqueId
                );

                if (wasCollectedAtCheckpoint)
                {
                    c.gameObject.SetActive(false);
                }
                else
                {
                    c.ResetCollectible();
                }
            }

            // Restore bom
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

        private void ResetAllCollectibles()
        {
            ColorCollectible[] all = FindObjectsByType<ColorCollectible>(FindObjectsSortMode.None);
            foreach (var c in all)
            {
                c.ResetCollectible();
            }
        }

        private void ResetAllBombs()
        {
            PaintBomb[] all = FindObjectsByType<PaintBomb>(FindObjectsSortMode.None);
            foreach (var b in all)
            {
                b.ResetBomb();
            }
        }

        public bool HasCheckpoint => _hasCheckpoint;
    }
}