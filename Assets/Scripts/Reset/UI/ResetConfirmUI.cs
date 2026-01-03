using UnityEngine;
using UnityEngine.UI;
using System;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset confirmation UI - UI xác nhận reset
    /// Confirmation dialog before performing reset
    /// </summary>
    public class ResetConfirmUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Confirmation panel - Panel xác nhận")]
        public GameObject confirmPanel;

        [Tooltip("Message text - Text thông báo")]
        public Text messageText;

        [Tooltip("Reset info text - Text thông tin reset")]
        public Text resetInfoText;

        [Tooltip("Confirm button - Nút xác nhận")]
        public Button confirmButton;

        [Tooltip("Cancel button - Nút hủy")]
        public Button cancelButton;

        [Header("Events")]
        public event Action OnConfirmed;
        public event Action OnCancelled;

        private static ResetConfirmUI _instance;
        public static ResetConfirmUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ResetConfirmUI>();
                }
                return _instance;
            }
        }

        private CharacterStats currentCharacter;
        private ResetType currentResetType;
        private Action onConfirmCallback;

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
            if (confirmButton != null)
                confirmButton.onClick.AddListener(OnConfirmButtonClicked);

            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelButtonClicked);

            Hide();
        }

        /// <summary>
        /// Show confirmation dialog
        /// Hiển thị dialog xác nhận
        /// </summary>
        public void Show(CharacterStats character, ResetType resetType, Action onConfirm)
        {
            if (character == null)
            {
                Debug.LogWarning("Cannot show confirmation with null character");
                return;
            }

            currentCharacter = character;
            currentResetType = resetType;
            onConfirmCallback = onConfirm;

            if (confirmPanel != null)
                confirmPanel.SetActive(true);

            UpdateUI();
        }

        /// <summary>
        /// Hide confirmation dialog
        /// Ẩn dialog xác nhận
        /// </summary>
        public void Hide()
        {
            if (confirmPanel != null)
                confirmPanel.SetActive(false);

            currentCharacter = null;
            onConfirmCallback = null;
        }

        /// <summary>
        /// Update UI content
        /// Cập nhật nội dung UI
        /// </summary>
        private void UpdateUI()
        {
            UpdateMessage();
            UpdateResetInfo();
        }

        /// <summary>
        /// Update confirmation message
        /// Cập nhật thông báo xác nhận
        /// </summary>
        private void UpdateMessage()
        {
            if (messageText == null)
                return;

            string message = "⚠️ CONFIRMATION REQUIRED ⚠️\n\n";
            message += "Are you sure you want to perform this reset?\n";
            message += "This action cannot be undone!\n\n";
            message += $"Reset Type: {currentResetType}\n";

            messageText.text = message;
        }

        /// <summary>
        /// Update reset information
        /// Cập nhật thông tin reset
        /// </summary>
        private void UpdateResetInfo()
        {
            if (resetInfoText == null || currentCharacter == null)
                return;

            string info = "What will happen:\n\n";

            switch (currentResetType)
            {
                case ResetType.Normal:
                    info += "✓ Level reset to 1\n";
                    info += "✓ Receive bonus stat points\n";
                    info += "✓ Increase damage & defense\n";
                    info += "✓ Keep all items & skills\n";
                    
                    long zenCost = ResetSystem.Instance.resetData.normalResetRequirement.CalculateZenCost(currentCharacter.normalResetCount);
                    info += $"\n✗ Cost: {zenCost:N0} Zen\n";
                    break;

                case ResetType.Grand:
                    info += "✓ Level reset to 1\n";
                    info += "✓ Receive massive bonus stats\n";
                    info += "✓ Huge damage & defense boost\n";
                    info += "✓ Earn special title\n";
                    info += "✓ Keep all items & skills\n";
                    info += $"\n✗ Cost: {ResetSystem.Instance.resetData.grandResetRequirement.ZenCost:N0} Zen\n";
                    info += "✗ Normal reset count reset to 0\n";
                    break;

                case ResetType.Master:
                    info += "✓ Level reset to 1\n";
                    info += "✓ Become a Master!\n";
                    info += "✓ Extreme stat bonuses\n";
                    info += "✓ Master skills & wings\n";
                    info += "✓ Golden name color\n";
                    info += "✓ Keep all previous bonuses\n";
                    info += $"\n✗ Cost: {ResetSystem.Instance.resetData.masterResetRequirement.ZenCost:N0} Zen\n";
                    info += "✗ Can only be done once!\n";
                    break;
            }

            resetInfoText.text = info;
        }

        /// <summary>
        /// Handle confirm button click
        /// Xử lý click nút xác nhận
        /// </summary>
        private void OnConfirmButtonClicked()
        {
            onConfirmCallback?.Invoke();
            OnConfirmed?.Invoke();
            Hide();
        }

        /// <summary>
        /// Handle cancel button click
        /// Xử lý click nút hủy
        /// </summary>
        private void OnCancelButtonClicked()
        {
            OnCancelled?.Invoke();
            Hide();
        }
    }
}
