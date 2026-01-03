using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Slow Effect - Giảm tốc độ di chuyển/tấn công
    /// Slow Effect - Reduce movement/attack speed
    /// </summary>
    public class SlowEffect : SkillEffect
    {
        [Header("Slow Settings")]
        public float slowPercentage = 0.5f;  // 0.5 = 50% slower
        public bool slowMovement = true;
        public bool slowAttackSpeed = true;
        
        private bool wasSlowed = false;
        
        /// <summary>
        /// Áp dụng slow / Apply slow
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            wasSlowed = true;
            
            float slowMultiplier = 1f - (slowPercentage * currentStacks);
            slowMultiplier = Mathf.Max(0.1f, slowMultiplier); // Tối thiểu 10% speed
            
            // Apply movement slow
            if (slowMovement)
            {
                var movement = target.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.isSlowed = true;
                    movement.slowMultiplier = slowMultiplier;
                }
            }
            
            // Apply attack speed slow
            if (slowAttackSpeed)
            {
                // TODO: Implement attack speed modification
            }
            
            // Visual effect (blue tint or ice particles)
            var renderer = target.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                // TODO: Add slow visual effect
            }
            
            Debug.Log($"Slow applied to {target.name}: {slowPercentage * 100}% slower");
        }
        
        /// <summary>
        /// Loại bỏ slow / Remove slow
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null || !wasSlowed) return;
            
            // Remove movement slow
            if (slowMovement)
            {
                var movement = target.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.isSlowed = false;
                    movement.slowMultiplier = 1f;
                }
            }
            
            // Remove attack speed slow
            if (slowAttackSpeed)
            {
                // TODO: Reset attack speed
            }
            
            wasSlowed = false;
            
            Debug.Log($"Slow removed from {target.name}");
        }
        
        /// <summary>
        /// Override AddStack để cập nhật slow / Override AddStack to update slow
        /// </summary>
        public override bool AddStack()
        {
            if (!base.AddStack()) return false;
            
            // Reapply với stack mới
            RemoveEffect();
            ApplyEffect();
            
            return true;
        }
    }
}
