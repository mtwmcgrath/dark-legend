using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Damage Effect - Gây sát thương
    /// Damage Effect - Deal damage
    /// </summary>
    public class DamageEffect : SkillEffect
    {
        [Header("Damage Settings")]
        public float damageAmount = 100f;
        public DamageType damageType = DamageType.Physical;
        public bool canCrit = true;
        public bool pierceArmor = false;
        
        /// <summary>
        /// Áp dụng damage / Apply damage
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats == null) return;
            
            CharacterStats sourceStats = source != null ? source.GetComponent<CharacterStats>() : null;
            
            float finalDamage = CalculateDamage(sourceStats, targetStats);
            
            // Apply damage
            targetStats.currentHP = Mathf.Max(0, targetStats.currentHP - finalDamage);
            
            // Show damage number
            ShowDamageNumber(finalDamage);
            
            Debug.Log($"Dealt {finalDamage} {damageType} damage to {target.name}");
        }
        
        /// <summary>
        /// Tính toán damage cuối cùng / Calculate final damage
        /// </summary>
        protected virtual float CalculateDamage(CharacterStats attacker, CharacterStats defender)
        {
            float damage = damageAmount * currentStacks;
            
            // Apply attacker stats
            if (attacker != null)
            {
                if (damageType == DamageType.Physical)
                {
                    damage += attacker.attackPower;
                }
                else if (damageType == DamageType.Magic)
                {
                    damage += attacker.magicPower;
                }
                
                // Critical hit
                if (canCrit && Random.value < attacker.critRate)
                {
                    damage *= 2f;
                    Debug.Log("Critical Hit!");
                }
            }
            
            // Apply defender defense
            if (!pierceArmor && defender != null)
            {
                float damageReduction = defender.defense / (defender.defense + 100f);
                damage *= (1f - damageReduction);
            }
            
            return Mathf.Max(1f, damage);
        }
        
        /// <summary>
        /// Hiển thị damage number / Show damage number
        /// </summary>
        protected virtual void ShowDamageNumber(float damage)
        {
            // TODO: Implement damage number UI
        }
        
        /// <summary>
        /// Damage effect là instant, không cần remove / Damage is instant, no removal needed
        /// </summary>
        protected override void RemoveEffect()
        {
            // Instant effect, nothing to remove
        }
        
        /// <summary>
        /// Override Update vì damage là instant / Override Update since damage is instant
        /// </summary>
        protected override void Update()
        {
            // Instant effect, destroy immediately
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
    
    /// <summary>
    /// Loại damage / Damage types
    /// </summary>
    public enum DamageType
    {
        Physical,    // Vật lý
        Magic,       // Phép thuật
        True         // True damage (bỏ qua armor)
    }
}
