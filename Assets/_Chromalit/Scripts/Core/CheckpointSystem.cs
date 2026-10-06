using UnityEngine;
using UnityEngine.SceneManagement;
using Chromalit.Color;
using Chromalit.Player;
using Chromalit.Traps;
using Chromalit.Collectibles;

namespace Chromalit.Core
{
    public class CheckpointSystem : MonoBehaviour
    {
        [Header("Mode")]
        [Tooltip("ON = mati langsung restart level dari awal. OFF = balik ke checkpoint terakhir.")]
        [SerializeField] private bool restartOnDeath = true;

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

        public void SaveCheckpoint()
        {
            _lastCheckpoint.inventory = _inventory.TakeSnapshot();
            _lastCheckpoint.playerPosition = transform.position;

            var collectedIds = new System.Collections.Generic.List<string>();
            foreach (var c in FindObjectsByType<ColorCollectible>(FindObjectsSortMode.None))
                if (c.IsCollected) collectedIds.Add(c.UniqueId);
            _lastCheckpoint.collectedItemIds = collectedIds.ToArray();

            var explodedIds = new System.Collections.Generic.List<string>();
            foreach (var b in FindObjectsByType<PaintBomb>(FindObjectsSortMode.None))
                if (b.IsExploded) explodedIds.Add(b.UniqueId);
            _lastCheckpoint.explodedBombIds = explodedIds.ToArray();

            _hasCheckpoint = true;
            Debug.Log("Checkpoint saved!");
        }

        /// <summary>
        /// Dipanggil saat mati (trap) atau tekan R.
        /// </summary>
        public void Restore()
        {
            if (restartOnDeath || !_hasCheckpoint)
            {
                RestartLevel();
                return;
            }

            RestoreToCheckpoint();
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void RestoreToCheckpoint()
        {
            if (_rb != null) _rb.linearVelocity = Vector2.zero;
            _colorState.Neutralize();

            _inventory.RestoreSnapshot(_lastCheckpoint.inventory);
            transform.position = _lastCheckpoint.playerPosition;

            foreach (var c in FindObjectsByType<ColorCollectible>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                bool taken = System.Array.Exists(_lastCheckpoint.collectedItemIds, id => id == c.UniqueId);
                if (taken) c.gameObject.SetActive(false);
                else c.ResetCollectible();
            }

            foreach (var b in FindObjectsByType<PaintBomb>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                bool exploded = System.Array.Exists(_lastCheckpoint.explodedBombIds, id => id == b.UniqueId);
                if (exploded) b.gameObject.SetActive(false);
                else b.ResetBomb();
            }

            Debug.Log("Restored to checkpoint!");
        }

        public bool HasCheckpoint => _hasCheckpoint;
    }
}