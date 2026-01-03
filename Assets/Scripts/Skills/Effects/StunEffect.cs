using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Stun Effect - Choáng, không thể hành động
    /// Stun Effect - Unable to take actions
    /// </summary>
    public class StunEffect : SkillEffect
    {
        [Header("Stun Settings")]
        public bool freezeMovement = true;
        public bool preventSkills = true;
        public bool preventAttacks = true;
        
        private bool wasStunned = false;
        
        /// <summary>
        /// Áp dụng stun / Apply stun
        /// </summary>
        protected override void ApplyEffect()
        {
            if (target == null) return;
            
            wasStunned = true;
            
            // Disable movement
            if (freezeMovement)
            {
                var movement = target.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.isStunned = true;
                }
                
                // Stop rigidbody
                var rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true;
                }
            }
            
            // Disable skills và attacks
            var skillManager = target.GetComponent<SkillManager>();
            if (skillManager != null && preventSkills)
            {
                // TODO: Add isStunned flag to SkillManager
            }
            
            // Play stun animation
            var animator = target.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("Stunned", true);
            }
            
            Debug.Log($"Stun applied to {target.name} for {remainingDuration}s");
        }
        
        /// <summary>
        /// Loại bỏ stun / Remove stun
        /// </summary>
        protected override void RemoveEffect()
        {
            if (target == null || !wasStunned) return;
            
            // Re-enable movement
            if (freezeMovement)
            {
                var movement = target.GetComponent<CharacterMovement>();
                if (movement != null)
                {
                    movement.isStunned = false;
                }
                
                var rb = target.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }
            
            // Reset animation
            var animator = target.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("Stunned", false);
            }
            
            wasStunned = false;
            
            Debug.Log($"Stun removed from {target.name}");
        }
    }
    
    /// <summary>
    /// Component giả định cho character movement
    /// Placeholder component for character movement
    /// </summary>
    public class CharacterMovement : MonoBehaviour
    {
        public bool isStunned = false;
        public bool isSlowed = false;
        public float slowMultiplier = 1f;
        
        // Các methods khác sẽ được implement sau
    }
}
