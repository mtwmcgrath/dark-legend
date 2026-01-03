using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Normal Reset handler - Xử lý Reset thường
    /// Manages normal character resets
    /// </summary>
    public class NormalReset : MonoBehaviour
    {
        [Header("Normal Reset Settings")]
        [Tooltip("Required level for reset - Level yêu cầu")]
        public int requiredLevel = 400;

        [Tooltip("Base zen cost - Chi phí Zen cơ bản")]
        public long baseZenCost = 10000000; // 10 million

        [Tooltip("Zen increase per reset - Tăng Zen mỗi reset")]
        public long zenIncrement = 2000000; // 2 million

        [Header("Reset Effects")]
        [Tooltip("Level after reset - Level sau reset")]
        public int levelAfterReset = 1;

        [Tooltip("Keep stat points - Giữ điểm stats")]
        public bool keepStats = false;

        [Tooltip("Keep items - Giữ đồ")]
        public bool keepItems = true;

        [Tooltip("Keep skills - Giữ skills")]
        public bool keepSkills = true;

        [Tooltip("Keep zen - Giữ tiền")]
        public bool keepZen = true;

        [Header("Rewards")]
        [Tooltip("Bonus stat points per reset - Điểm stats bonus mỗi reset")]
        public int bonusStatPoints = 200;

        [Tooltip("Damage multiplier increase - Tăng damage %")]
        [Range(0f, 0.1f)]
        public float damageMultiplier = 0.01f; // 1%

        [Tooltip("Defense multiplier increase - Tăng defense %")]
        [Range(0f, 0.1f)]
        public float defenseMultiplier = 0.01f; // 1%

        [Tooltip("HP multiplier increase - Tăng HP %")]
        [Range(0f, 0.1f)]
        public float hpMultiplier = 0.005f; // 0.5%

        [Tooltip("MP multiplier increase - Tăng MP %")]
        [Range(0f, 0.1f)]
        public float mpMultiplier = 0.005f; // 0.5%

        [Header("Limits")]
        [Tooltip("Maximum normal resets - Tối đa reset thường")]
        public int maxNormalResets = 100;

        /// <summary>
        /// Check if character can reset
        /// Kiểm tra xem nhân vật có thể reset
        /// </summary>
        public bool CanReset(CharacterStats character)
        {
            if (character == null)
                return false;

            // Check level
            if (character.level < requiredLevel)
                return false;

            // Check max resets
            if (character.normalResetCount >= maxNormalResets)
                return false;

            // Check zen
            long zenCost = CalculateZenCost(character.normalResetCount);
            if (character.zen < zenCost)
                return false;

            return true;
        }

        /// <summary>
        /// Calculate zen cost based on current reset count
        /// Tính chi phí Zen dựa trên số reset hiện tại
        /// </summary>
        public long CalculateZenCost(int currentResets)
        {
            return baseZenCost + (currentResets * zenIncrement);
        }

        /// <summary>
        /// Calculate rewards for next reset
        /// Tính phần thưởng cho lần reset tiếp theo
        /// </summary>
        public ResetReward CalculateRewards(int resetCount)
        {
            // Use tiered reward system
            return ResetReward.CalculateReward(resetCount);
        }

        /// <summary>
        /// Perform the reset operation
        /// Thực hiện thao tác reset
        /// </summary>
        public bool PerformReset(CharacterStats character)
        {
            if (!CanReset(character))
                return false;

            // Use the main ResetSystem
            return ResetSystem.Instance.PerformNormalReset(character);
        }

        /// <summary>
        /// Get reset information
        /// Lấy thông tin reset
        /// </summary>
        public string GetResetInfo(CharacterStats character)
        {
            if (character == null)
                return "Invalid character";

            int nextReset = character.normalResetCount + 1;
            long zenCost = CalculateZenCost(character.normalResetCount);
            ResetReward reward = CalculateRewards(nextReset);

            string info = "=== NORMAL RESET ===\n";
            info += $"Reset Number: {nextReset}\n";
            info += $"Required Level: {requiredLevel}\n";
            info += $"Current Level: {character.level}\n";
            info += $"Zen Cost: {zenCost:N0}\n";
            info += $"Your Zen: {character.zen:N0}\n";
            info += $"\nRewards:\n";
            info += $"- Bonus Stats: +{reward.BonusStatPoints}\n";
            info += $"- Damage Bonus: +{reward.DamageBonus * 100:F1}%\n";
            info += $"- Defense Bonus: +{reward.DefenseBonus * 100:F1}%\n";
            info += $"- HP Bonus: +{reward.HPBonus * 100:F1}%\n";
            info += $"- MP Bonus: +{reward.MPBonus * 100:F1}%\n";
            info += $"\nProgress: {character.normalResetCount}/{maxNormalResets}";

            return info;
        }
    }
}
