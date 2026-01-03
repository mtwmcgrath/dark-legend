using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset requirements - Yêu cầu để thực hiện reset
    /// Defines the conditions that must be met before a character can reset
    /// </summary>
    [System.Serializable]
    public class ResetRequirement
    {
        [Header("Level Requirements")]
        [Tooltip("Minimum level required to reset - Level tối thiểu để reset")]
        public int MinLevel = 400;

        [Header("Currency Requirements")]
        [Tooltip("Zen cost for reset - Chi phí Zen cho reset")]
        public long ZenCost = 10000000; // 10 million Zen

        [Header("Item Requirements")]
        [Tooltip("Required items for reset (optional) - Items cần thiết cho reset")]
        public List<ItemRequirement> RequiredItems = new List<ItemRequirement>();

        [Header("Reset Count Requirements")]
        [Tooltip("Minimum reset count (for Grand Reset) - Số reset tối thiểu (cho Grand Reset)")]
        public int MinResetCount = 0;

        /// <summary>
        /// Calculate Zen cost based on current reset count
        /// Tính chi phí Zen dựa trên số lần reset hiện tại
        /// Formula: Base cost + (current reset * increment)
        /// </summary>
        public long CalculateZenCost(int currentReset)
        {
            // Reset 1: 10M Zen
            // Reset 2: 12M Zen
            // Reset 3: 14M Zen
            // ... (+2M per reset)
            return 10000000 + (currentReset * 2000000);
        }

        /// <summary>
        /// Check if character meets all requirements
        /// Kiểm tra xem nhân vật có đáp ứng đủ yêu cầu không
        /// </summary>
        public bool CheckRequirements(int level, long zen, int resetCount, List<Item> inventory = null)
        {
            // Check level requirement
            if (level < MinLevel)
                return false;

            // Check zen requirement
            long requiredZen = CalculateZenCost(resetCount);
            if (zen < requiredZen)
                return false;

            // Check reset count requirement
            if (resetCount < MinResetCount)
                return false;

            // Check item requirements (if any)
            if (RequiredItems != null && RequiredItems.Count > 0)
            {
                if (inventory == null)
                    return false;

                foreach (var itemReq in RequiredItems)
                {
                    if (!HasRequiredItem(inventory, itemReq))
                        return false;
                }
            }

            return true;
        }

        private bool HasRequiredItem(List<Item> inventory, ItemRequirement requirement)
        {
            // This is a placeholder - implement based on your actual Item system
            // Đây là placeholder - implement dựa trên Item system thực tế
            return true;
        }
    }

    /// <summary>
    /// Item requirement for reset
    /// Yêu cầu item cho reset
    /// </summary>
    [System.Serializable]
    public class ItemRequirement
    {
        public string ItemId;
        public int Quantity = 1;
    }

    /// <summary>
    /// Placeholder Item class - replace with your actual Item system
    /// Class Item tạm thời - thay thế bằng Item system thực tế
    /// </summary>
    public class Item
    {
        public string Id;
        public string Name;
    }
}
