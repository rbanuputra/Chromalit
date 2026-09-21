using UnityEngine;

namespace Chromalit.Core
{
    [CreateAssetMenu(fileName = "NewGameMode", menuName = "Chromalit/Game Mode")]
    public class GameMode : ScriptableObject
    {
        [Header("Identitas")]
        public string modeName;

        [Header("Inventory")]
        [Tooltip("Batas maksimal stack per warna di inventory")]
        public int maxStackPerColor = 3;

        [Header("Warna dari Collectible")]
        [Tooltip("Durasi warna aktif dari collectible (detik)")]
        public float collectibleColorDuration = 45f;
    }
}