using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Damage bonus - Bonus sát thương
    /// Increases character damage output
    /// </summary>
    [System.Serializable]
    public class DamageBonus : ResetBonus
    {
        [Header("Damage Bonus Settings")]
        [Tooltip("Base damage bonus percentage - % damage cơ bản")]
        [Range(0f, 1f)]
        public float baseDamageBonus = 0.01f; // 1%

        [Tooltip("Additional bonus per tier - % thêm mỗi cấp")]
        [Range(0f, 0.1f)]
        public float tierIncrement = 0.005f; // 0.5%

        /// <summary>
        /// Calculate damage bonus for a given reset count
        /// Tính damage bonus cho số reset cho trước
        /// </summary>
        public float CalculateDamageBonus(int resetCount)
        {
            // Tiered system:
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

            return baseDamageBonus;
        }

        /// <summary>
        /// Calculate total accumulated damage bonus
        /// Tính tổng damage bonus tích lũy
        /// </summary>
        public float CalculateTotalBonus(int totalResets)
        {
            float total = 0f;

            // Calculate accumulated bonus from all resets
            for (int i = 1; i <= totalResets; i++)
            {
                total += CalculateDamageBonus(i);
            }

            return total;
        }

        public override void Apply(CharacterStats character, int resetCount)
        {
            if (character == null)
                return;

            float damageBonus = CalculateDamageBonus(resetCount);
            character.resetDamageMultiplier += damageBonus;

            Debug.Log($"Applied {damageBonus * 100:F1}% damage bonus to {character.name}. Total: {(character.resetDamageMultiplier - 1f) * 100:F1}%");
        }

        public override void Remove(CharacterStats character)
        {
            if (character == null)
                return;

            Debug.LogWarning("Removing individual damage bonuses is not supported");
        }

        public override string GetBonusString(int resetCount)
        {
            float damageBonus = CalculateDamageBonus(resetCount);
            return $"+{damageBonus * 100:F1}% Damage";
        }

        /// <summary>
        /// Get formatted string with total bonus
        /// Lấy chuỗi định dạng với tổng bonus
        /// </summary>
        public string GetTotalBonusString(CharacterStats character)
        {
            if (character == null)
                return "+0% Damage";

            float totalBonus = (character.resetDamageMultiplier - 1f) * 100f;
            return $"+{totalBonus:F1}% Total Damage";
        }
    }
}
