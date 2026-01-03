using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Defense bonus - Bonus phòng thủ
    /// Increases character defense
    /// </summary>
    [System.Serializable]
    public class DefenseBonus : ResetBonus
    {
        [Header("Defense Bonus Settings")]
        [Tooltip("Base defense bonus percentage - % defense cơ bản")]
        [Range(0f, 1f)]
        public float baseDefenseBonus = 0.01f; // 1%

        [Tooltip("Additional bonus per tier - % thêm mỗi cấp")]
        [Range(0f, 0.1f)]
        public float tierIncrement = 0.005f; // 0.5%

        /// <summary>
        /// Calculate defense bonus for a given reset count
        /// Tính defense bonus cho số reset cho trước
        /// </summary>
        public float CalculateDefenseBonus(int resetCount)
        {
            // Tiered system matching damage bonus:
            // Reset 1-10: 1% per reset
            // Reset 11-30: 1.5% per reset
            // Reset 31-50: 2% per reset
            // Reset 51-100: 2.5% per reset

            if (resetCount >= 1 && resetCount <= 10)
                return 0.01f;
            else if (resetCount >= 11 && resetCount <= 30)
                return 0.015f;
            else if (resetCount >= 31 && resetCount <= 50)
                return 0.02f;
            else if (resetCount >= 51 && resetCount <= 100)
                return 0.025f;

            return baseDefenseBonus;
        }

        /// <summary>
        /// Calculate total accumulated defense bonus
        /// Tính tổng defense bonus tích lũy
        /// </summary>
        public float CalculateTotalBonus(int totalResets)
        {
            float total = 0f;

            // Calculate accumulated bonus from all resets
            for (int i = 1; i <= totalResets; i++)
            {
                total += CalculateDefenseBonus(i);
            }

            return total;
        }

        public override void Apply(CharacterStats character, int resetCount)
        {
            if (character == null)
                return;

            float defenseBonus = CalculateDefenseBonus(resetCount);
            character.resetDefenseMultiplier += defenseBonus;

            Debug.Log($"Applied {defenseBonus * 100:F1}% defense bonus to {character.name}. Total: {(character.resetDefenseMultiplier - 1f) * 100:F1}%");
        }

        public override void Remove(CharacterStats character)
        {
            if (character == null)
                return;

            Debug.LogWarning("Removing individual defense bonuses is not supported");
        }

        public override string GetBonusString(int resetCount)
        {
            float defenseBonus = CalculateDefenseBonus(resetCount);
            return $"+{defenseBonus * 100:F1}% Defense";
        }

        /// <summary>
        /// Get formatted string with total bonus
        /// Lấy chuỗi định dạng với tổng bonus
        /// </summary>
        public string GetTotalBonusString(CharacterStats character)
        {
            if (character == null)
                return "+0% Defense";

            float totalBonus = (character.resetDefenseMultiplier - 1f) * 100f;
            return $"+{totalBonus:F1}% Total Defense";
        }
    }
}
