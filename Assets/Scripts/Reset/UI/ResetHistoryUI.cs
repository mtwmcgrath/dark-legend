using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset history UI - UI lịch sử reset
    /// Displays character's reset history
    /// </summary>
    public class ResetHistoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("History panel - Panel lịch sử")]
        public GameObject historyPanel;

        [Tooltip("Title text - Text tiêu đề")]
        public Text titleText;

        [Tooltip("History list - Danh sách lịch sử")]
        public Transform historyListContent;

        [Tooltip("History entry prefab - Prefab mục lịch sử")]
        public GameObject historyEntryPrefab;

        [Tooltip("Summary text - Text tổng kết")]
        public Text summaryText;

        [Tooltip("Close button - Nút đóng")]
        public Button closeButton;

        [Header("Settings")]
        [Tooltip("Max entries to display - Số mục hiển thị tối đa")]
        public int maxEntriesToDisplay = 50;

        private static ResetHistoryUI _instance;
        public static ResetHistoryUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ResetHistoryUI>();
                }
                return _instance;
            }
        }

        private CharacterStats currentCharacter;
        private List<GameObject> entryObjects = new List<GameObject>();

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
        /// Show history for character
        /// Hiển thị lịch sử cho nhân vật
        /// </summary>
        public void Show(CharacterStats character)
        {
            if (character == null)
            {
                Debug.LogWarning("Cannot show history with null character");
                return;
            }

            currentCharacter = character;

            if (historyPanel != null)
                historyPanel.SetActive(true);

            UpdateUI();
        }

        /// <summary>
        /// Hide history panel
        /// Ẩn panel lịch sử
        /// </summary>
        public void Hide()
        {
            if (historyPanel != null)
                historyPanel.SetActive(false);

            ClearHistoryList();
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
            UpdateHistoryList();
            UpdateSummary();
        }

        /// <summary>
        /// Update title
        /// Cập nhật tiêu đề
        /// </summary>
        private void UpdateTitle()
        {
            if (titleText == null)
                return;

            string title = $"Reset History - {currentCharacter.name}";
            titleText.text = title;
        }

        /// <summary>
        /// Update history list
        /// Cập nhật danh sách lịch sử
        /// </summary>
        private void UpdateHistoryList()
        {
            ClearHistoryList();

            if (currentCharacter.resetHistory == null || currentCharacter.resetHistory.Entries.Count == 0)
            {
                CreateNoHistoryEntry();
                return;
            }

            // Get recent entries (reversed to show newest first)
            var entries = currentCharacter.resetHistory.Entries;
            int startIndex = Mathf.Max(0, entries.Count - maxEntriesToDisplay);
            
            for (int i = entries.Count - 1; i >= startIndex; i--)
            {
                CreateHistoryEntry(entries[i]);
            }
        }

        /// <summary>
        /// Create a history entry in the list
        /// Tạo một mục lịch sử trong danh sách
        /// </summary>
        private void CreateHistoryEntry(ResetHistoryEntry entry)
        {
            if (historyListContent == null)
                return;

            GameObject entryObj;

            if (historyEntryPrefab != null)
            {
                entryObj = Instantiate(historyEntryPrefab, historyListContent);
            }
            else
            {
                // Create simple text entry if no prefab
                entryObj = new GameObject("HistoryEntry");
                entryObj.transform.SetParent(historyListContent);
                Text text = entryObj.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 14;
                text.text = entry.GetFormattedString();
            }

            // If prefab has a Text component, set its content
            Text entryText = entryObj.GetComponentInChildren<Text>();
            if (entryText != null)
            {
                entryText.text = FormatHistoryEntry(entry);
            }

            entryObjects.Add(entryObj);
        }

        /// <summary>
        /// Format history entry for display
        /// Định dạng mục lịch sử để hiển thị
        /// </summary>
        private string FormatHistoryEntry(ResetHistoryEntry entry)
        {
            string icon = GetResetTypeIcon(entry.Type);
            string color = GetResetTypeColor(entry.Type);
            
            string formatted = $"{icon} {entry.Type} Reset #{entry.ResetNumber}\n";
            formatted += $"   Date: {entry.Timestamp:yyyy-MM-dd HH:mm:ss}\n";
            formatted += $"   Level: {entry.LevelAtReset}\n";
            formatted += $"   Reward: +{entry.RewardStats:N0} Stats\n";

            return formatted;
        }

        /// <summary>
        /// Get icon for reset type
        /// Lấy icon cho loại reset
        /// </summary>
        private string GetResetTypeIcon(ResetType type)
        {
            switch (type)
            {
                case ResetType.Normal:
                    return "⚔️";
                case ResetType.Grand:
                    return "👑";
                case ResetType.Master:
                    return "⭐";
                default:
                    return "•";
            }
        }

        /// <summary>
        /// Get color for reset type
        /// Lấy màu cho loại reset
        /// </summary>
        private string GetResetTypeColor(ResetType type)
        {
            switch (type)
            {
                case ResetType.Normal:
                    return "#FFFFFF"; // White
                case ResetType.Grand:
                    return "#FFD700"; // Gold
                case ResetType.Master:
                    return "#FF6B00"; // Orange
                default:
                    return "#FFFFFF";
            }
        }

        /// <summary>
        /// Create entry when no history exists
        /// Tạo mục khi không có lịch sử
        /// </summary>
        private void CreateNoHistoryEntry()
        {
            if (historyListContent == null)
                return;

            GameObject entryObj = new GameObject("NoHistoryEntry");
            entryObj.transform.SetParent(historyListContent);
            Text text = entryObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = "No reset history yet.\nPerform your first reset to start your journey!";

            entryObjects.Add(entryObj);
        }

        /// <summary>
        /// Clear history list
        /// Xóa danh sách lịch sử
        /// </summary>
        private void ClearHistoryList()
        {
            foreach (GameObject obj in entryObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            entryObjects.Clear();
        }

        /// <summary>
        /// Update summary
        /// Cập nhật tổng kết
        /// </summary>
        private void UpdateSummary()
        {
            if (summaryText == null || currentCharacter == null)
                return;

            string summary = "=== RESET SUMMARY ===\n\n";

            if (currentCharacter.resetHistory != null)
            {
                summary += $"Total Normal Resets: {currentCharacter.resetHistory.TotalNormalResets}\n";
                summary += $"Total Grand Resets: {currentCharacter.resetHistory.TotalGrandResets}\n";
                summary += $"Master Reset: {(currentCharacter.resetHistory.HasMasterReset ? "Yes" : "No")}\n";
                summary += $"\nTotal Entries: {currentCharacter.resetHistory.Entries.Count}\n";
                summary += $"Reset Power: {currentCharacter.resetHistory.GetTotalResetPower():N0}\n";
            }
            else
            {
                summary += "No reset history available.\n";
            }

            summaryText.text = summary;
        }

        /// <summary>
        /// Export history to string
        /// Xuất lịch sử ra chuỗi
        /// </summary>
        public string ExportHistoryToString()
        {
            if (currentCharacter == null || currentCharacter.resetHistory == null)
                return "No history to export";

            string export = $"=== RESET HISTORY FOR {currentCharacter.name} ===\n\n";

            foreach (var entry in currentCharacter.resetHistory.Entries)
            {
                export += entry.GetFormattedString() + "\n";
            }

            export += $"\n=== SUMMARY ===\n";
            export += $"Total Normal Resets: {currentCharacter.resetHistory.TotalNormalResets}\n";
            export += $"Total Grand Resets: {currentCharacter.resetHistory.TotalGrandResets}\n";
            export += $"Master Reset: {(currentCharacter.resetHistory.HasMasterReset ? "Yes" : "No")}\n";

            return export;
        }
    }
}
