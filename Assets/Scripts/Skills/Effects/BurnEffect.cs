using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Burn Effect - Damage over time từ lửa + giảm defense
    /// Burn Effect - Fire damage over time + defense reduction
    /// </summary>
    public class BurnEffect : SkillEffect
    {
        [Header("Burn Settings")]
        public float damagePerTick = 15f;
        public float tickInterval = 1f;
        public float defenseReduction = 10f;  // Giảm defense khi bị cháy
        
        private float nextTickTime;
        private float appliedDefenseReduction = 0f;
        
        /// <summary>
        /// Initialize burn / Khởi tạo burn
        /// </summary>
        public override void Initialize(GameObject target, GameObject source, float duration = 0f)
        {
            base.Initialize(target, source, duration);
            nextTickTime = tickInterval;
        }
        
        /// <summary>
        /// Áp dụng burn effect / Apply burn effect
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            // Apply defense reduction
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats != null)
            {
                appliedDefenseReduction = defenseReduction * currentStacks;
                stats.defense = Mathf.Max(0, stats.defense - appliedDefenseReduction);
            }
            
            // Visual effect (fire particles)
            SpawnFireEffect();
            
            Debug.Log($"Burn applied to {target.name} for {remainingDuration}s (Defense -{appliedDefenseReduction})");
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
                ApplyBurnTick();
                nextTickTime = tickInterval;
            }
        }
        
        /// <summary>
        /// Áp dụng burn tick damage / Apply burn tick damage
        /// </summary>
        protected virtual void ApplyBurnTick()
        {
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            float damage = damagePerTick * currentStacks;
            
            stats.currentHP = Mathf.Max(0, stats.currentHP - damage);
            
            // Show damage number
            ShowBurnDamage(damage);
            
            Debug.Log($"Burn tick: {damage} damage to {target.name} (HP: {stats.currentHP}/{stats.maxHP})");
            
            // Check if target died
            if (stats.currentHP <= 0)
            {
                Debug.Log($"{target.name} died from burning");
            }
        }
        
        /// <summary>
        /// Spawn fire visual effect / Tạo hiệu ứng lửa
        /// </summary>
        protected virtual void SpawnFireEffect()
        {
            // Fire effect sẽ được spawn bởi base class
            // TODO: Add specific fire particle effect
        }
        
        /// <summary>
        /// Hiển thị burn damage / Show burn damage
        /// </summary>
        protected virtual void ShowBurnDamage(float damage)
        {
            // TODO: Implement burn damage number UI (red/orange color)
        }
        
        /// <summary>
        /// Loại bỏ burn / Remove burn
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null) return;
            
            // Remove defense reduction
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats != null)
            {
                stats.defense += appliedDefenseReduction;
            }
            
            Debug.Log($"Burn removed from {target.name}");
        }
        
        /// <summary>
        /// Override AddStack để tăng damage và defense reduction / Override AddStack
        /// </summary>
        public override bool AddStack()
        {
            if (!base.AddStack()) return false;
            
            // Reapply defense reduction với stack mới
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats != null)
            {
                // Remove old reduction
                stats.defense += appliedDefenseReduction;
                
                // Apply new reduction
                appliedDefenseReduction = defenseReduction * currentStacks;
                stats.defense = Mathf.Max(0, stats.defense - appliedDefenseReduction);
            }
            
            Debug.Log($"Burn stacked on {target.name}: {currentStacks} stacks");
            
            return true;
        }
    }
}
