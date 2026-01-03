using UnityEngine;

namespace DarkLegend.Combat
{
    /// <summary>
    /// Damage calculation utilities
    /// Tiện ích tính toán sát thương
    /// </summary>
    public static class DamageCalculator
    {
        /// <summary>
        /// Calculate physical damage
        /// Tính sát thương vật lý
        /// </summary>
        public static int CalculatePhysicalDamage(float attackerDamage, float targetDefense, bool isCritical = false)
        {
            // Basic formula: Damage - (Defense * 0.5)
            float baseDamage = attackerDamage - (targetDefense * 0.5f);
            baseDamage = Mathf.Max(1, baseDamage); // Minimum 1 damage
            
            // Apply critical hit
            if (isCritical)
            {
                baseDamage *= Utils.Constants.CRITICAL_HIT_MULTIPLIER;
            }
            
            // Add random variance (±10%)
            float variance = Random.Range(0.9f, 1.1f);
            baseDamage *= variance;
            
            return Mathf.RoundToInt(baseDamage);
        }
        
        /// <summary>
        /// Calculate magic damage
        /// Tính sát thương phép thuật
        /// </summary>
        public static int CalculateMagicDamage(float attackerMagicDamage, float targetDefense, bool isCritical = false)
        {
            // Magic damage ignores more defense
            float baseDamage = attackerMagicDamage - (targetDefense * 0.3f);
            baseDamage = Mathf.Max(1, baseDamage);
            
            if (isCritical)
            {
                baseDamage *= Utils.Constants.CRITICAL_HIT_MULTIPLIER;
            }
            
            float variance = Random.Range(0.9f, 1.1f);
            baseDamage *= variance;
            
            return Mathf.RoundToInt(baseDamage);
        }
        
        /// <summary>
        /// Calculate skill damage with multiplier
        /// Tính sát thương skill với hệ số nhân
        /// </summary>
        public static int CalculateSkillDamage(float baseDamage, float skillMultiplier, float targetDefense, bool isMagic = false, bool isCritical = false)
        {
            float damage = baseDamage * skillMultiplier;
            
            if (isMagic)
            {
                damage -= targetDefense * 0.3f;
            }
            else
            {
                damage -= targetDefense * 0.5f;
            }
            
            damage = Mathf.Max(1, damage);
            
            if (isCritical)
            {
                damage *= Utils.Constants.CRITICAL_HIT_MULTIPLIER;
            }
            
            float variance = Random.Range(0.9f, 1.1f);
            damage *= variance;
            
            return Mathf.RoundToInt(damage);
        }
        
        /// <summary>
        /// Check if attack is critical hit
        /// Kiểm tra xem tấn công có phải là chí mạng không
        /// </summary>
        public static bool RollCritical(float criticalChance)
        {
            return Random.value <= criticalChance;
        }
        
        /// <summary>
        /// Check if attack is dodged
        /// Kiểm tra xem tấn công có bị né không
        /// </summary>
        public static bool RollDodge(float dodgeChance)
        {
            return Random.value <= dodgeChance;
        }
        
        /// <summary>
        /// Calculate damage reduction from defense rate
        /// Tính giảm sát thương từ chỉ số phòng thủ
        /// </summary>
        public static int ApplyDefenseRate(int damage, float defenseRate)
        {
            float reduction = defenseRate / (defenseRate + 100f);
            int reducedDamage = Mathf.RoundToInt(damage * (1f - reduction));
            return Mathf.Max(1, reducedDamage);
        }
    }
}
