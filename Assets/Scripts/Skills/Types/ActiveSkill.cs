using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Active skill - Skill chủ động cần cast
    /// Active skill - Skills that require casting
    /// </summary>
    public class ActiveSkill : SkillBase
    {
        [Header("Active Skill Settings")]
        public bool requiresTarget = true;
        public bool isCasting = false;
        public float castTimeRemaining = 0f;
        
        private Vector3 pendingTargetPosition;
        private GameObject pendingTargetObject;
        
        /// <summary>
        /// Override Use để xử lý cast time
        /// Override Use to handle cast time
        /// </summary>
        public override bool Use(Vector3 targetPosition, GameObject targetObject = null)
        {
            if (!CanUse()) return false;
            
            // Nếu skill có cast time, bắt đầu cast
            if (skillData.castTime > 0f)
            {
                StartCasting(targetPosition, targetObject);
                return true;
            }
            
            // Skill instant, execute ngay
            return base.Use(targetPosition, targetObject);
        }
        
        /// <summary>
        /// Bắt đầu cast skill / Start casting skill
        /// </summary>
        protected virtual void StartCasting(Vector3 targetPosition, GameObject targetObject)
        {
            isCasting = true;
            castTimeRemaining = skillData.castTime;
            pendingTargetPosition = targetPosition;
            pendingTargetObject = targetObject;
            
            // Play cast animation
            PlayCastAnimation();
            
            // Spawn cast effect
            if (skillData.castEffect != null)
            {
                Instantiate(skillData.castEffect, owner.transform.position, Quaternion.identity);
            }
            
            // Play cast sound
            if (skillData.castSound != null)
            {
                AudioSource.PlayClipAtPoint(skillData.castSound, owner.transform.position);
            }
        }
        
        /// <summary>
        /// Hủy cast / Cancel casting
        /// </summary>
        public virtual void CancelCast()
        {
            if (!isCasting) return;
            
            isCasting = false;
            castTimeRemaining = 0f;
            
            Debug.Log($"Cast cancelled: {skillData.skillName}");
        }
        
        /// <summary>
        /// Update để xử lý cast time / Update to handle cast time
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            if (isCasting)
            {
                castTimeRemaining -= Time.deltaTime;
                
                if (castTimeRemaining <= 0f)
                {
                    // Cast hoàn thành, execute skill
                    CompleteCast();
                }
            }
        }
        
        /// <summary>
        /// Hoàn thành cast / Complete casting
        /// </summary>
        protected virtual void CompleteCast()
        {
            isCasting = false;
            castTimeRemaining = 0f;
            
            // Execute skill với target đã lưu
            ConsumeCost();
            StartCooldown();
            ExecuteSkill(pendingTargetPosition, pendingTargetObject);
        }
        
        /// <summary>
        /// Execute skill effect - override trong subclass
        /// Execute skill effect - override in subclass
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            Debug.Log($"Executing active skill: {skillData.skillName}");
            
            // Play impact effect
            if (skillData.impactEffect != null)
            {
                Instantiate(skillData.impactEffect, targetPosition, Quaternion.identity);
            }
            
            // Play impact sound
            if (skillData.impactSound != null)
            {
                AudioSource.PlayClipAtPoint(skillData.impactSound, targetPosition);
            }
        }
        
        /// <summary>
        /// Play cast animation / Phát animation cast
        /// </summary>
        protected virtual void PlayCastAnimation()
        {
            Animator animator = owner.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Cast");
            }
        }
        
        /// <summary>
        /// Kiểm tra target có hợp lệ không / Check if target is valid
        /// </summary>
        protected virtual bool IsValidTarget(GameObject target)
        {
            if (target == null) return false;
            
            // Kiểm tra khoảng cách
            float distance = Vector3.Distance(owner.transform.position, target.transform.position);
            if (distance > skillData.castRange)
            {
                return false;
            }
            
            return true;
        }
    }
}
