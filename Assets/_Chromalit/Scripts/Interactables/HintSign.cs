using UnityEngine;
using Chromalit.UI;

namespace Chromalit.Interactables
{
    public class HintSign : MonoBehaviour
    {
        [Header("Isi Hint")]
        [SerializeField] private string title = "Petunjuk";
        [TextArea(3, 8)]
        [SerializeField] private string message =
            "Kunci tersembunyi di seberang air.\nGunakan warna Biru untuk berenang!";

        [Header("Settings")]
        [SerializeField] private float detectRadius = 2f;
        [SerializeField] private string promptLabel = "Baca Peta";

        [Header("Prompt")]
        [SerializeField] private InteractPrompt prompt;

        private Transform _player;
        private HintPanelUI _panel;

        private void Start()
        {
            _panel = FindFirstObjectByType<HintPanelUI>();
            if (_panel == null)
                Debug.LogWarning("HintPanelUI tidak ditemukan di scene!");
        }

        private void Update()
        {
            if (HintPanelUI.BlocksInput)
            {
                if (prompt != null) prompt.Hide();
                return;
            }

            if (_player == null)
            {
                GameObject p = GameObject.FindWithTag("Player");
                if (p != null) _player = p.transform;
            }

            bool inRange = _player != null
                && _player.gameObject.activeInHierarchy
                && Vector2.Distance(transform.position, _player.position) <= detectRadius;

            if (!inRange)
            {
                if (prompt != null) prompt.Hide();
                return;
            }

            if (prompt != null) prompt.Show("E", promptLabel, PromptState.Ready);

            if (Input.GetKeyDown(KeyCode.E) && _panel != null)
            {
                if (prompt != null) prompt.Press();
                _panel.Open(title, message);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = UnityEngine.Color.green;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
}