using UnityEngine;
using UnityEngine.Tilemaps;

namespace Chromalit.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Bounds (auto-calculated from Ground)")]
        [SerializeField] private Transform groundTransform;
        [SerializeField] private bool clampY = true;

        private float _minX, _maxX, _minY, _maxY;
        private bool _hasBounds;
        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            CalculateBounds();
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void CalculateBounds()
        {
            _hasBounds = false;
            if (groundTransform == null || _cam == null) return;

            float left, right, bottom, top;

            var tilemap = groundTransform.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                tilemap.CompressBounds();
                Bounds b = tilemap.localBounds;
                Vector3 p = groundTransform.position;
                left = p.x + b.min.x;
                right = p.x + b.max.x;
                bottom = p.y + b.min.y;
                top = p.y + b.max.y;
            }
            else
            {
                Vector3 s = groundTransform.lossyScale;
                Vector3 p = groundTransform.position;
                left = p.x - s.x / 2f;
                right = p.x + s.x / 2f;
                bottom = p.y - s.y / 2f;
                top = p.y + s.y / 2f;
            }

            float halfH = _cam.orthographicSize;
            float halfW = halfH * _cam.aspect;

            _minX = left + halfW;
            _maxX = right - halfW;
            _minY = bottom + halfH;          // kamera nggak turun di bawah dasar level
            _maxY = top + halfH * 2f;        // kasih ruang di atas platform tertinggi

            // Kalau level lebih sempit/pendek dari kamera, kunci di tengah
            if (_minX > _maxX) _minX = _maxX = (left + right) / 2f;
            if (_minY > _maxY) _minY = _maxY = (bottom + top) / 2f;

            _hasBounds = true;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = target.position + offset;
            Vector3 pos = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

            if (_hasBounds)
            {
                pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
                if (clampY) pos.y = Mathf.Clamp(pos.y, _minY, _maxY);
            }

            pos.z = offset.z;
            transform.position = pos;
        }
    }
}