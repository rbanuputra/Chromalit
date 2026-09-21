using UnityEngine;

namespace Chromalit.Player
{
    public class PlayerGroundCheck : MonoBehaviour
    {
        [SerializeField] private Vector2 checkSize = new Vector2(0.4f, 0.05f);

        // Visualisasi di Scene view biar gampang atur posisi
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawWireCube(transform.position, checkSize);
        }
    }
}