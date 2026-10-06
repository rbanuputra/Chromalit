using UnityEngine;
using TMPro;

namespace Chromalit.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class ChromaTitle : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Color[] palette =
        {
            new UnityEngine.Color(0.91f, 0.30f, 0.24f), // merah
            new UnityEngine.Color(0.20f, 0.60f, 0.86f), // biru
            new UnityEngine.Color(0.95f, 0.77f, 0.06f), // kuning
            new UnityEngine.Color(0.18f, 0.80f, 0.44f), // hijau
        };

        [SerializeField] private float waveHeight = 8f;
        [SerializeField] private float waveSpeed = 3f;
        [SerializeField] private float waveSpacing = 0.5f;
        [Tooltip("0 = warna per huruf diam")]
        [SerializeField] private float colorCycleSpeed = 1.5f;

        private TMP_Text _text;

        private void Awake() => _text = GetComponent<TMP_Text>();

        private void Update()
        {
            if (palette == null || palette.Length == 0) return;

            _text.ForceMeshUpdate();
            TMP_TextInfo info = _text.textInfo;
            float t = Time.unscaledTime;

            for (int i = 0; i < info.characterCount; i++)
            {
                TMP_CharacterInfo ch = info.characterInfo[i];
                if (!ch.isVisible) continue;

                int mi = ch.materialReferenceIndex;
                int vi = ch.vertexIndex;
                Vector3[] verts = info.meshInfo[mi].vertices;
                Color32[] cols = info.meshInfo[mi].colors32;

                float offset = Mathf.Sin(t * waveSpeed + i * waveSpacing) * waveHeight;
                int colorIndex = Mathf.FloorToInt(i + t * colorCycleSpeed) % palette.Length;
                Color32 c = palette[colorIndex];

                for (int j = 0; j < 4; j++)
                {
                    verts[vi + j].y += offset;
                    cols[vi + j] = c;
                }
            }

            _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
}