using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Heal Effect - Hồi HP/MP
    /// Heal Effect - Restore HP/MP
    /// </summary>
    public class HealEffect : SkillEffect
    {
        [Header("Heal Settings")]
        public float healAmount = 50f;
        public HealType healType = HealType.HP;
        public bool isPercentage = false;    // Heal theo % max HP/MP
        
        /// <summary>
        /// Áp dụng heal / Apply heal
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            float actualHeal = CalculateHealAmount(stats);
            
            if (healType == HealType.HP)
            {
                float oldHP = stats.currentHP;
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + actualHeal);
                float healed = stats.currentHP - oldHP;
                
                ShowHealNumber(healed);
                Debug.Log($"Healed {target.name} for {healed} HP");
            }
            else if (healType == HealType.MP)
            {
                float oldMP = stats.currentMP;
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + actualHeal);
                float restored = stats.currentMP - oldMP;
                
                Debug.Log($"Restored {restored} MP to {target.name}");
            }
            else if (healType == HealType.Both)
            {
                stats.currentHP = Mathf.Min(stats.maxHP, stats.currentHP + actualHeal);
                stats.currentMP = Mathf.Min(stats.maxMP, stats.currentMP + actualHeal * 0.5f);
            }
        }
        
        /// <summary>
        /// Tính lượng heal / Calculate heal amount
        /// </summary>
        protected virtual float CalculateHealAmount(CharacterStats stats)
        {
            float heal = healAmount * currentStacks;
            
            if (isPercentage)
            {
                if (healType == HealType.HP)
                {
                    heal = stats.maxHP * healAmount;
                }
                else if (healType == HealType.MP)
                {
                    heal = stats.maxMP * healAmount;
                }
                else if (healType == HealType.Both)
                {
                    heal = stats.maxHP * healAmount; // Use HP as base for both
                }
            }
            
            return heal;
        }
        
        /// <summary>
        /// Hiển thị heal number / Show heal number
        /// </summary>
        protected virtual void ShowHealNumber(float amount)
        {
            // TODO: Implement heal number UI
        }
        
        /// <summary>
        /// Heal effect là instant / Heal is instant
        /// </summary>
        protected override void RemoveEffect()
        {
            // Instant effect, nothing to remove
        }
        
        /// <summary>
        /// Override Update vì heal là instant / Override Update since heal is instant
        /// </summary>
        protected override void Update()
        {
            if (duration <= 0)
            {
                EndEffect();
            }
            else
            {
                base.Update();
            }
        }
    }
}
