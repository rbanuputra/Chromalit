using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Chromalit.Color;
using Chromalit.Player;

namespace Chromalit.UI
{
    public class InventoryHUD : MonoBehaviour
    {
        [Header("Slot UI (drag 5 slot)")]
        [SerializeField] private Image[] slotBackgrounds = new Image[5];
        [SerializeField] private Image[] slotColorIcons = new Image[5];
        [SerializeField] private TextMeshProUGUI[] slotCountTexts = new TextMeshProUGUI[5];
        [SerializeField] private TextMeshProUGUI[] slotKeyTexts = new TextMeshProUGUI[5];

        [Header("Active Color Indicator")]
        [SerializeField] private Image activeColorIcon;
        [SerializeField] private Image countdownBar;
        [SerializeField] private TextMeshProUGUI countdownText;

        [Header("Player Reference")]
        [SerializeField] private ColorInventory inventory;
        [SerializeField] private PlayerColorState colorState;

        [Header("Colors")]
        [SerializeField] private UnityEngine.Color emptySlotColor = new UnityEngine.Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private UnityEngine.Color filledSlotColor = new UnityEngine.Color(0.3f, 0.3f, 0.3f, 0.8f);

        private void OnEnable()
        {
            if (inventory != null)
                inventory.OnInventoryChanged += RefreshSlots;

            if (colorState != null)
            {
                colorState.OnColorChanged += OnColorChanged;
                colorState.OnCountdownTick += OnCountdownTick;
                colorState.OnColorExpired += OnColorExpired;
            }
        }

        private void OnDisable()
        {
            if (inventory != null)
                inventory.OnInventoryChanged -= RefreshSlots;

            if (colorState != null)
            {
                colorState.OnColorChanged -= OnColorChanged;
                colorState.OnCountdownTick -= OnCountdownTick;
                colorState.OnColorExpired -= OnColorExpired;
            }
        }

        private void Start()
        {
            InitKeyLabels();
            RefreshSlots();
            ResetCountdownUI();
        }

        // ─── Slot UI ────────────────────────────────────────

        private void InitKeyLabels()
        {
            for (int i = 0; i < slotKeyTexts.Length; i++)
            {
                if (slotKeyTexts[i] != null)
                    slotKeyTexts[i].text = (i + 1).ToString();
            }
        }

        private void RefreshSlots()
        {
            if (inventory == null) return;

            var slots = inventory.Slots;

            for (int i = 0; i < slotBackgrounds.Length; i++)
            {
                if (i >= slots.Length) continue;

                bool hasColor = slots[i].color != null && slots[i].count > 0;

                // Background
                if (slotBackgrounds[i] != null)
                    slotBackgrounds[i].color = hasColor ? filledSlotColor : emptySlotColor;

                // Color icon
                if (slotColorIcons[i] != null)
                {
                    if (hasColor)
                    {
                        slotColorIcons[i].color = slots[i].color.displayColor;
                        slotColorIcons[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        slotColorIcons[i].gameObject.SetActive(false);
                    }
                }

                // Count text
                if (slotCountTexts[i] != null)
                {
                    slotCountTexts[i].text = hasColor ? $"x{slots[i].count}" : "";
                }
            }
        }

        // ─── Active Color ───────────────────────────────────

        private void OnColorChanged(ColorData color, ColorSource source)
        {
            if (activeColorIcon != null && color != null)
            {
                activeColorIcon.color = color.displayColor;
            }

            if (source != ColorSource.Collectible)
            {
                ResetCountdownUI();
            }
        }

        // ─── Countdown ──────────────────────────────────────

        private void OnCountdownTick(float timeLeft, float totalDuration)
        {
            if (countdownBar != null)
            {
                countdownBar.gameObject.SetActive(true);
                countdownBar.fillAmount = timeLeft / totalDuration;

                // Berkedip di 5 detik terakhir
                if (timeLeft <= 5f)
                {
                    countdownBar.color = UnityEngine.Color.Lerp(
                        UnityEngine.Color.red,
                        UnityEngine.Color.white,
                        Mathf.PingPong(Time.time * 4f, 1f)
                    );
                }
                else
                {
                    countdownBar.color = UnityEngine.Color.white;
                }
            }

            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
                countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
            }
        }

        private void OnColorExpired()
        {
            ResetCountdownUI();
        }

        private void ResetCountdownUI()
        {
            if (countdownBar != null)
            {
                countdownBar.fillAmount = 1f;
                countdownBar.color = UnityEngine.Color.white;
                countdownBar.gameObject.SetActive(false);
            }

            if (countdownText != null)
            {
                countdownText.text = "";
                countdownText.gameObject.SetActive(false);
            }
        }
    }
}