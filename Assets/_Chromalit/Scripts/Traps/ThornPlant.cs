using UnityEngine;
using Chromalit.Player;
using Chromalit.Core;

namespace Chromalit.Traps
{
    [RequireComponent(typeof(Collider2D))]
    public class ThornPlant : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool killOnContact = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerColorState colorState = other.GetComponent<PlayerColorState>();
            if (colorState == null) return;

            // Hijau kebal terhadap duri tanaman
            if (colorState.IsImmuneToPoison())
            {
                return;
            }

            if (killOnContact)
            {
                CheckpointSystem checkpoint = other.GetComponent<CheckpointSystem>();
                if (checkpoint != null)
                {
                    checkpoint.Restore();
                }
            }
        }
    }
}