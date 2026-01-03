using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Melee Skill - Skill tấn công cận chiến
    /// Melee Skill - Melee combat attack skills
    /// </summary>
    public class MeleeSkill : ActiveSkill
    {
        [Header("Melee Settings")]
        public float attackAngle = 60f;     // Góc tấn công (cone)
        public float attackRange = 3f;      // Tầm đánh
        public bool chainAttack = false;    // Có thể chain sang target khác
        public int maxChainTargets = 3;
        public LayerMask enemyLayer;
        
        /// <summary>
        /// Execute melee skill / Thực hiện melee skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            // Quay character hướng về target
            if (targetObject != null)
            {
                Vector3 direction = (targetObject.transform.position - owner.transform.position).normalized;
                owner.transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                Vector3 direction = (targetPosition - owner.transform.position).normalized;
                owner.transform.rotation = Quaternion.LookRotation(direction);
            }
            
            // Play attack animation
            PlayAttackAnimation();
            
            // Tìm và hit targets trong cone
            HitTargetsInCone();
        }
        
        /// <summary>
        /// Tìm và gây damage cho targets trong cone / Find and damage targets in cone
        /// </summary>
        protected virtual void HitTargetsInCone()
        {
            // Tìm tất cả colliders trong attack range
            Collider[] colliders = Physics.OverlapSphere(
                owner.transform.position, 
                attackRange, 
                enemyLayer
            );
            
            int hitCount = 0;
            
            foreach (Collider col in colliders)
            {
                if (col.gameObject == owner) continue;
                
                // Kiểm tra có nằm trong cone không
                if (IsInAttackCone(col.gameObject))
                {
                    DealDamageToTarget(col.gameObject);
                    hitCount++;
                    
                    // Chain attack nếu có
                    if (chainAttack && hitCount >= 1)
                    {
                        ChainToNearbyTargets(col.gameObject, 1);
                    }
                    
                    if (!chainAttack && hitCount >= skillData.maxTargets)
                    {
                        break;
                    }
                }
            }
            
            Debug.Log($"Melee skill hit {hitCount} targets: {skillData.skillName}");
        }
        
        /// <summary>
        /// Kiểm tra target có trong cone không / Check if target is in attack cone
        /// </summary>
        protected virtual bool IsInAttackCone(GameObject target)
        {
            Vector3 directionToTarget = (target.transform.position - owner.transform.position).normalized;
            float angle = Vector3.Angle(owner.transform.forward, directionToTarget);
            
            return angle <= attackAngle / 2f;
        }
        
        /// <summary>
        /// Chain attack sang targets gần / Chain attack to nearby targets
        /// </summary>
        protected virtual void ChainToNearbyTargets(GameObject lastTarget, int chainCount)
        {
            if (chainCount >= maxChainTargets) return;
            
            // Tìm target gần nhất chưa bị hit
            Collider[] nearbyColliders = Physics.OverlapSphere(
                lastTarget.transform.position, 
                5f, 
                enemyLayer
            );
            
            GameObject nextTarget = null;
            float nearestDistance = float.MaxValue;
            
            foreach (Collider col in nearbyColliders)
            {
                if (col.gameObject == owner) continue;
                if (col.gameObject == lastTarget) continue;
                
                float distance = Vector3.Distance(lastTarget.transform.position, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nextTarget = col.gameObject;
                }
            }
            
            if (nextTarget != null)
            {
                // Spawn chain effect
                CreateChainEffect(lastTarget.transform.position, nextTarget.transform.position);
                
                // Deal damage
                DealDamageToTarget(nextTarget, 0.8f); // Chain damage giảm 20%
                
                // Continue chain
                ChainToNearbyTargets(nextTarget, chainCount + 1);
            }
        }
        
        /// <summary>
        /// Tạo hiệu ứng chain / Create chain effect
        /// </summary>
        protected virtual void CreateChainEffect(Vector3 from, Vector3 to)
        {
            // TODO: Implement chain lightning/effect visual
            Debug.DrawLine(from, to, Color.yellow, 0.5f);
        }
        
        /// <summary>
        /// Gây damage lên target / Deal damage to target
        /// </summary>
        protected virtual void DealDamageToTarget(GameObject target, float damageMultiplier = 1f)
        {
            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats == null) return;
            
            CharacterStats ownerStats = owner.GetComponent<CharacterStats>();
            if (ownerStats == null) return;
            
            // Tính damage
            float damage = CalculateDamage(ownerStats, targetStats) * damageMultiplier;
            
            // Apply damage
            targetStats.currentHP = Mathf.Max(0, targetStats.currentHP - damage);
            
            // Spawn impact effect
            if (skillData.impactEffect != null)
            {
                Vector3 effectPosition = target.transform.position + Vector3.up;
                GameObject effect = Instantiate(skillData.impactEffect, effectPosition, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Apply knockback nếu có
            if (skillData.knockbackForce > 0f)
            {
                ApplyKnockback(target);
            }
            
            Debug.Log($"Dealt {damage} melee damage to {target.name}");
        }
        
        /// <summary>
        /// Tính damage / Calculate damage
        /// </summary>
        protected virtual float CalculateDamage(CharacterStats attacker, CharacterStats defender)
        {
            float baseDamage = GetDamage();
            
            // Melee skills chủ yếu dùng STR
            float statBonus = (attacker.STR * skillData.strRatio) + 
                            (attacker.AGI * skillData.agiRatio);
            
            float totalDamage = baseDamage + statBonus + attacker.attackPower;
            
            // Critical hit
            if (skillData.canCrit && Random.value < attacker.critRate)
            {
                totalDamage *= 2f;
                Debug.Log("Critical Hit!");
            }
            
            // Defense
            if (!skillData.pierceArmor)
            {
                float damageReduction = defender.defense / (defender.defense + 100f);
                totalDamage *= (1f - damageReduction);
            }
            
            return Mathf.Max(1f, totalDamage);
        }
        
        /// <summary>
        /// Áp dụng knockback / Apply knockback
        /// </summary>
        protected virtual void ApplyKnockback(GameObject target)
        {
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            Vector3 knockbackDirection = (target.transform.position - owner.transform.position).normalized;
            rb.AddForce(knockbackDirection * skillData.knockbackForce, ForceMode.Impulse);
        }
        
        /// <summary>
        /// Play attack animation / Phát animation tấn công
        /// </summary>
        protected virtual void PlayAttackAnimation()
        {
            Animator animator = owner.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }
}
