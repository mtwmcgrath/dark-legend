using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Stat point bonus - Bonus điểm stats
    /// Grants additional stat points that can be distributed
    /// </summary>
    [System.Serializable]
    public class StatPointBonus : ResetBonus
    {
        [Header("Stat Point Settings")]
        [Tooltip("Base stat points per reset - Điểm stats cơ bản mỗi reset")]
        public int baseStatPoints = 200;

        [Tooltip("Additional points per tier - Điểm thêm mỗi cấp")]
        public int tierIncrement = 50;

        [Tooltip("Resets per tier - Số reset mỗi cấp")]
        public int resetsPerTier = 10;

        /// <summary>
        /// Calculate stat points for a given reset count
        /// Tính điểm stats cho số reset cho trước
        /// </summary>
        public int CalculateStatPoints(int resetCount)
        {
            // Tiered system: 
            // Reset 1-10: 200 points
            // Reset 11-30: 250 points
            // Reset 31-50: 300 points
            // Reset 51+: 400 points

            if (resetCount >= 1 && resetCount <= 10)
                return 200;
            else if (resetCount >= 11 && resetCount <= 30)
                return 250;
            else if (resetCount >= 31 && resetCount <= 50)
                return 300;
            else if (resetCount >= 51)
                return 400;

            return baseStatPoints;
        }

        public override void Apply(CharacterStats character, int resetCount)
        {
            if (character == null)
                return;

            int statPoints = CalculateStatPoints(resetCount);
            character.resetBonusStats += statPoints;

            Debug.Log($"Applied {statPoints} bonus stat points to {character.name}");
        }

        public override void Remove(CharacterStats character)
        {
            if (character == null)
                return;

            // This would require tracking individual bonus applications
            // For now, we don't support removing individual bonuses
            Debug.LogWarning("Removing individual stat bonuses is not supported");
        }

        public override string GetBonusString(int resetCount)
        {
            int statPoints = CalculateStatPoints(resetCount);
            return $"+{statPoints} Stat Points";
        }
    }
}
