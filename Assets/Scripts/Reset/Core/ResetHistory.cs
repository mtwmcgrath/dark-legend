using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset history for a character - Lịch sử reset của nhân vật
    /// Tracks all resets performed by a character
    /// </summary>
    [System.Serializable]
    public class ResetHistory
    {
        [Header("History Entries")]
        [Tooltip("List of all reset entries - Danh sách các lần reset")]
        public List<ResetHistoryEntry> Entries = new List<ResetHistoryEntry>();

        [Header("Summary")]
        [Tooltip("Total normal resets - Tổng số reset thường")]
        public int TotalNormalResets = 0;

        [Tooltip("Total grand resets - Tổng số Grand Reset")]
        public int TotalGrandResets = 0;

        [Tooltip("Has master reset - Đã thực hiện Master Reset")]
        public bool HasMasterReset = false;

        /// <summary>
        /// Add a new reset entry to history
        /// Thêm một lần reset mới vào lịch sử
        /// </summary>
        public void AddEntry(ResetType type, int resetNumber, int levelAtReset, int rewardStats)
        {
            ResetHistoryEntry entry = new ResetHistoryEntry
            {
                Type = type,
                ResetNumber = resetNumber,
                Timestamp = DateTime.Now,
                LevelAtReset = levelAtReset,
                RewardStats = rewardStats
            };

            Entries.Add(entry);

            // Update summary
            switch (type)
            {
                case ResetType.Normal:
                    TotalNormalResets++;
                    break;
                case ResetType.Grand:
                    TotalGrandResets++;
                    break;
                case ResetType.Master:
                    HasMasterReset = true;
                    break;
            }
        }

        /// <summary>
        /// Get recent reset entries
        /// Lấy các lần reset gần đây
        /// </summary>
        public List<ResetHistoryEntry> GetRecentResets(int count = 10)
        {
            if (Entries.Count == 0)
                return new List<ResetHistoryEntry>();

            int startIndex = Mathf.Max(0, Entries.Count - count);
            return Entries.GetRange(startIndex, Entries.Count - startIndex);
        }

        /// <summary>
        /// Get total reset power (calculated value)
        /// Lấy tổng sức mạnh từ reset
        /// </summary>
        public int GetTotalResetPower()
        {
            int power = 0;
            power += TotalNormalResets * 100;
            power += TotalGrandResets * 10000;
            power += HasMasterReset ? 100000 : 0;
            return power;
        }

        /// <summary>
        /// Clear all history (use with caution)
        /// Xóa toàn bộ lịch sử (sử dụng cẩn thận)
        /// </summary>
        public void Clear()
        {
            Entries.Clear();
            TotalNormalResets = 0;
            TotalGrandResets = 0;
            HasMasterReset = false;
        }
    }

    /// <summary>
    /// Single reset history entry - Một mục lịch sử reset
    /// </summary>
    [System.Serializable]
    public class ResetHistoryEntry
    {
        [Tooltip("Type of reset - Loại reset")]
        public ResetType Type;

        [Tooltip("Reset number - Số thứ tự reset")]
        public int ResetNumber;

        [Tooltip("Timestamp of reset - Thời gian reset")]
        public DateTime Timestamp;

        [Tooltip("Level at time of reset - Level khi reset")]
        public int LevelAtReset;

        [Tooltip("Reward stats received - Điểm stats nhận được")]
        public int RewardStats;

        /// <summary>
        /// Get formatted string for display
        /// Lấy chuỗi định dạng để hiển thị
        /// </summary>
        public string GetFormattedString()
        {
            return $"[{Type}] Reset #{ResetNumber} - Level {LevelAtReset} - {Timestamp:yyyy-MM-dd HH:mm} - +{RewardStats} Stats";
        }
    }

    /// <summary>
    /// Type of reset - Loại reset
    /// </summary>
    public enum ResetType
    {
        Normal,     // Reset thường
        Grand,      // Grand Reset
        Master      // Master Reset
    }
}
