using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Grand Reset handler - Xử lý Grand Reset (Đại Reset)
    /// Manages grand character resets - requires 100 normal resets
    /// </summary>
    public class GrandReset : MonoBehaviour
    {
        [Header("Grand Reset Requirements")]
        [Tooltip("Required normal resets - Số reset thường yêu cầu")]
        public int requiredNormalResets = 100;

        [Tooltip("Required level - Level yêu cầu")]
        public int requiredLevel = 400;

        [Tooltip("Zen cost - Chi phí Zen")]
        public long requiredZen = 1000000000; // 1 billion

        [Header("Grand Reset Effects")]
        [Tooltip("Reset normal reset count - Reset số reset thường về 0")]
        public bool resetNormalResetCount = true;

        [Tooltip("Reset level - Level về 1")]
        public bool resetLevel = true;

        [Tooltip("Keep grand reset bonuses - Giữ bonuses từ Grand Reset trước")]
        public bool keepGrandResetBonuses = true;

        [Tooltip("Keep items - Giữ đồ")]
        public bool keepItems = true;

        [Tooltip("Keep skills - Giữ skills")]
        public bool keepSkills = true;

        [Header("Grand Reset Rewards")]
        [Tooltip("Bonus stats - Điểm stats bonus")]
        public int grandResetBonusStats = 5000;

        [Tooltip("Damage bonus - % damage bonus")]
        [Range(0f, 0.5f)]
        public float grandDamageBonus = 0.10f; // 10%

        [Tooltip("Defense bonus - % defense bonus")]
        [Range(0f, 0.5f)]
        public float grandDefenseBonus = 0.10f; // 10%

        [Tooltip("HP bonus - % HP bonus")]
        [Range(0f, 0.5f)]
        public float grandHPBonus = 0.05f; // 5%

        [Tooltip("MP bonus - % MP bonus")]
        [Range(0f, 0.5f)]
        public float grandMPBonus = 0.05f; // 5%

        [Tooltip("Grand reset title - Danh hiệu Grand Reset")]
        public string grandResetTitle = "Grand Master";

        [Header("Limits")]
        [Tooltip("Maximum grand resets - Tối đa Grand Reset")]
        public int maxGrandResets = 10;

        /// <summary>
        /// Check if character can perform grand reset
        /// Kiểm tra xem nhân vật có thể Grand Reset
        /// </summary>
        public bool CanReset(CharacterStats character)
        {
            if (character == null)
                return false;

            // Check normal reset count
            if (character.normalResetCount < requiredNormalResets)
                return false;

            // Check level
            if (character.level < requiredLevel)
                return false;

            // Check max grand resets
            if (character.grandResetCount >= maxGrandResets)
                return false;

            // Check zen
            if (character.zen < requiredZen)
                return false;

            return true;
        }

        /// <summary>
        /// Perform grand reset
        /// Thực hiện Grand Reset
        /// </summary>
        public bool PerformReset(CharacterStats character)
        {
            if (!CanReset(character))
                return false;

            // Use the main ResetSystem
            return ResetSystem.Instance.PerformGrandReset(character);
        }

        /// <summary>
        /// Get grand reset information
        /// Lấy thông tin Grand Reset
        /// </summary>
        public string GetResetInfo(CharacterStats character)
        {
            if (character == null)
                return "Invalid character";

            int nextGrandReset = character.grandResetCount + 1;

            string info = "=== GRAND RESET ===\n";
            info += $"Grand Reset Number: {nextGrandReset}\n";
            info += $"\nRequirements:\n";
            info += $"- Normal Resets: {character.normalResetCount}/{requiredNormalResets} ";
            info += character.normalResetCount >= requiredNormalResets ? "✓\n" : "✗\n";
            info += $"- Level: {character.level}/{requiredLevel} ";
            info += character.level >= requiredLevel ? "✓\n" : "✗\n";
            info += $"- Zen: {character.zen:N0}/{requiredZen:N0} ";
            info += character.zen >= requiredZen ? "✓\n" : "✗\n";
            info += $"\nRewards:\n";
            info += $"- Bonus Stats: +{grandResetBonusStats:N0}\n";
            info += $"- Damage Bonus: +{grandDamageBonus * 100:F0}%\n";
            info += $"- Defense Bonus: +{grandDefenseBonus * 100:F0}%\n";
            info += $"- HP Bonus: +{grandHPBonus * 100:F0}%\n";
            info += $"- MP Bonus: +{grandMPBonus * 100:F0}%\n";
            info += $"- Title: \"{grandResetTitle}\"\n";
            info += $"\nEffects:\n";
            info += $"- Level reset to 1\n";
            info += $"- Normal reset count reset to 0\n";
            info += $"- Keep all items and skills\n";
            info += $"\nProgress: {character.grandResetCount}/{maxGrandResets}";

            return info;
        }

        /// <summary>
        /// Get total grand reset bonuses accumulated
        /// Lấy tổng bonus Grand Reset đã tích lũy
        /// </summary>
        public void GetAccumulatedBonuses(CharacterStats character, out int totalStats, out float totalDamage, out float totalDefense)
        {
            totalStats = character.grandResetCount * grandResetBonusStats;
            totalDamage = character.grandResetCount * grandDamageBonus;
            totalDefense = character.grandResetCount * grandDefenseBonus;
        }
    }
}
