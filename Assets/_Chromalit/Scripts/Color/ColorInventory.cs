using System;
using UnityEngine;
using Chromalit.Core;
using Chromalit.Player;

namespace Chromalit.Color
{
    public class ColorInventory : MonoBehaviour
    {
        [System.Serializable]
        public struct Slot
        {
            public ColorData color;
            public int count;
        }

        [System.Serializable]
        public struct InventorySnapshot
        {
            public ColorData[] colors;
            public int[] counts;
        }

        [Header("Setup")]
        [SerializeField] private GameMode currentMode;

        [Header("Debug (read-only saat Play)")]
        [SerializeField] private Slot[] slots = new Slot[5];

        // Cached
        private PlayerColorState _colorState;

        // Events — UI subscribe ke ini
        public event Action OnInventoryChanged;

        // Public getters
        public Slot[] Slots => slots;
        public int MaxStack => currentMode != null ? currentMode.maxStackPerColor : 3;

        private void Awake()
        {
            _colorState = GetComponent<PlayerColorState>();
            ClearAll();
        }

        private void Update()
        {
            HandleInput();
        }

        // ─── Input 1-5 ──────────────────────────────────────

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Use(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) Use(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) Use(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) Use(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) Use(4);
        }

        // ─── Add ────────────────────────────────────────────

        /// <summary>
        /// Coba tambah warna ke inventory.
        /// Return true kalau berhasil, false kalau stack penuh dan slot penuh.
        /// </summary>
        public bool TryAdd(ColorData color)
        {
            if (color == null) return false;

            // Cari slot yang warnanya sama dan belum penuh
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].color == color && slots[i].count < MaxStack)
                {
                    slots[i].count++;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }

            // Cari slot kosong
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].color == null)
                {
                    slots[i].color = color;
                    slots[i].count = 1;
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }

            // Stack penuh dan nggak ada slot kosong
            return false;
        }

        // ─── Use ────────────────────────────────────────────

        /// <summary>
        /// Pakai warna dari slot index (0-4).
        /// Kurangi count, aktifkan warna, return ColorData yang dipakai.
        /// </summary>
        public ColorData Use(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= slots.Length) return null;
            if (slots[slotIndex].color == null || slots[slotIndex].count <= 0) return null;

            ColorData color = slots[slotIndex].color;
            slots[slotIndex].count--;

            // Kalau count habis, kosongkan slot
            if (slots[slotIndex].count <= 0)
            {
                slots[slotIndex].color = null;
                slots[slotIndex].count = 0;
            }

            // Apply warna ke player (source: Collectible = countdown 45 detik)
            _colorState.ApplyColor(color, ColorSource.Collectible);

            OnInventoryChanged?.Invoke();
            return color;
        }

        // ─── Snapshot / Restore (untuk Checkpoint) ───────────

        public InventorySnapshot TakeSnapshot()
        {
            InventorySnapshot snap = new InventorySnapshot
            {
                colors = new ColorData[slots.Length],
                counts = new int[slots.Length]
            };

            for (int i = 0; i < slots.Length; i++)
            {
                snap.colors[i] = slots[i].color;
                snap.counts[i] = slots[i].count;
            }

            return snap;
        }

        public void RestoreSnapshot(InventorySnapshot snap)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].color = snap.colors[i];
                slots[i].count = snap.counts[i];
            }

            OnInventoryChanged?.Invoke();
        }

        // ─── Clear ──────────────────────────────────────────

        public void ClearAll()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].color = null;
                slots[i].count = 0;
            }

            OnInventoryChanged?.Invoke();
        }

        // ─── Utility ────────────────────────────────────────

        /// <summary>
        /// Cek apakah warna tertentu bisa ditambahkan (untuk UI notifikasi)
        /// </summary>
        public bool CanAdd(ColorData color)
        {
            if (color == null) return false;

            // Ada slot sama yang belum penuh?
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].color == color && slots[i].count < MaxStack)
                    return true;
            }

            // Ada slot kosong?
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].color == null)
                    return true;
            }

            return false;
        }

        public void SetGameMode(GameMode mode)
        {
            currentMode = mode;
        }
    }
}