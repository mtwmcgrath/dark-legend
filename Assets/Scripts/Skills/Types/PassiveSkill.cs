using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Passive skill - Skill bị động tự động kích hoạt
    /// Passive skill - Auto-triggered passive abilities
    /// </summary>
    public class PassiveSkill : SkillBase
    {
        [Header("Passive Settings")]
        public bool isActive = false;
        
        [Header("Stat Bonuses")]
        public float damageBonus = 0f;
        public float defenseBonus = 0f;
        public float hpRegenBonus = 0f;
        public float mpRegenBonus = 0f;
        public float critRateBonus = 0f;
        public float attackSpeedBonus = 0f;
        public float movementSpeedBonus = 0f;
        
        /// <summary>
        /// Passive skill không cần cooldown / Passive skills don't need cooldown
        /// </summary>
        public override bool CanUse()
        {
            return true;
        }
        
        /// <summary>
        /// Passive skill tự động activate khi học / Auto-activate when learned
        /// </summary>
        public override void Initialize(GameObject owner, SkillManager manager)
        {
            base.Initialize(owner, manager);
            ActivatePassive();
        }
        
        /// <summary>
        /// Kích hoạt passive skill / Activate passive skill
        /// </summary>
        public virtual void ActivatePassive()
        {
            if (isActive) return;
            
            isActive = true;
            ApplyPassiveEffects();
            
            Debug.Log($"Passive skill activated: {skillData.skillName}");
        }
        
        /// <summary>
        /// Vô hiệu hóa passive skill / Deactivate passive skill
        /// </summary>
        public virtual void DeactivatePassive()
        {
            if (!isActive) return;
            
            isActive = false;
            RemovePassiveEffects();
            
            Debug.Log($"Passive skill deactivated: {skillData.skillName}");
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng passive / Apply passive effects
        /// </summary>
        protected virtual void ApplyPassiveEffects()
        {
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            // Tính bonus dựa trên skill level
            float levelMultiplier = 1f + (currentLevel - 1) * 0.1f;
            
            // Áp dụng các bonus
            if (damageBonus > 0)
            {
                stats.attackPower += damageBonus * levelMultiplier;
            }
            
            if (defenseBonus > 0)
            {
                stats.defense += defenseBonus * levelMultiplier;
            }
            
            if (critRateBonus > 0)
            {
                stats.critRate += critRateBonus * levelMultiplier;
            }
        }
        
        /// <summary>
        /// Loại bỏ hiệu ứng passive / Remove passive effects
        /// </summary>
        protected virtual void RemovePassiveEffects()
        {
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            float levelMultiplier = 1f + (currentLevel - 1) * 0.1f;
            
            if (damageBonus > 0)
            {
                stats.attackPower -= damageBonus * levelMultiplier;
            }
            
            if (defenseBonus > 0)
            {
                stats.defense -= defenseBonus * levelMultiplier;
            }
            
            if (critRateBonus > 0)
            {
                stats.critRate -= critRateBonus * levelMultiplier;
            }
        }
        
        /// <summary>
        /// Level up passive skill cũng cập nhật bonus / Level up also updates bonuses
        /// </summary>
        public override bool LevelUp()
        {
            if (!base.LevelUp()) return false;
            
            // Cập nhật lại passive effects với level mới
            if (isActive)
            {
                RemovePassiveEffects();
                ApplyPassiveEffects();
            }
            
            return true;
        }
        
        /// <summary>
        /// Passive skill không execute theo cách thông thường
        /// Passive skills don't execute in the normal way
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            // Passive skills don't have active execution
            Debug.LogWarning("Passive skills should not be executed actively!");
        }
        
        /// <summary>
        /// Override Use cho passive (không làm gì)
        /// Override Use for passive (does nothing)
        /// </summary>
        public override bool Use(Vector3 targetPosition, GameObject targetObject = null)
        {
            Debug.LogWarning($"Cannot actively use passive skill: {skillData.skillName}");
            return false;
        }
        
        /// <summary>
        /// Update để xử lý regen nếu có / Update to handle regen if any
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            if (!isActive) return;
            
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            // HP Regen
            if (hpRegenBonus > 0)
            {
                float regenAmount = hpRegenBonus * Time.deltaTime * currentLevel;
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + regenAmount);
            }
            
            // MP Regen
            if (mpRegenBonus > 0)
            {
                float regenAmount = mpRegenBonus * Time.deltaTime * currentLevel;
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + regenAmount);
            }
        }
    }
}
