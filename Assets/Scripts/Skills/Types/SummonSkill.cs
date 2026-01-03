using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Summon Skill - Skill triệu hồi
    /// Summon Skill - Summoning abilities
    /// </summary>
    public class SummonSkill : ActiveSkill
    {
        [Header("Summon Settings")]
        public GameObject summonPrefab;
        public int maxSummons = 1;           // Số summon tối đa cùng lúc
        public float summonDuration = 60f;   // Thời gian tồn tại (0 = vĩnh viễn)
        public float summonRadius = 2f;      // Khoảng cách spawn từ caster
        public SummonBehavior summonBehavior = SummonBehavior.FollowAndAttack;
        
        [Header("Summon Stats")]
        public float summonHP = 100f;
        public float summonDamage = 20f;
        public float summonDefense = 10f;
        public float hpScalePerLevel = 10f;
        public float damageScalePerLevel = 2f;
        
        private List<GameObject> activeSummons = new List<GameObject>();
        
        /// <summary>
        /// Execute summon skill / Thực hiện summon skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            // Xóa summons cũ nếu đã đạt max
            while (activeSummons.Count >= maxSummons)
            {
                RemoveSummon(0);
            }
            
            // Spawn summon
            SpawnSummon(targetPosition);
        }
        
        /// <summary>
        /// Spawn một summon / Spawn a summon
        /// </summary>
        protected virtual void SpawnSummon(Vector3 targetPosition)
        {
            if (summonPrefab == null)
            {
                Debug.LogWarning($"No summon prefab for skill: {skillData.skillName}");
                return;
            }
            
            // Tính vị trí spawn
            Vector3 spawnPosition = owner.transform.position + 
                owner.transform.forward * summonRadius +
                Vector3.up * 0.5f;
            
            // Random offset nhỏ
            spawnPosition += new Vector3(
                Random.Range(-1f, 1f),
                0f,
                Random.Range(-1f, 1f)
            );
            
            // Spawn summon
            GameObject summon = Instantiate(summonPrefab, spawnPosition, Quaternion.identity);
            
            // Setup summon component
            SummonedCreature creature = summon.GetComponent<SummonedCreature>();
            if (creature == null)
            {
                creature = summon.AddComponent<SummonedCreature>();
            }
            
            // Tính stats dựa trên level
            float hp = summonHP + (hpScalePerLevel * (currentLevel - 1));
            float damage = summonDamage + (damageScalePerLevel * (currentLevel - 1));
            
            creature.Initialize(
                owner,
                this,
                hp,
                damage,
                summonDefense,
                summonDuration,
                summonBehavior
            );
            
            // Thêm vào list
            activeSummons.Add(summon);
            
            // Spawn effect
            if (skillData.impactEffect != null)
            {
                GameObject effect = Instantiate(skillData.impactEffect, spawnPosition, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            Debug.Log($"Summoned creature: {skillData.skillName} (Level {currentLevel})");
        }
        
        /// <summary>
        /// Xóa summon / Remove a summon
        /// </summary>
        protected virtual void RemoveSummon(int index)
        {
            if (index < 0 || index >= activeSummons.Count) return;
            
            GameObject summon = activeSummons[index];
            if (summon != null)
            {
                // Spawn despawn effect
                if (skillData.impactEffect != null)
                {
                    Instantiate(skillData.impactEffect, summon.transform.position, Quaternion.identity);
                }
                
                Destroy(summon);
            }
            
            activeSummons.RemoveAt(index);
        }
        
        /// <summary>
        /// Xóa tất cả summons / Remove all summons
        /// </summary>
        public virtual void RemoveAllSummons()
        {
            while (activeSummons.Count > 0)
            {
                RemoveSummon(0);
            }
        }
        
        /// <summary>
        /// Update để cleanup dead summons / Update to cleanup dead summons
        /// </summary>
        protected override void Update()
        {
            base.Update();
            
            // Cleanup dead/null summons
            for (int i = activeSummons.Count - 1; i >= 0; i--)
            {
                if (activeSummons[i] == null)
                {
                    activeSummons.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Cleanup khi destroy / Cleanup on destroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            RemoveAllSummons();
        }
    }
    
    /// <summary>
    /// Hành vi của summon / Summon behavior
    /// </summary>
    public enum SummonBehavior
    {
        Idle,              // Đứng yên
        Follow,            // Đi theo chủ
        FollowAndAttack,   // Đi theo và tấn công
        AttackTarget,      // Tấn công target cụ thể
        Patrol             // Tuần tra
    }
    
    /// <summary>
    /// Component cho summoned creature / Summoned creature component
    /// </summary>
    public class SummonedCreature : MonoBehaviour
    {
        public GameObject owner;
        public SummonSkill summonSkill;
        
        public float maxHP;
        public float currentHP;
        public float damage;
        public float defense;
        public float duration;
        public SummonBehavior behavior;
        
        private float remainingLifetime;
        private GameObject currentTarget;
        private float attackCooldown = 1f;
        private float nextAttackTime = 0f;
        
        public void Initialize(GameObject owner, SummonSkill skill, float hp, float damage, 
            float defense, float duration, SummonBehavior behavior)
        {
            this.owner = owner;
            this.summonSkill = skill;
            this.maxHP = hp;
            this.currentHP = hp;
            this.damage = damage;
            this.defense = defense;
            this.duration = duration;
            this.behavior = behavior;
            
            this.remainingLifetime = duration;
            
            // Setup tag
            gameObject.tag = "Summon";
        }
        
        private void Update()
        {
            if (owner == null)
            {
                Die();
                return;
            }
            
            // Countdown lifetime
            if (duration > 0)
            {
                remainingLifetime -= Time.deltaTime;
                if (remainingLifetime <= 0f)
                {
                    Die();
                    return;
                }
            }
            
            // Check HP
            if (currentHP <= 0)
            {
                Die();
                return;
            }
            
            // Update behavior
            UpdateBehavior();
        }
        
        private void UpdateBehavior()
        {
            switch (behavior)
            {
                case SummonBehavior.Idle:
                    // Không làm gì
                    break;
                    
                case SummonBehavior.Follow:
                    FollowOwner();
                    break;
                    
                case SummonBehavior.FollowAndAttack:
                    if (currentTarget == null || !IsValidTarget(currentTarget))
                    {
                        FindNearestEnemy();
                    }
                    
                    if (currentTarget != null)
                    {
                        AttackTarget();
                    }
                    else
                    {
                        FollowOwner();
                    }
                    break;
                    
                case SummonBehavior.AttackTarget:
                    if (currentTarget != null)
                    {
                        AttackTarget();
                    }
                    break;
            }
        }
        
        private void FollowOwner()
        {
            float distanceToOwner = Vector3.Distance(transform.position, owner.transform.position);
            
            if (distanceToOwner > 3f)
            {
                Vector3 direction = (owner.transform.position - transform.position).normalized;
                transform.position += direction * 3f * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        
        private void FindNearestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);
            
            float nearestDistance = float.MaxValue;
            GameObject nearestEnemy = null;
            
            foreach (Collider col in colliders)
            {
                if (IsValidTarget(col.gameObject))
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestEnemy = col.gameObject;
                    }
                }
            }
            
            currentTarget = nearestEnemy;
        }
        
        private void AttackTarget()
        {
            if (currentTarget == null) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            
            // Di chuyển đến target
            if (distanceToTarget > 2f)
            {
                Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                transform.position += direction * 3f * Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(direction);
            }
            else
            {
                // Tấn công
                if (Time.time >= nextAttackTime)
                {
                    DealDamage();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        
        private void DealDamage()
        {
            if (currentTarget == null) return;
            
            CharacterStats targetStats = currentTarget.GetComponent<CharacterStats>();
            if (targetStats == null) return;
            
            float actualDamage = damage;
            
            // Apply defense
            float damageReduction = targetStats.defense / (targetStats.defense + 100f);
            actualDamage *= (1f - damageReduction);
            
            targetStats.currentHP = Mathf.Max(0, targetStats.currentHP - actualDamage);
            
            Debug.Log($"Summon dealt {actualDamage} damage to {currentTarget.name}");
        }
        
        private bool IsValidTarget(GameObject target)
        {
            if (target == owner) return false;
            if (target == gameObject) return false;
            
            CharacterStats stats = target.GetComponent<CharacterStats>();
            if (stats == null || stats.currentHP <= 0) return false;
            
            return target.CompareTag("Enemy") || target.CompareTag("Monster");
        }
        
        public void TakeDamage(float damage)
        {
            float actualDamage = damage;
            
            // Apply defense
            float damageReduction = defense / (defense + 100f);
            actualDamage *= (1f - damageReduction);
            
            currentHP -= actualDamage;
            
            Debug.Log($"Summon took {actualDamage} damage (HP: {currentHP}/{maxHP})");
        }
        
        private void Die()
        {
            Debug.Log("Summon died");
            Destroy(gameObject);
        }
    }
}
