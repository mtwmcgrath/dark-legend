using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Buff Effect - Tăng stats
    /// Buff Effect - Increase stats
    /// </summary>
    public class BuffEffect : SkillEffect
    {
        [Header("Buff Settings")]
        public BuffType buffType = BuffType.AttackPower;
        public float buffValue = 50f;
        public bool isPercentage = false;    // Buff theo % hay flat value
        
        private float appliedValue = 0f;
        
        /// <summary>
        /// Áp dụng buff / Apply buff
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            appliedValue = CalculateBuffValue(stats);
            
            switch (buffType)
            {
                case BuffType.AttackPower:
                    stats.attackPower += appliedValue;
                    break;
                    
                case BuffType.Defense:
                    stats.defense += appliedValue;
                    break;
                    
                case BuffType.CritRate:
                    stats.critRate += appliedValue;
                    break;
                    
                case BuffType.MaxHP:
                    stats.maxHP += appliedValue;
                    break;
                    
                case BuffType.MaxMP:
                    stats.maxMP += appliedValue;
                    break;
                    
                case BuffType.AttackSpeed:
                    // TODO: Implement attack speed modification
                    break;
                    
                case BuffType.MovementSpeed:
                    // TODO: Implement movement speed modification
                    break;
            }
            
            Debug.Log($"Buff {buffType} applied to {target.name}: +{appliedValue}");
        }
        
        /// <summary>
        /// Loại bỏ buff / Remove buff
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            switch (buffType)
            {
                case BuffType.AttackPower:
                    stats.attackPower = Mathf.Max(0, stats.attackPower - appliedValue);
                    break;
                    
                case BuffType.Defense:
                    stats.defense = Mathf.Max(0, stats.defense - appliedValue);
                    break;
                    
                case BuffType.CritRate:
                    stats.critRate = Mathf.Max(0, stats.critRate - appliedValue);
                    break;
                    
                case BuffType.MaxHP:
                    stats.maxHP = Mathf.Max(1, stats.maxHP - appliedValue);
                    // Clamp current HP
                    stats.currentHP = Mathf.Min(stats.currentHP, stats.maxHP);
                    break;
                    
                case BuffType.MaxMP:
                    stats.maxMP = Mathf.Max(0, stats.maxMP - appliedValue);
                    stats.currentMP = Mathf.Min(stats.currentMP, stats.maxMP);
                    break;
            }
            
            Debug.Log($"Buff {buffType} removed from {target.name}");
        }
        
        /// <summary>
        /// Tính giá trị buff / Calculate buff value
        /// </summary>
        protected virtual float CalculateBuffValue(CharacterStats stats)
        {
            float value = buffValue * currentStacks;
            
            if (isPercentage)
            {
                switch (buffType)
                {
                    case BuffType.AttackPower:
                        value = stats.attackPower * buffValue;
                        break;
                        
                    case BuffType.Defense:
                        value = stats.defense * buffValue;
                        break;
                        
                    case BuffType.MaxHP:
                        value = stats.maxHP * buffValue;
                        break;
                        
                    case BuffType.MaxMP:
                        value = stats.maxMP * buffValue;
                        break;
                }
            }
            
            return value;
        }
        
        /// <summary>
        /// Override AddStack để cập nhật buff / Override AddStack to update buff
        /// </summary>
        public override bool AddStack()
        {
            if (!base.AddStack()) return false;
            
            // Remove old buff và apply lại với stack mới
            RemoveEffect();
            ApplyEffect();
            
            return true;
        }
    }
}
