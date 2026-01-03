using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Combat
{
    /// <summary>
    /// Skill types
    /// Loại kỹ năng
    /// </summary>
    public enum SkillType
    {
        Physical,       // Physical damage skill
        Magic,          // Magic damage skill
        Buff,           // Buff/enhancement skill
        Heal,           // Healing skill
        Debuff          // Debuff/weakening skill
    }
    
    /// <summary>
    /// Skill target type
    /// Loại mục tiêu của skill
    /// </summary>
    public enum SkillTargetType
    {
        Self,           // Targets self
        SingleEnemy,    // Single enemy target
        AOE,            // Area of effect
        AllEnemies,     // All enemies in range
        Ally            // Ally target
    }
    
    /// <summary>
    /// Base skill class
    /// Class cơ bản cho skill
    /// </summary>
    [System.Serializable]
    public class Skill
    {
        public string skillName;
        public string description;
        public SkillType skillType;
        public SkillTargetType targetType;
        
        public int manaCost;
        public float cooldown;
        public float castTime;
        public float range;
        
        public float damageMultiplier = 1.0f;
        public float aoeRadius = 0f;
        public int maxTargets = 1;
        
        public GameObject effectPrefab;
        public AudioClip castSound;
        
        // Runtime data
        [System.NonSerialized]
        public float currentCooldown = 0f;
        
        public bool IsOnCooldown => currentCooldown > 0f;
        public bool CanCast(int currentMP) => !IsOnCooldown && currentMP >= manaCost;
        
        /// <summary>
        /// Update cooldown timer
        /// Cập nhật bộ đếm cooldown
        /// </summary>
        public void UpdateCooldown(float deltaTime)
        {
            if (currentCooldown > 0f)
            {
                currentCooldown -= deltaTime;
                if (currentCooldown < 0f)
                {
                    currentCooldown = 0f;
                }
            }
        }
        
        /// <summary>
        /// Start cooldown after casting
        /// Bắt đầu cooldown sau khi cast
        /// </summary>
        public void StartCooldown()
        {
            currentCooldown = cooldown;
        }
        
        /// <summary>
        /// Get cooldown progress (0 to 1)
        /// Lấy tiến độ cooldown (0 đến 1)
        /// </summary>
        public float GetCooldownProgress()
        {
            if (cooldown <= 0f) return 1f;
            return 1f - (currentCooldown / cooldown);
        }
    }
}
