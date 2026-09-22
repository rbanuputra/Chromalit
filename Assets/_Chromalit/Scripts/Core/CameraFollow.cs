using UnityEngine;

namespace Chromalit.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 1f, -10f);
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Bounds (auto-calculated from Ground)")]
        [SerializeField] private Transform groundTransform;

        private float _minX;
        private float _maxX;
        private float _minY;
        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            CalculateBounds();
        }

        private void CalculateBounds()
        {
            if (groundTransform == null || _cam == null) return;

            // Hitung lebar ground dari scale
            float groundHalfWidth = groundTransform.localScale.x / 2f;
            float groundLeft = groundTransform.position.x - groundHalfWidth;
            float groundRight = groundTransform.position.x + groundHalfWidth;

            // Hitung setengah lebar kamera (orthographic)
            float camHalfWidth = _cam.orthographicSize * _cam.aspect;

            // Clamp supaya kamera nggak nunjukin area di luar ground
            _minX = groundLeft + camHalfWidth;
            _maxX = groundRight - camHalfWidth;

            // Batas bawah kamera = atas ground
            float groundTop = groundTransform.position.y + (groundTransform.localScale.y / 2f);
            _minY = groundTop + _cam.orthographicSize * 0.3f;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothed = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            smoothed.x = Mathf.Clamp(smoothed.x, _minX, _maxX);
            smoothed.y = Mathf.Max(smoothed.y, _minY);

            transform.position = smoothed;
        }
    }
}