using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset info UI - UI thông tin reset
    /// Displays detailed information about resets
    /// </summary>
    public class ResetInfoUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Info panel - Panel thông tin")]
        public GameObject infoPanel;

        [Tooltip("Title text - Text tiêu đề")]
        public Text titleText;

        [Tooltip("Info text - Text thông tin")]
        public Text infoText;

        [Tooltip("Stats text - Text chỉ số")]
        public Text statsText;

        [Tooltip("Close button - Nút đóng")]
        public Button closeButton;

        private static ResetInfoUI _instance;
        public static ResetInfoUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ResetInfoUI>();
                }
                return _instance;
            }
        }

        private CharacterStats currentCharacter;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            Hide();
        }

        /// <summary>
        /// Show info for character
        /// Hiển thị thông tin cho nhân vật
        /// </summary>
        public void Show(CharacterStats character)
        {
            if (character == null)
            {
                Debug.LogWarning("Cannot show info with null character");
                return;
            }

            currentCharacter = character;

            if (infoPanel != null)
                infoPanel.SetActive(true);

            UpdateUI();
        }

        /// <summary>
        /// Hide info panel
        /// Ẩn panel thông tin
        /// </summary>
        public void Hide()
        {
            if (infoPanel != null)
                infoPanel.SetActive(false);

            currentCharacter = null;
        }

        /// <summary>
        /// Update UI content
        /// Cập nhật nội dung UI
        /// </summary>
        private void UpdateUI()
        {
            if (currentCharacter == null)
                return;

            UpdateTitle();
            UpdateInfo();
            UpdateStats();
        }

        /// <summary>
        /// Update title
        /// Cập nhật tiêu đề
        /// </summary>
        private void UpdateTitle()
        {
            if (titleText == null)
                return;

            string title = $"Reset Information - {currentCharacter.name}";
            titleText.text = title;
        }

        /// <summary>
        /// Update general info
        /// Cập nhật thông tin chung
        /// </summary>
        private void UpdateInfo()
        {
            if (infoText == null || currentCharacter == null)
                return;

            string info = "╔═══════════════════════════════════════════════════╗\n";
            info += "║              RESET SYSTEM OVERVIEW                ║\n";
            info += "╠═══════════════════════════════════════════════════╣\n";
            info += $"║ Character: {currentCharacter.name.PadRight(36)}║\n";
            info += $"║ Level: {currentCharacter.level.ToString().PadRight(41)}║\n";
            info += "╠═══════════════════════════════════════════════════╣\n";
            info += $"║ Normal Resets:  {currentCharacter.normalResetCount.ToString().PadRight(32)}║\n";
            info += $"║ Grand Resets:   {currentCharacter.grandResetCount.ToString().PadRight(32)}║\n";
            info += $"║ Master Reset:   {(currentCharacter.hasMasterReset ? "Yes" : "No").PadRight(32)}║\n";
            info += "╠═══════════════════════════════════════════════════╣\n";
            info += "║ TYPE          │ CURRENT   │ MAX                 ║\n";
            info += "╠═══════════════════════════════════════════════════╣\n";
            info += $"║ Normal Reset  │ {currentCharacter.normalResetCount.ToString().PadLeft(9)} │ {ResetSystem.Instance.resetData.maxNormalResets.ToString().PadRight(17)}║\n";
            info += $"║ Grand Reset   │ {currentCharacter.grandResetCount.ToString().PadLeft(9)} │ {ResetSystem.Instance.resetData.maxGrandResets.ToString().PadRight(17)}║\n";
            info += $"║ Master Reset  │ {(currentCharacter.hasMasterReset ? "1" : "0").PadLeft(9)} │ 1                 ║\n";
            info += "╚═══════════════════════════════════════════════════╝\n";

            infoText.text = info;
        }

        /// <summary>
        /// Update stats display
        /// Cập nhật hiển thị chỉ số
        /// </summary>
        private void UpdateStats()
        {
            if (statsText == null || currentCharacter == null)
                return;

            string stats = "=== ACCUMULATED BONUSES ===\n\n";
            stats += $"Bonus Stat Points: +{currentCharacter.resetBonusStats:N0}\n";
            stats += $"Damage Multiplier: +{(currentCharacter.resetDamageMultiplier - 1f) * 100:F1}%\n";
            stats += $"Defense Multiplier: +{(currentCharacter.resetDefenseMultiplier - 1f) * 100:F1}%\n";
            stats += $"HP Multiplier: +{(currentCharacter.resetHPMultiplier - 1f) * 100:F1}%\n";
            stats += $"MP Multiplier: +{(currentCharacter.resetMPMultiplier - 1f) * 100:F1}%\n";

            if (currentCharacter.resetHistory != null)
            {
                stats += $"\n=== RESET POWER ===\n";
                stats += $"Total Reset Power: {currentCharacter.resetHistory.GetTotalResetPower():N0}\n";
            }

            statsText.text = stats;
        }

        /// <summary>
        /// Show reset overview
        /// Hiển thị tổng quan reset
        /// </summary>
        public void ShowResetOverview()
        {
            if (infoText == null)
                return;

            string overview = "╔═══════════════════════════════════════════════════════════════════╗\n";
            overview += "║                    RESET SYSTEM OVERVIEW                          ║\n";
            overview += "╠═══════════════════════════════════════════════════════════════════╣\n";
            overview += "║ TYPE          │ REQUIREMENT      │ REWARD                         ║\n";
            overview += "╠═══════════════════════════════════════════════════════════════════╣\n";
            overview += "║ Normal Reset  │ Level 400        │ +200-400 Stats                 ║\n";
            overview += "║ (1-100 lần)   │ 10M-210M Zen     │ +1-2.5% Damage/Defense         ║\n";
            overview += "╠═══════════════════════════════════════════════════════════════════╣\n";
            overview += "║ Grand Reset   │ 100 Normal Reset │ +5,000 Stats                   ║\n";
            overview += "║ (1-10 lần)    │ Level 400        │ +10% Damage/Defense            ║\n";
            overview += "║               │ 1 tỷ Zen         │ Special Title                  ║\n";
            overview += "╠═══════════════════════════════════════════════════════════════════╣\n";
            overview += "║ Master Reset  │ 10 Grand Reset   │ +50,000 Stats                  ║\n";
            overview += "║ (1 lần)       │ Level 400        │ +50% Damage/Defense            ║\n";
            overview += "║               │ 10 tỷ Zen        │ Master Title + Golden Name     ║\n";
            overview += "║               │ Special Item     │ Master Skills & Wings          ║\n";
            overview += "╚═══════════════════════════════════════════════════════════════════╝\n";

            infoText.text = overview;
        }
    }
}
