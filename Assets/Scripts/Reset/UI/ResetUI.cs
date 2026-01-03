using UnityEngine;
using UnityEngine.UI;
using System;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Main Reset UI - UI chính hệ thống reset
    /// Main user interface for the reset system
    /// </summary>
    public class ResetUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Main panel - Panel chính")]
        public GameObject mainPanel;

        [Tooltip("Character info text - Text thông tin nhân vật")]
        public Text characterInfoText;

        [Tooltip("Reset type dropdown - Dropdown chọn loại reset")]
        public Dropdown resetTypeDropdown;

        [Tooltip("Requirements panel - Panel yêu cầu")]
        public GameObject requirementsPanel;

        [Tooltip("Requirements text - Text yêu cầu")]
        public Text requirementsText;

        [Tooltip("Rewards panel - Panel phần thưởng")]
        public GameObject rewardsPanel;

        [Tooltip("Rewards text - Text phần thưởng")]
        public Text rewardsText;

        [Tooltip("Warning text - Text cảnh báo")]
        public Text warningText;

        [Tooltip("Reset button - Nút reset")]
        public Button resetButton;

        [Tooltip("Cancel button - Nút hủy")]
        public Button cancelButton;

        [Header("Settings")]
        [Tooltip("Show confirmation dialog - Hiển thị dialog xác nhận")]
        public bool showConfirmation = true;

        [Header("Events")]
        public event Action<ResetType> OnResetRequested;
        public event Action OnUIClosed;

        private static ResetUI _instance;
        public static ResetUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ResetUI>();
                }
                return _instance;
            }
        }

        private CharacterStats currentCharacter;
        private ResetType selectedResetType = ResetType.Normal;

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
            // Setup button listeners
            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetButtonClicked);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelButtonClicked);

            // Setup dropdown
            if (resetTypeDropdown != null)
            {
                resetTypeDropdown.ClearOptions();
                resetTypeDropdown.AddOptions(new System.Collections.Generic.List<string> 
                { 
                    "Normal Reset", 
                    "Grand Reset", 
                    "Master Reset" 
                });
                resetTypeDropdown.onValueChanged.AddListener(OnResetTypeChanged);
            }

            // Hide by default
            Hide();
        }

        /// <summary>
        /// Show UI for character
        /// Hiển thị UI cho nhân vật
        /// </summary>
        public void Show(CharacterStats character)
        {
            if (character == null)
            {
                Debug.LogWarning("Cannot show Reset UI with null character");
                return;
            }

            currentCharacter = character;
            
            if (mainPanel != null)
                mainPanel.SetActive(true);

            UpdateUI();
        }

        /// <summary>
        /// Hide UI
        /// Ẩn UI
        /// </summary>
        public void Hide()
        {
            if (mainPanel != null)
                mainPanel.SetActive(false);

            currentCharacter = null;
        }

        /// <summary>
        /// Update all UI elements
        /// Cập nhật tất cả các thành phần UI
        /// </summary>
        private void UpdateUI()
        {
            if (currentCharacter == null)
                return;

            UpdateCharacterInfo();
            UpdateRequirements();
            UpdateRewards();
            UpdateWarning();
            UpdateResetButton();
        }

        /// <summary>
        /// Update character information display
        /// Cập nhật hiển thị thông tin nhân vật
        /// </summary>
        private void UpdateCharacterInfo()
        {
            if (characterInfoText == null || currentCharacter == null)
                return;

            string info = $"Character: {currentCharacter.name}\n";
            info += $"Level: {currentCharacter.level}\n";
            info += $"Normal Resets: {currentCharacter.normalResetCount}\n";
            info += $"Grand Resets: {currentCharacter.grandResetCount}\n";
            info += $"Master Reset: {(currentCharacter.hasMasterReset ? "Yes" : "No")}\n";

            characterInfoText.text = info;
        }

        /// <summary>
        /// Update requirements display
        /// Cập nhật hiển thị yêu cầu
        /// </summary>
        private void UpdateRequirements()
        {
            if (requirementsText == null || currentCharacter == null)
                return;

            string requirements = "";
            bool canReset = false;
            string reason = "";

            switch (selectedResetType)
            {
                case ResetType.Normal:
                    canReset = ResetSystem.Instance.CanPerformNormalReset(currentCharacter, out reason);
                    long normalZenCost = ResetSystem.Instance.resetData.normalResetRequirement.CalculateZenCost(currentCharacter.normalResetCount);
                    requirements += $"Level Required: {ResetSystem.Instance.resetData.normalResetRequirement.MinLevel}\n";
                    requirements += $"Zen Cost: {normalZenCost:N0}\n";
                    requirements += $"Your Zen: {currentCharacter.zen:N0}\n";
                    break;

                case ResetType.Grand:
                    canReset = ResetSystem.Instance.CanPerformGrandReset(currentCharacter, out reason);
                    requirements += $"Normal Resets Required: {ResetSystem.Instance.resetData.grandResetRequirement.MinResetCount}\n";
                    requirements += $"Your Normal Resets: {currentCharacter.normalResetCount}\n";
                    requirements += $"Level Required: {ResetSystem.Instance.resetData.grandResetRequirement.MinLevel}\n";
                    requirements += $"Zen Cost: {ResetSystem.Instance.resetData.grandResetRequirement.ZenCost:N0}\n";
                    requirements += $"Your Zen: {currentCharacter.zen:N0}\n";
                    break;

                case ResetType.Master:
                    canReset = ResetSystem.Instance.CanPerformMasterReset(currentCharacter, out reason);
                    requirements += $"Grand Resets Required: {ResetSystem.Instance.resetData.masterResetRequirement.MinResetCount}\n";
                    requirements += $"Your Grand Resets: {currentCharacter.grandResetCount}\n";
                    requirements += $"Level Required: {ResetSystem.Instance.resetData.masterResetRequirement.MinLevel}\n";
                    requirements += $"Zen Cost: {ResetSystem.Instance.resetData.masterResetRequirement.ZenCost:N0}\n";
                    requirements += $"Your Zen: {currentCharacter.zen:N0}\n";
                    break;
            }

            requirements += $"\nStatus: {(canReset ? "✓ Ready to Reset!" : $"✗ {reason}")}";
            requirementsText.text = requirements;
        }

        /// <summary>
        /// Update rewards display
        /// Cập nhật hiển thị phần thưởng
        /// </summary>
        private void UpdateRewards()
        {
            if (rewardsText == null || currentCharacter == null)
                return;

            string rewards = "";

            switch (selectedResetType)
            {
                case ResetType.Normal:
                    ResetReward normalReward = ResetReward.CalculateReward(currentCharacter.normalResetCount + 1);
                    rewards += $"Bonus Stats: +{normalReward.BonusStatPoints}\n";
                    rewards += $"Damage Bonus: +{normalReward.DamageBonus * 100:F1}%\n";
                    rewards += $"Defense Bonus: +{normalReward.DefenseBonus * 100:F1}%\n";
                    rewards += $"HP Bonus: +{normalReward.HPBonus * 100:F1}%\n";
                    rewards += $"MP Bonus: +{normalReward.MPBonus * 100:F1}%\n";
                    
                    float totalDamage = (currentCharacter.resetDamageMultiplier - 1f + normalReward.DamageBonus) * 100f;
                    float totalDefense = (currentCharacter.resetDefenseMultiplier - 1f + normalReward.DefenseBonus) * 100f;
                    rewards += $"\nTotal After Reset:\n";
                    rewards += $"Damage: +{totalDamage:F1}%\n";
                    rewards += $"Defense: +{totalDefense:F1}%\n";
                    break;

                case ResetType.Grand:
                    rewards += $"Bonus Stats: +{ResetSystem.Instance.resetData.grandResetBonusStats:N0}\n";
                    rewards += $"Damage Bonus: +{ResetSystem.Instance.resetData.grandDamageBonus * 100:F0}%\n";
                    rewards += $"Defense Bonus: +{ResetSystem.Instance.resetData.grandDefenseBonus * 100:F0}%\n";
                    rewards += $"HP Bonus: +{ResetSystem.Instance.resetData.grandHPBonus * 100:F0}%\n";
                    rewards += $"Title: {ResetSystem.Instance.resetData.grandResetTitle}\n";
                    break;

                case ResetType.Master:
                    rewards += $"Bonus Stats: +{ResetSystem.Instance.resetData.masterBonusStats:N0}\n";
                    rewards += $"Damage Bonus: +{ResetSystem.Instance.resetData.masterDamageBonus * 100:F0}%\n";
                    rewards += $"Defense Bonus: +{ResetSystem.Instance.resetData.masterDefenseBonus * 100:F0}%\n";
                    rewards += $"Title: {ResetSystem.Instance.resetData.masterTitle}\n";
                    rewards += $"Special: Master Skills & Wings\n";
                    rewards += $"Special: Golden Name Color\n";
                    break;
            }

            rewardsText.text = rewards;
        }

        /// <summary>
        /// Update warning display
        /// Cập nhật hiển thị cảnh báo
        /// </summary>
        private void UpdateWarning()
        {
            if (warningText == null)
                return;

            string warning = "⚠️ WARNING:\n";
            warning += "- Level will be reset to 1\n";
            warning += "- Items and Skills will be kept\n";

            if (ResetSystem.Instance.resetData.keepStats)
                warning += "- Stat points will be kept\n";
            else
                warning += "- Stat points will be reset (but bonuses kept)\n";

            if (selectedResetType == ResetType.Grand)
                warning += "- Normal reset count will be reset to 0\n";

            warningText.text = warning;
        }

        /// <summary>
        /// Update reset button state
        /// Cập nhật trạng thái nút reset
        /// </summary>
        private void UpdateResetButton()
        {
            if (resetButton == null || currentCharacter == null)
                return;

            bool canReset = false;

            switch (selectedResetType)
            {
                case ResetType.Normal:
                    canReset = ResetSystem.Instance.CanPerformNormalReset(currentCharacter, out _);
                    break;
                case ResetType.Grand:
                    canReset = ResetSystem.Instance.CanPerformGrandReset(currentCharacter, out _);
                    break;
                case ResetType.Master:
                    canReset = ResetSystem.Instance.CanPerformMasterReset(currentCharacter, out _);
                    break;
            }

            resetButton.interactable = canReset;
        }

        /// <summary>
        /// Handle reset type dropdown change
        /// Xử lý thay đổi dropdown loại reset
        /// </summary>
        private void OnResetTypeChanged(int value)
        {
            selectedResetType = (ResetType)value;
            UpdateUI();
        }

        /// <summary>
        /// Handle reset button click
        /// Xử lý click nút reset
        /// </summary>
        private void OnResetButtonClicked()
        {
            if (currentCharacter == null)
                return;

            if (showConfirmation)
            {
                // Show confirmation dialog
                ResetConfirmUI.Instance?.Show(currentCharacter, selectedResetType, PerformReset);
            }
            else
            {
                PerformReset();
            }
        }

        /// <summary>
        /// Perform the reset
        /// Thực hiện reset
        /// </summary>
        private void PerformReset()
        {
            if (currentCharacter == null)
                return;

            bool success = false;

            switch (selectedResetType)
            {
                case ResetType.Normal:
                    success = ResetSystem.Instance.PerformNormalReset(currentCharacter);
                    break;
                case ResetType.Grand:
                    success = ResetSystem.Instance.PerformGrandReset(currentCharacter);
                    break;
                case ResetType.Master:
                    success = ResetSystem.Instance.PerformMasterReset(currentCharacter);
                    break;
            }

            if (success)
            {
                Debug.Log($"Reset successful! Type: {selectedResetType}");
                UpdateUI();
                OnResetRequested?.Invoke(selectedResetType);
            }
            else
            {
                Debug.LogWarning($"Reset failed! Type: {selectedResetType}");
            }
        }

        /// <summary>
        /// Handle cancel button click
        /// Xử lý click nút hủy
        /// </summary>
        private void OnCancelButtonClicked()
        {
            Hide();
            OnUIClosed?.Invoke();
        }
    }
}
