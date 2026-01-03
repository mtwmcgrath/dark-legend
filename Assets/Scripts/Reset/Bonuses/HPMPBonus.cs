using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// HP and MP bonus - Bonus HP và MP
    /// Increases character HP and MP pools
    /// </summary>
    [System.Serializable]
    public class HPMPBonus : ResetBonus
    {
        [Header("HP Bonus Settings")]
        [Tooltip("Base HP bonus percentage - % HP cơ bản")]
        [Range(0f, 0.5f)]
        public float baseHPBonus = 0.005f; // 0.5%

        [Header("MP Bonus Settings")]
        [Tooltip("Base MP bonus percentage - % MP cơ bản")]
        [Range(0f, 0.5f)]
        public float baseMPBonus = 0.005f; // 0.5%

        /// <summary>
        /// Calculate HP bonus for a given reset count
        /// Tính HP bonus cho số reset cho trước
        /// </summary>
        public float CalculateHPBonus(int resetCount)
        {
            // Tiered system:
            // Reset 1-10: 0.5% per reset
            // Reset 11-30: 0.75% per reset
            // Reset 31-50: 1% per reset
            // Reset 51-100: 1.25% per reset

            if (resetCount >= 1 && resetCount <= 10)
                return 0.005f;
            else if (resetCount >= 11 && resetCount <= 30)
                return 0.0075f;
            else if (resetCount >= 31 && resetCount <= 50)
                return 0.01f;
            else if (resetCount >= 51 && resetCount <= 100)
                return 0.0125f;

            return baseHPBonus;
        }

        /// <summary>
        /// Calculate MP bonus for a given reset count
        /// Tính MP bonus cho số reset cho trước
        /// </summary>
        public float CalculateMPBonus(int resetCount)
        {
            // Same as HP bonus
            return CalculateHPBonus(resetCount);
        }

        /// <summary>
        /// Calculate total accumulated HP bonus
        /// Tính tổng HP bonus tích lũy
        /// </summary>
        public float CalculateTotalHPBonus(int totalResets)
        {
            float total = 0f;

            for (int i = 1; i <= totalResets; i++)
            {
                total += CalculateHPBonus(i);
            }

            return total;
        }

        /// <summary>
        /// Calculate total accumulated MP bonus
        /// Tính tổng MP bonus tích lũy
        /// </summary>
        public float CalculateTotalMPBonus(int totalResets)
        {
            float total = 0f;

            for (int i = 1; i <= totalResets; i++)
            {
                total += CalculateMPBonus(i);
            }

            return total;
        }

        public override void Apply(CharacterStats character, int resetCount)
        {
            if (character == null)
                return;

            float hpBonus = CalculateHPBonus(resetCount);
            float mpBonus = CalculateMPBonus(resetCount);

            character.resetHPMultiplier += hpBonus;
            character.resetMPMultiplier += mpBonus;

            Debug.Log($"Applied HP/MP bonus to {character.name}: +{hpBonus * 100:F2}% HP, +{mpBonus * 100:F2}% MP");
            Debug.Log($"Total bonuses: +{(character.resetHPMultiplier - 1f) * 100:F1}% HP, +{(character.resetMPMultiplier - 1f) * 100:F1}% MP");
        }

        public override void Remove(CharacterStats character)
        {
            if (character == null)
                return;

            Debug.LogWarning("Removing individual HP/MP bonuses is not supported");
        }

        public override string GetBonusString(int resetCount)
        {
            float hpBonus = CalculateHPBonus(resetCount);
            float mpBonus = CalculateMPBonus(resetCount);
            return $"+{hpBonus * 100:F2}% HP, +{mpBonus * 100:F2}% MP";
        }

        /// <summary>
        /// Get formatted string with total bonuses
        /// Lấy chuỗi định dạng với tổng bonus
        /// </summary>
        public string GetTotalBonusString(CharacterStats character)
        {
            if (character == null)
                return "+0% HP, +0% MP";

            float totalHP = (character.resetHPMultiplier - 1f) * 100f;
            float totalMP = (character.resetMPMultiplier - 1f) * 100f;
            return $"+{totalHP:F1}% Total HP, +{totalMP:F1}% Total MP";
        }
    }
}
