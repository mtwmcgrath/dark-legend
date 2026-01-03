using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Knockback Effect - Đẩy lùi kẻ địch
    /// Knockback Effect - Push back enemies
    /// </summary>
    public class KnockbackEffect : SkillEffect
    {
        [Header("Knockback Settings")]
        public float knockbackForce = 10f;
        public float knockbackDuration = 0.3f;
        public bool stunDuringKnockback = true;
        
        private Vector3 knockbackDirection;
        private Rigidbody targetRigidbody;
        
        /// <summary>
        /// Initialize knockback với direction / Initialize knockback with direction
        /// </summary>
        public void Initialize(GameObject target, GameObject source, Vector3 direction, float force = 0f)
        {
            this.knockbackDirection = direction.normalized;
            if (force > 0) this.knockbackForce = force;
            
            Initialize(target, source, knockbackDuration);
        }
        
        /// <summary>
        /// Áp dụng knockback / Apply knockback
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            targetRigidbody = target.GetComponent<Rigidbody>();
            if (targetRigidbody == null)
            {
                Debug.LogWarning($"Target {target.name} has no Rigidbody for knockback!");
                EndEffect();
                return;
            }
            
            // Tính direction nếu chưa có
            if (knockbackDirection == Vector3.zero && source != null)
            {
                knockbackDirection = (target.transform.position - source.transform.position).normalized;
            }
            
            // Apply knockback force
            targetRigidbody.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            
            // Stun during knockback nếu có
            if (stunDuringKnockback)
            {
                var movement = target.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.isStunned = true;
                }
            }
            
            Debug.Log($"Knockback applied to {target.name}: force {knockbackForce}");
        }
        
        /// <summary>
        /// Loại bỏ knockback / Remove knockback
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null) return;
            
            // Stop momentum
            if (targetRigidbody != null)
            {
                targetRigidbody.velocity = Vector3.zero;
            }
            
            // Remove stun
            if (stunDuringKnockback)
            {
                var movement = target.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.isStunned = false;
                }
            }
            
            Debug.Log($"Knockback ended on {target.name}");
        }
        
        /// <summary>
        /// Override Update để check nếu target chạm đất / Override Update to check ground
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            // End knockback early nếu velocity gần 0
            if (targetRigidbody != null && targetRigidbody.velocity.magnitude < 0.5f)
            {
                EndEffect();
            }
        }
    }
}
