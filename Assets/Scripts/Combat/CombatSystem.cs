using UnityEngine;
using System;

namespace DarkLegend.Combat
{
    /// <summary>
    /// Main combat system handler
    /// Hệ thống xử lý combat chính
    /// </summary>
    public class CombatSystem : MonoBehaviour
    {
        [Header("Combat Settings")]
        public float attackRange = 2f;
        public float attackCooldown = 1f;
        public LayerMask enemyLayer;
        
        [Header("References")]
        public Character.CharacterStats characterStats;
        public Animator animator;
        public Transform attackPoint;
        
        // Events
        public event Action<GameObject, int, bool> OnAttackHit; // target, damage, isCritical
        public event Action OnAttackMiss;
        
        private float attackTimer = 0f;
        private GameObject currentTarget;
        private bool isAttacking = false;
        
        private static readonly int IsAttackingHash = Animator.StringToHash(Utils.Constants.ANIM_IS_ATTACKING);
        private static readonly int AttackHash = Animator.StringToHash(Utils.Constants.ANIM_ATTACK_TRIGGER);
        
        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<Character.CharacterStats>();
            }
            
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            
            if (attackPoint == null)
            {
                attackPoint = transform;
            }
        }
        
        private void Update()
        {
            UpdateAttackCooldown();
            HandleAttackInput();
        }
        
        /// <summary>
        /// Update attack cooldown timer
        /// Cập nhật bộ đếm thời gian cooldown tấn công
        /// </summary>
        private void UpdateAttackCooldown()
        {
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
        }
        
        /// <summary>
        /// Handle mouse click attack input
        /// Xử lý input tấn công bằng chuột
        /// </summary>
        private void HandleAttackInput()
        {
            if (characterStats != null && characterStats.IsDead)
                return;
            
            // Left click to attack
            if (Input.GetMouseButtonDown(0))
            {
                // Raycast to find target
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.collider.CompareTag(Utils.Constants.TAG_ENEMY))
                    {
                        currentTarget = hit.collider.gameObject;
                        TryAttack();
                    }
                }
            }
        }
        
        /// <summary>
        /// Try to perform an attack
        /// Thử thực hiện một đòn tấn công
        /// </summary>
        public bool TryAttack(GameObject target = null)
        {
            if (characterStats == null || characterStats.IsDead)
                return false;
            
            if (attackTimer > 0f)
                return false;
            
            if (target != null)
            {
                currentTarget = target;
            }
            
            if (currentTarget == null)
                return false;
            
            // Check range
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distance > attackRange)
                return false;
            
            // Perform attack
            PerformAttack();
            return true;
        }
        
        /// <summary>
        /// Perform the attack
        /// Thực hiện đòn tấn công
        /// </summary>
        private void PerformAttack()
        {
            isAttacking = true;
            
            // Apply cooldown with attack speed modifier
            float modifiedCooldown = attackCooldown / characterStats.attackSpeed;
            attackTimer = modifiedCooldown;
            
            // Face target
            if (currentTarget != null)
            {
                Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                direction.y = 0f;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            
            // Trigger animation
            if (animator != null)
            {
                animator.SetTrigger(AttackHash);
            }
            
            // Apply damage (called from animation event or immediately)
            ApplyDamage();
        }
        
        /// <summary>
        /// Apply damage to current target
        /// Áp dụng sát thương lên mục tiêu hiện tại
        /// </summary>
        public void ApplyDamage()
        {
            if (currentTarget == null) return;
            
            Character.CharacterStats targetStats = currentTarget.GetComponent<Character.CharacterStats>();
            if (targetStats == null || targetStats.IsDead) return;
            
            // Check if target is in range
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distance > attackRange)
            {
                OnAttackMiss?.Invoke();
                return;
            }
            
            // Roll for critical hit
            bool isCritical = DamageCalculator.RollCritical(characterStats.criticalChance);
            
            // Calculate damage
            int damage = DamageCalculator.CalculatePhysicalDamage(
                characterStats.physicalDamage,
                targetStats.defense,
                isCritical
            );
            
            // Apply damage
            targetStats.TakeDamage(damage);
            
            // Invoke event
            OnAttackHit?.Invoke(currentTarget, damage, isCritical);
            
            Debug.Log($"Attack hit {currentTarget.name} for {damage} damage{(isCritical ? " (CRITICAL!)" : "")}");
        }
        
        /// <summary>
        /// Set current attack target
        /// Đặt mục tiêu tấn công hiện tại
        /// </summary>
        public void SetTarget(GameObject target)
        {
            currentTarget = target;
        }
        
        /// <summary>
        /// Clear current target
        /// Xóa mục tiêu hiện tại
        /// </summary>
        public void ClearTarget()
        {
            currentTarget = null;
            isAttacking = false;
        }
        
        /// <summary>
        /// Check if can attack
        /// Kiểm tra có thể tấn công không
        /// </summary>
        public bool CanAttack()
        {
            return attackTimer <= 0f && !characterStats.IsDead;
        }
        
        /// <summary>
        /// Get attack cooldown progress (0 to 1)
        /// Lấy tiến độ cooldown tấn công (0 đến 1)
        /// </summary>
        public float GetAttackCooldownProgress()
        {
            if (attackCooldown <= 0f) return 1f;
            return 1f - (attackTimer / attackCooldown);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
