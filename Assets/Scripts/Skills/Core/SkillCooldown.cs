using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Hệ thống cooldown cho skills
    /// Cooldown system for skills
    /// </summary>
    [CreateAssetMenu(fileName = "New Cooldown", menuName = "Dark Legend/Skills/Cooldown")]
    public class SkillCooldown : ScriptableObject
    {
        [Header("Cooldown Settings")]
        [Tooltip("Cooldown cơ bản ở level 1 (giây) / Base cooldown at level 1 (seconds)")]
        public float baseCooldown = 5f;
        
        [Tooltip("Giảm cooldown mỗi level / Cooldown reduction per level")]
        public float cooldownReductionPerLevel = 0.1f;
        
        [Tooltip("Cooldown tối thiểu / Minimum cooldown")]
        public float minCooldown = 1f;
        
        /// <summary>
        /// Tính thời gian cooldown ở level cụ thể
        /// Calculate cooldown time at specific level
        /// </summary>
        public float GetCooldownTime(int level)
        {
            float cooldown = baseCooldown - (cooldownReductionPerLevel * (level - 1));
            return Mathf.Max(cooldown, minCooldown);
        }
        
        /// <summary>
        /// Tính cooldown với % giảm thêm từ equipment/buffs
        /// Calculate cooldown with additional reduction from equipment/buffs
        /// </summary>
        public float GetCooldownTime(int level, float additionalReduction)
        {
            float baseCooldownTime = GetCooldownTime(level);
            float reducedCooldown = baseCooldownTime * (1f - additionalReduction);
            return Mathf.Max(reducedCooldown, minCooldown);
        }
    }
}
