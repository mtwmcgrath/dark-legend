using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Poison Effect - Damage over time từ độc
    /// Poison Effect - Damage over time from poison
    /// </summary>
    public class PoisonEffect : SkillEffect
    {
        [Header("Poison Settings")]
        public float damagePerTick = 10f;
        public float tickInterval = 1f;      // Damage mỗi giây
        
        private float nextTickTime;
        
        /// <summary>
        /// Initialize poison / Khởi tạo poison
        /// </summary>
        public override void Initialize(GameObject target, GameObject source, float duration = 0f)
        {
            base.Initialize(target, source, duration);
            nextTickTime = tickInterval;
        }
        
        /// <summary>
        /// Áp dụng poison effect ban đầu / Apply initial poison effect
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            Debug.Log($"Poison applied to {target.name} for {remainingDuration}s");
        }
        
        /// <summary>
        /// Update để tick damage / Update to tick damage
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            if (target == null)
            {
                EndEffect();
                return;
            }
            
            // Tick damage
            nextTickTime -= Time.deltaTime;
            if (nextTickTime <= 0f)
            {
                ApplyPoisonTick();
                nextTickTime = tickInterval;
            }
        }
        
        /// <summary>
        /// Áp dụng poison tick damage / Apply poison tick damage
        /// </summary>
        protected virtual void ApplyPoisonTick()
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            float damage = damagePerTick * currentStacks;
            
            stats.currentHP = Mathf.Max(0, stats.currentHP - damage);
            
            // Show damage number
            ShowPoisonDamage(damage);
            
            Debug.Log($"Poison tick: {damage} damage to {target.name} (HP: {stats.currentHP}/{stats.maxHP})");
            
            // Check if target died
            if (stats.currentHP <= 0)
            {
                Debug.Log($"{target.name} died from poison");
            }
        }
        
        /// <summary>
        /// Hiển thị poison damage / Show poison damage
        /// </summary>
        protected virtual void ShowPoisonDamage(float damage)
        {
            // TODO: Implement poison damage number UI (green color)
        }
        
        /// <summary>
        /// Loại bỏ poison / Remove poison
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null) return;
            
            Debug.Log($"Poison removed from {target.name}");
        }
        
        /// <summary>
        /// Override AddStack để tăng damage / Override AddStack to increase damage
        /// </summary>
        public override bool AddStack()
        {
            if (!base.AddStack()) return false;
            
            Debug.Log($"Poison stacked on {target.name}: {currentStacks} stacks");
            
            return true;
        }
    }
}
