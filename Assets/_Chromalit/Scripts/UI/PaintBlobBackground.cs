using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chromalit.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class PaintBlobBackground : MonoBehaviour
    {
        [SerializeField] private Sprite blobSprite;
        [SerializeField] private int count = 18;
        [SerializeField] private Vector2 sizeRange = new Vector2(40f, 160f);
        [SerializeField] private Vector2 speedRange = new Vector2(15f, 45f);
        [SerializeField, Range(0f, 1f)] private float alpha = 0.18f;
        [SerializeField] private UnityEngine.Color[] palette =
        {
            new UnityEngine.Color(0.91f, 0.30f, 0.24f),
            new UnityEngine.Color(0.20f, 0.60f, 0.86f),
            new UnityEngine.Color(0.95f, 0.77f, 0.06f),
            new UnityEngine.Color(0.18f, 0.80f, 0.44f),
        };

        private class Blob
        {
            public RectTransform rt;
            public float size, speed, swayAmp, swayFreq, phase, baseX, y;
        }

        private readonly List<Blob> _blobs = new List<Blob>();
        private RectTransform _area;

        private void Start()
        {
            _area = (RectTransform)transform;
            for (int i = 0; i < count; i++)
                _blobs.Add(CreateBlob(randomY: true));
        }

        private Blob CreateBlob(bool randomY)
        {
            GameObject go = new GameObject("Blob", typeof(RectTransform), typeof(Image));
            go.transform.SetParent(transform, false);

            Image img = go.GetComponent<Image>();
            img.sprite = blobSprite;
            img.raycastTarget = false;
            UnityEngine.Color c = palette[Random.Range(0, palette.Length)];
            c.a = alpha;
            img.color = c;

            var blob = new Blob { rt = (RectTransform)go.transform };
            blob.rt.anchorMin = blob.rt.anchorMax = new Vector2(0.5f, 0.5f);
            Respawn(blob, randomY);
            return blob;
        }

        private void Respawn(Blob b, bool randomY)
        {
            float w = _area.rect.width, h = _area.rect.height;
            b.size = Random.Range(sizeRange.x, sizeRange.y);
            b.speed = Random.Range(speedRange.x, speedRange.y);
            b.swayAmp = Random.Range(10f, 40f);
            b.swayFreq = Random.Range(0.3f, 0.8f);
            b.phase = Random.Range(0f, Mathf.PI * 2f);
            b.baseX = Random.Range(-w / 2f, w / 2f);
            b.y = randomY ? Random.Range(-h / 2f, h / 2f) : -h / 2f - b.size;
            b.rt.sizeDelta = Vector2.one * b.size;
        }

        private void Update()
        {
            float dt = Time.unscaledDeltaTime;
            float t = Time.unscaledTime;
            float h = _area.rect.height;

            foreach (Blob b in _blobs)
            {
                b.y += b.speed * dt;
                if (b.y > h / 2f + b.size) Respawn(b, randomY: false);

                float x = b.baseX + Mathf.Sin(t * b.swayFreq + b.phase) * b.swayAmp;
                b.rt.anchoredPosition = new Vector2(x, b.y);
            }
        }
    }
}