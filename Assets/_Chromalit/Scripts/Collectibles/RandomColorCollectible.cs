using UnityEngine;
using Chromalit.Color;

namespace Chromalit.Collectibles
{
    [RequireComponent(typeof(Collider2D))]
    public class RandomColorCollectible : MonoBehaviour
    {
        [System.Serializable]
        public class ColorChance
        {
            public ColorData color;

            [Tooltip("Bobot peluang. Tidak harus total 100, akan dinormalisasi otomatis.")]
            [Min(0f)] public float weight = 25f;

            [Tooltip("Peluang sebenarnya (otomatis dihitung, read-only).")]
            public string chance;
        }

        [Header("Warna & Peluang")]
        [SerializeField] private ColorChance[] colorChances =
        {
            new ColorChance { weight = 25f },
            new ColorChance { weight = 25f },
            new ColorChance { weight = 25f },
            new ColorChance { weight = 25f },
        };

        [Header("Visual")]
        [SerializeField] private SpriteRenderer iconRenderer;
        [SerializeField] private bool cycleColorVisual = true;
        [SerializeField] private float colorChangeInterval = 0.3f;

        [Header("ID")]
        [SerializeField] private string uniqueId;

        private bool _collected;
        private float _colorTimer;
        private int _visualIndex;

        // ─── Editor helpers ─────────────────────────────────

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = System.Guid.NewGuid().ToString();
        }

        // Dipanggil setiap ada perubahan di Inspector → update kolom "chance"
        private void OnValidate()
        {
            if (colorChances == null) return;

            float total = TotalWeight();
            foreach (var entry in colorChances)
            {
                if (entry == null) continue;

                if (entry.color == null)
                    entry.chance = "— (warna kosong)";
                else if (total <= 0f)
                    entry.chance = "0%";
                else
                    entry.chance = $"{(entry.weight / total) * 100f:0.#}%";
            }
        }

        // ─── Runtime ────────────────────────────────────────

        private void Awake()
        {
            if (iconRenderer == null)
                iconRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_collected || !cycleColorVisual || iconRenderer == null) return;
            if (colorChances == null || colorChances.Length == 0) return;

            _colorTimer += Time.deltaTime;
            if (_colorTimer < colorChangeInterval) return;
            _colorTimer = 0f;

            // Cuma tampilkan warna yang peluangnya > 0
            for (int i = 0; i < colorChances.Length; i++)
            {
                _visualIndex = (_visualIndex + 1) % colorChances.Length;
                var entry = colorChances[_visualIndex];
                if (entry != null && entry.color != null && entry.weight > 0f)
                {
                    iconRenderer.color = entry.color.displayColor;
                    break;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;

            ColorInventory inventory = other.GetComponent<ColorInventory>();
            if (inventory == null) return;

            ColorData picked = PickWeightedColor();
            if (picked == null)
            {
                Debug.LogWarning($"{name}: semua bobot warna 0 atau warna belum di-assign!");
                return;
            }

            if (inventory.TryAdd(picked))
            {
                _collected = true;
                Debug.Log($"Random collectible: dapat warna {picked.colorName}!");
                gameObject.SetActive(false);
            }
            else
            {
                Debug.Log($"Inventory penuh! Tidak bisa mengambil {picked.colorName}");
            }
        }

        /// <summary>Pilih warna berdasarkan bobot peluang.</summary>
        private ColorData PickWeightedColor()
        {
            float total = TotalWeight();
            if (total <= 0f) return null;

            float roll = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (var entry in colorChances)
            {
                if (entry == null || entry.color == null || entry.weight <= 0f) continue;

                cumulative += entry.weight;
                if (roll < cumulative)
                    return entry.color;
            }

            // Fallback (pembulatan float): ambil entri valid terakhir
            for (int i = colorChances.Length - 1; i >= 0; i--)
            {
                var e = colorChances[i];
                if (e != null && e.color != null && e.weight > 0f) return e.color;
            }
            return null;
        }

        private float TotalWeight()
        {
            float total = 0f;
            if (colorChances == null) return total;

            foreach (var entry in colorChances)
            {
                if (entry != null && entry.color != null && entry.weight > 0f)
                    total += entry.weight;
            }
            return total;
        }

        // ─── Checkpoint ─────────────────────────────────────

        public void ResetCollectible()
        {
            _collected = false;
            gameObject.SetActive(true);
        }

        public string UniqueId => uniqueId;
        public bool IsCollected => _collected;
    }
}