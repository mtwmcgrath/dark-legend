using UnityEngine;
using UnityEngine.AI;

namespace DarkLegend.Enemy
{
    /// <summary>
    /// AI states for enemy behavior
    /// Các trạng thái AI cho hành vi quái vật
    /// </summary>
    public enum EnemyState
    {
        Idle,           // Standing still
        Patrol,         // Patrolling area
        Chase,          // Chasing target
        Attack,         // Attacking target
        Return          // Returning to patrol area
    }
    
    /// <summary>
    /// Enemy AI behavior: patrol, chase, attack
    /// Hành vi AI của quái: tuần tra, đuổi theo, tấn công
    /// </summary>
    [RequireComponent(typeof(EnemyStats))]
    public class EnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        public EnemyState currentState = EnemyState.Idle;
        public float patrolRange = 10f;
        public float chaseRange = 15f;
        public float attackRange = 2f;
        public float aggroRange = 10f;
        public bool isAggressive = true;
        
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float patrolWaitTime = 2f;
        public float returnToPatrolDistance = 20f;
        
        [Header("Combat")]
        public float attackCooldown = 2f;
        
        [Header("References")]
        public EnemyStats stats;
        public Animator animator;
        
        // Private variables
        private Vector3 spawnPosition;
        private Vector3 currentPatrolTarget;
        private GameObject targetEnemy;
        private float stateTimer = 0f;
        private float attackTimer = 0f;
        private UnityEngine.AI.NavMeshAgent navAgent;
        
        // Animation hashes
        private static readonly int MoveSpeedHash = Animator.StringToHash(Utils.Constants.ANIM_MOVE_SPEED);
        private static readonly int AttackHash = Animator.StringToHash(Utils.Constants.ANIM_ATTACK_TRIGGER);
        private static readonly int IsDeadHash = Animator.StringToHash(Utils.Constants.ANIM_IS_DEAD);
        
        private void Start()
        {
            spawnPosition = transform.position;
            
            if (stats == null)
            {
                stats = GetComponent<EnemyStats>();
            }
            
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            
            // Try to get NavMeshAgent (optional)
            navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.speed = moveSpeed;
            }
            
            SetState(EnemyState.Patrol);
        }
        
        private void Update()
        {
            if (stats != null && stats.IsDead)
                return;
            
            UpdateAttackCooldown();
            UpdateState();
        }
        
        /// <summary>
        /// Initialize AI from enemy data
        /// Khởi tạo AI từ dữ liệu quái vật
        /// </summary>
        public void InitializeFromData(EnemyData data)
        {
            if (data == null) return;
            
            patrolRange = data.patrolRange;
            chaseRange = data.chaseRange;
            attackRange = data.attackRange;
            moveSpeed = data.moveSpeed;
            attackCooldown = data.attackCooldown;
            returnToPatrolDistance = data.returnToPatrolDistance;
            patrolWaitTime = data.patrolWaitTime;
            isAggressive = data.isAggressive;
            aggroRange = data.aggroRange;
            
            if (navAgent != null)
            {
                navAgent.speed = moveSpeed;
            }
        }
        
        /// <summary>
        /// Update attack cooldown
        /// Cập nhật thời gian chờ tấn công
        /// </summary>
        private void UpdateAttackCooldown()
        {
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
        }
        
        /// <summary>
        /// Main state machine update
        /// Cập nhật máy trạng thái chính
        /// </summary>
        private void UpdateState()
        {
            // Find player if aggressive
            if (isAggressive && targetEnemy == null)
            {
                FindNearbyPlayer();
            }
            
            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdleState();
                    break;
                case EnemyState.Patrol:
                    UpdatePatrolState();
                    break;
                case EnemyState.Chase:
                    UpdateChaseState();
                    break;
                case EnemyState.Attack:
                    UpdateAttackState();
                    break;
                case EnemyState.Return:
                    UpdateReturnState();
                    break;
            }
        }
        
        /// <summary>
        /// Update idle state
        /// Cập nhật trạng thái đứng yên
        /// </summary>
        private void UpdateIdleState()
        {
            stateTimer -= Time.deltaTime;
            
            if (stateTimer <= 0f)
            {
                SetState(EnemyState.Patrol);
            }
            
            // Check for nearby enemies
            if (CheckForTarget())
            {
                SetState(EnemyState.Chase);
            }
        }
        
        /// <summary>
        /// Update patrol state
        /// Cập nhật trạng thái tuần tra
        /// </summary>
        private void UpdatePatrolState()
        {
            // Check for nearby enemies first
            if (CheckForTarget())
            {
                SetState(EnemyState.Chase);
                return;
            }
            
            // Move to patrol point
            if (Vector3.Distance(transform.position, currentPatrolTarget) < 1f)
            {
                SetState(EnemyState.Idle);
            }
            else
            {
                MoveTowards(currentPatrolTarget);
            }
        }
        
        /// <summary>
        /// Update chase state
        /// Cập nhật trạng thái đuổi theo
        /// </summary>
        private void UpdateChaseState()
        {
            if (targetEnemy == null)
            {
                SetState(EnemyState.Return);
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
            
            // Check if target is too far
            if (distanceToTarget > returnToPatrolDistance)
            {
                targetEnemy = null;
                SetState(EnemyState.Return);
                return;
            }
            
            // Check if in attack range
            if (distanceToTarget <= attackRange)
            {
                SetState(EnemyState.Attack);
            }
            else
            {
                MoveTowards(targetEnemy.transform.position);
            }
        }
        
        /// <summary>
        /// Update attack state
        /// Cập nhật trạng thái tấn công
        /// </summary>
        private void UpdateAttackState()
        {
            if (targetEnemy == null)
            {
                SetState(EnemyState.Return);
                return;
            }
            
            float distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
            
            // Face target
            Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(direction), 
                    10f * Time.deltaTime);
            }
            
            // Check if target moved out of range
            if (distanceToTarget > attackRange * 1.2f)
            {
                SetState(EnemyState.Chase);
                return;
            }
            
            // Attack if cooldown is ready
            if (attackTimer <= 0f)
            {
                PerformAttack();
            }
        }
        
        /// <summary>
        /// Update return to patrol state
        /// Cập nhật trạng thái quay về tuần tra
        /// </summary>
        private void UpdateReturnState()
        {
            // Check for nearby enemies
            if (CheckForTarget())
            {
                SetState(EnemyState.Chase);
                return;
            }
            
            // Move back to spawn
            if (Vector3.Distance(transform.position, spawnPosition) < 2f)
            {
                SetState(EnemyState.Patrol);
            }
            else
            {
                MoveTowards(spawnPosition);
            }
        }
        
        /// <summary>
        /// Set new AI state
        /// Đặt trạng thái AI mới
        /// </summary>
        private void SetState(EnemyState newState)
        {
            currentState = newState;
            
            switch (newState)
            {
                case EnemyState.Idle:
                    stateTimer = patrolWaitTime;
                    StopMovement();
                    break;
                    
                case EnemyState.Patrol:
                    currentPatrolTarget = GetRandomPatrolPoint();
                    break;
                    
                case EnemyState.Chase:
                    break;
                    
                case EnemyState.Attack:
                    StopMovement();
                    break;
                    
                case EnemyState.Return:
                    targetEnemy = null;
                    break;
            }
        }
        
        /// <summary>
        /// Move towards target position
        /// Di chuyển về phía vị trí mục tiêu
        /// </summary>
        private void MoveTowards(Vector3 targetPosition)
        {
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.SetDestination(targetPosition);
                
                if (animator != null)
                {
                    animator.SetFloat(MoveSpeedHash, navAgent.velocity.magnitude);
                }
            }
            else
            {
                // Manual movement if no NavMeshAgent
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction.y = 0f;
                
                if (direction != Vector3.zero)
                {
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(direction), 
                        10f * Time.deltaTime);
                    
                    if (animator != null)
                    {
                        animator.SetFloat(MoveSpeedHash, moveSpeed);
                    }
                }
            }
        }
        
        /// <summary>
        /// Stop movement
        /// Dừng di chuyển
        /// </summary>
        private void StopMovement()
        {
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.ResetPath();
            }
            
            if (animator != null)
            {
                animator.SetFloat(MoveSpeedHash, 0f);
            }
        }
        
        /// <summary>
        /// Get random patrol point around spawn
        /// Lấy điểm tuần tra ngẫu nhiên quanh vị trí spawn
        /// </summary>
        private Vector3 GetRandomPatrolPoint()
        {
            Vector2 randomCircle = Random.insideUnitCircle * patrolRange;
            Vector3 randomPoint = spawnPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);
            return randomPoint;
        }
        
        /// <summary>
        /// Check for target in range
        /// Kiểm tra mục tiêu trong phạm vi
        /// </summary>
        private bool CheckForTarget()
        {
            if (!isAggressive) return false;
            
            Collider[] hits = Physics.OverlapSphere(transform.position, aggroRange);
            foreach (var hit in hits)
            {
                if (hit.CompareTag(Utils.Constants.TAG_PLAYER))
                {
                    targetEnemy = hit.gameObject;
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Find nearby player
        /// Tìm người chơi gần đó
        /// </summary>
        private void FindNearbyPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= aggroRange)
                {
                    targetEnemy = player;
                }
            }
        }
        
        /// <summary>
        /// Perform attack on target
        /// Thực hiện tấn công lên mục tiêu
        /// </summary>
        private void PerformAttack()
        {
            attackTimer = attackCooldown;
            
            if (animator != null)
            {
                animator.SetTrigger(AttackHash);
            }
            
            // Apply damage to target
            if (targetEnemy != null)
            {
                Character.CharacterStats targetStats = targetEnemy.GetComponent<Character.CharacterStats>();
                if (targetStats != null && !targetStats.IsDead)
                {
                    int damage = Combat.DamageCalculator.CalculatePhysicalDamage(
                        stats.physicalDamage,
                        targetStats.defense,
                        false
                    );
                    
                    targetStats.TakeDamage(damage);
                    Debug.Log($"{stats.enemyName} attacked player for {damage} damage!");
                }
            }
        }
        
        /// <summary>
        /// Called when enemy takes damage (from EnemyBase)
        /// Được gọi khi quái nhận sát thương (từ EnemyBase)
        /// </summary>
        public void OnTakeDamage()
        {
            // Aggro on attacker if not already in combat
            if (currentState == EnemyState.Idle || currentState == EnemyState.Patrol)
            {
                GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
                if (player != null)
                {
                    targetEnemy = player;
                    SetState(EnemyState.Chase);
                }
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            // Draw ranges
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, patrolRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRange);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
