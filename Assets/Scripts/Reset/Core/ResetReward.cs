using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset reward configuration - Cấu hình phần thưởng reset
    /// Defines the rewards a character receives after resetting
    /// </summary>
    [System.Serializable]
    public class ResetReward
    {
        [Header("Stat Bonuses")]
        [Tooltip("Bonus stat points awarded - Điểm stats bonus")]
        public int BonusStatPoints = 200;

        [Header("Percentage Bonuses (Cumulative)")]
        [Tooltip("Damage bonus percentage - % tăng damage")]
        [Range(0f, 1f)]
        public float DamageBonus = 0.01f; // 1%

        [Tooltip("Defense bonus percentage - % tăng defense")]
        [Range(0f, 1f)]
        public float DefenseBonus = 0.01f; // 1%

        [Tooltip("HP bonus percentage - % tăng HP")]
        [Range(0f, 1f)]
        public float HPBonus = 0.005f; // 0.5%

        [Tooltip("MP bonus percentage - % tăng MP")]
        [Range(0f, 1f)]
        public float MPBonus = 0.005f; // 0.5%

        [Tooltip("Experience bonus percentage - % tăng EXP nhận được")]
        [Range(0f, 1f)]
        public float ExpBonus = 0.01f; // 1%

        [Tooltip("Drop rate bonus percentage - % tăng drop rate")]
        [Range(0f, 1f)]
        public float DropBonus = 0.005f; // 0.5%

        [Header("Special Rewards")]
        [Tooltip("Reward items - Items thưởng")]
        public List<Item> RewardItems = new List<Item>();

        [Tooltip("Title awarded - Danh hiệu")]
        public string Title = "";

        /// <summary>
        /// Calculate rewards based on reset count
        /// Tính phần thưởng dựa trên số lần reset
        /// </summary>
        public static ResetReward CalculateReward(int resetCount)
        {
            ResetReward reward = new ResetReward();

            // Tiered reward system based on reset count
            // Hệ thống phần thưởng phân cấp theo số lần reset
            if (resetCount >= 1 && resetCount <= 10)
            {
                // Reset 1-10: +200 stats, +1% damage/defense
                reward.BonusStatPoints = 200;
                reward.DamageBonus = 0.01f;
                reward.DefenseBonus = 0.01f;
                reward.HPBonus = 0.005f;
                reward.MPBonus = 0.005f;
            }
            else if (resetCount >= 11 && resetCount <= 30)
            {
                // Reset 11-30: +250 stats, +1.5% damage/defense
                reward.BonusStatPoints = 250;
                reward.DamageBonus = 0.015f;
                reward.DefenseBonus = 0.015f;
                reward.HPBonus = 0.0075f;
                reward.MPBonus = 0.0075f;
            }
            else if (resetCount >= 31 && resetCount <= 50)
            {
                // Reset 31-50: +300 stats, +2% damage/defense
                reward.BonusStatPoints = 300;
                reward.DamageBonus = 0.02f;
                reward.DefenseBonus = 0.02f;
                reward.HPBonus = 0.01f;
                reward.MPBonus = 0.01f;
            }
            else if (resetCount >= 51 && resetCount <= 100)
            {
                // Reset 51-100: +400 stats, +2.5% damage/defense
                reward.BonusStatPoints = 400;
                reward.DamageBonus = 0.025f;
                reward.DefenseBonus = 0.025f;
                reward.HPBonus = 0.0125f;
                reward.MPBonus = 0.0125f;
            }

            return reward;
        }

        /// <summary>
        /// Apply reward to character
        /// Áp dụng phần thưởng cho nhân vật
        /// </summary>
        public void ApplyToCharacter(CharacterStats character)
        {
            if (character == null)
                return;

            // Add bonus stat points
            character.resetBonusStats += BonusStatPoints;

            // Apply multipliers (cumulative)
            character.resetDamageMultiplier += DamageBonus;
            character.resetDefenseMultiplier += DefenseBonus;
            character.resetHPMultiplier += HPBonus;
            character.resetMPMultiplier += MPBonus;
        }
    }

    /// <summary>
    /// Placeholder CharacterStats class - replace with actual implementation
    /// Class CharacterStats tạm thời - thay thế bằng implementation thực tế
    /// </summary>
    public class CharacterStats
    {
        public int resetBonusStats;
        public float resetDamageMultiplier = 1f;
        public float resetDefenseMultiplier = 1f;
        public float resetHPMultiplier = 1f;
        public float resetMPMultiplier = 1f;
    }
}
