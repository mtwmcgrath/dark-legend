using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Drop rate bonus - Bonus tỷ lệ rơi đồ
    /// Increases item drop rates from monsters
    /// </summary>
    [System.Serializable]
    public class DropRateBonus : ResetBonus
    {
        [Header("Drop Rate Settings")]
        [Tooltip("Base drop rate bonus percentage - % drop rate cơ bản")]
        [Range(0f, 1f)]
        public float baseDropRateBonus = 0.005f; // 0.5%

        [Tooltip("Maximum drop rate bonus - % drop rate tối đa")]
        [Range(0f, 2f)]
        public float maxDropRateBonus = 0.5f; // 50%

        [Tooltip("Apply to rare items - Áp dụng cho đồ hiếm")]
        public bool applyToRareItems = true;

        [Tooltip("Apply to normal items - Áp dụng cho đồ thường")]
        public bool applyToNormalItems = true;

        /// <summary>
        /// Calculate drop rate bonus for a given reset count
        /// Tính drop rate bonus cho số reset cho trước
        /// </summary>
        public float CalculateDropRateBonus(int resetCount)
        {
            // Each reset gives 0.5% bonus, capped at 50%
            float bonus = resetCount * baseDropRateBonus;
            return Mathf.Min(bonus, maxDropRateBonus);
        }

        /// <summary>
        /// Calculate total accumulated drop rate bonus
        /// Tính tổng drop rate bonus tích lũy
        /// </summary>
        public float CalculateTotalBonus(int totalResets)
        {
            return CalculateDropRateBonus(totalResets);
        }

        public override void Apply(CharacterStats character, int resetCount)
        {
            if (character == null)
                return;

            // Note: This requires a drop rate system in CharacterStats
            // For now, we'll just log it
            float dropBonus = CalculateDropRateBonus(resetCount);
            
            Debug.Log($"Applied {dropBonus * 100:F1}% drop rate bonus to {character.name}");
        }

        public override void Remove(CharacterStats character)
        {
            if (character == null)
                return;

            Debug.LogWarning("Removing individual drop rate bonuses is not supported");
        }

        public override string GetBonusString(int resetCount)
        {
            float dropBonus = CalculateDropRateBonus(resetCount);
            return $"+{dropBonus * 100:F1}% Drop Rate";
        }

        /// <summary>
        /// Apply drop rate to a base drop chance
        /// Áp dụng drop rate bonus vào tỷ lệ rơi cơ bản
        /// </summary>
        public float ApplyDropBonus(float baseDropChance, int resetCount, bool isRareItem = false)
        {
            // Check if bonus applies to this item type
            if (isRareItem && !applyToRareItems)
                return baseDropChance;
            
            if (!isRareItem && !applyToNormalItems)
                return baseDropChance;

            float bonus = CalculateDropRateBonus(resetCount);
            float modifiedChance = baseDropChance * (1f + bonus);

            // Cap at 100% (1.0)
            return Mathf.Min(modifiedChance, 1f);
        }

        /// <summary>
        /// Get formatted description
        /// Lấy mô tả định dạng
        /// </summary>
        public string GetDescription(int resetCount)
        {
            float bonus = CalculateDropRateBonus(resetCount);
            string desc = $"Drop Rate: +{bonus * 100:F1}%\n";
            
            if (applyToNormalItems)
                desc += "- Applies to normal items\n";
            
            if (applyToRareItems)
                desc += "- Applies to rare items\n";
            
            if (bonus >= maxDropRateBonus)
                desc += $"- Maximum bonus reached ({maxDropRateBonus * 100:F0}%)\n";

            return desc;
        }
    }
}
