using UnityEngine;
using Chromalit.Player;

namespace Chromalit.Collectibles
{
    [RequireComponent(typeof(Collider2D))]
    public class KeyCollectible : MonoBehaviour
    {
        [SerializeField] private string uniqueId;

        private bool _collected;

        private void Reset()
        {
            if (string.IsNullOrEmpty(uniqueId))
                uniqueId = System.Guid.NewGuid().ToString();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected) return;

            KeyHolder keyHolder = other.GetComponent<KeyHolder>();
            if (keyHolder == null) return;

            keyHolder.AddKey();
            _collected = true;
            gameObject.SetActive(false);
        }

        public void ResetCollectible()
        {
            _collected = false;
            gameObject.SetActive(true);
        }

        public string UniqueId => uniqueId;
        public bool IsCollected => _collected;
    }
}