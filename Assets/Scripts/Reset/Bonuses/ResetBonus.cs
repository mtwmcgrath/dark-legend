using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Base class for reset bonuses - Class cơ sở cho bonuses reset
    /// All bonus types inherit from this class
    /// </summary>
    [System.Serializable]
    public abstract class ResetBonus
    {
        [Header("Bonus Info")]
        [Tooltip("Bonus name - Tên bonus")]
        public string bonusName;

        [Tooltip("Bonus description - Mô tả bonus")]
        public string description;

        [Tooltip("Is cumulative - Có tích lũy không")]
        public bool isCumulative = true;

        /// <summary>
        /// Apply bonus to character
        /// Áp dụng bonus cho nhân vật
        /// </summary>
        public abstract void Apply(CharacterStats character, int resetCount);

        /// <summary>
        /// Remove bonus from character
        /// Gỡ bỏ bonus khỏi nhân vật
        /// </summary>
        public abstract void Remove(CharacterStats character);

        /// <summary>
        /// Get bonus value as string for display
        /// Lấy giá trị bonus dưới dạng string để hiển thị
        /// </summary>
        public abstract string GetBonusString(int resetCount);

        /// <summary>
        /// Calculate bonus value based on reset count
        /// Tính giá trị bonus dựa trên số lần reset
        /// </summary>
        protected float CalculateBonusMultiplier(int resetCount, float baseMultiplier, float increment)
        {
            if (!isCumulative)
                return baseMultiplier;

            return baseMultiplier + (resetCount * increment);
        }
    }
}
