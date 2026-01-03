using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Debuff Effect - Giảm stats
    /// Debuff Effect - Decrease stats
    /// </summary>
    public class DebuffEffect : SkillEffect
    {
        [Header("Debuff Settings")]
        public DebuffType debuffType = DebuffType.AttackPower;
        public float debuffValue = 30f;
        public bool isPercentage = false;    // Debuff theo % hay flat value
        
        private float appliedValue = 0f;
        
        /// <summary>
        /// Áp dụng debuff / Apply debuff
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            appliedValue = CalculateDebuffValue(stats);
            
            switch (debuffType)
            {
                case DebuffType.AttackPower:
                    stats.attackPower = Mathf.Max(0, stats.attackPower - appliedValue);
                    break;
                    
                case DebuffType.Defense:
                    stats.defense = Mathf.Max(0, stats.defense - appliedValue);
                    break;
                    
                case DebuffType.AttackSpeed:
                    // TODO: Implement attack speed modification
                    break;
                    
                case DebuffType.MovementSpeed:
                    // TODO: Implement movement speed modification
                    break;
                    
                case DebuffType.Blind:
                    // TODO: Implement accuracy reduction
                    break;
            }
            
            Debug.Log($"Debuff {debuffType} applied to {target.name}: -{appliedValue}");
        }
        
        /// <summary>
        /// Loại bỏ debuff / Remove debuff
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null) return;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            switch (debuffType)
            {
                case DebuffType.AttackPower:
                    stats.attackPower += appliedValue;
                    break;
                    
                case DebuffType.Defense:
                    stats.defense += appliedValue;
                    break;
            }
            
            Debug.Log($"Debuff {debuffType} removed from {target.name}");
        }
        
        /// <summary>
        /// Tính giá trị debuff / Calculate debuff value
        /// </summary>
        protected virtual float CalculateDebuffValue(CharacterStats stats)
        {
            float value = debuffValue * currentStacks;
            
            if (isPercentage)
            {
                switch (debuffType)
                {
                    case DebuffType.AttackPower:
                        value = stats.attackPower * debuffValue;
                        break;
                        
                    case DebuffType.Defense:
                        value = stats.defense * debuffValue;
                        break;
                }
            }
            
            return value;
        }
        
        /// <summary>
        /// Override AddStack để cập nhật debuff / Override AddStack to update debuff
        /// </summary>
        public override bool AddStack()
        {
            if (!base.AddStack()) return false;
            
            // Remove old debuff và apply lại với stack mới
            RemoveEffect();
            ApplyEffect();
            
            return true;
        }
    }
}
