using System;
using UnityEngine;

namespace Chromalit.Player
{
    public class KeyHolder : MonoBehaviour
    {
        [SerializeField] private int keyCount;

        public event Action<int> OnKeyCountChanged;

        public int KeyCount => keyCount;

        public void AddKey()
        {
            keyCount++;
            OnKeyCountChanged?.Invoke(keyCount);
            Debug.Log($"Key collected! Total: {keyCount}");
        }

        public bool UseKey()
        {
            if (keyCount <= 0) return false;

            keyCount--;
            OnKeyCountChanged?.Invoke(keyCount);
            return true;
        }

        // Untuk checkpoint snapshot
        public int TakeSnapshot()
        {
            return keyCount;
        }

        public void RestoreSnapshot(int count)
        {
            keyCount = count;
            OnKeyCountChanged?.Invoke(keyCount);
        }
    }
}