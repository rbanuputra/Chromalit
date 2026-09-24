using UnityEngine;

namespace Chromalit.Core
{
    [RequireComponent(typeof(Collider2D))]
    public class CheckpointTrigger : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer flagRenderer;
        private bool _activated;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activated) return;

            CheckpointSystem checkpoint = other.GetComponent<CheckpointSystem>();
            if (checkpoint == null) return;

            checkpoint.SaveCheckpoint();
            _activated = true;

            // Visual feedback — ganti warna flag
            if (flagRenderer != null)
            {
                flagRenderer.color = UnityEngine.Color.green;
            }
        }
    }
}