using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Projectile Skill - Skill bắn đạn/phép
    /// Projectile Skill - Projectile-based attack skills
    /// </summary>
    public class ProjectileSkill : ActiveSkill
    {
        [Header("Projectile Settings")]
        public float projectileSpeed = 20f;
        public bool canPierce = false;
        public int maxPierceTargets = 3;
        public bool homing = false;
        public float homingStrength = 5f;
        
        /// <summary>
        /// Execute projectile skill / Thực hiện projectile skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            if (skillData.projectilePrefab == null)
            {
                Debug.LogWarning($"No projectile prefab for skill: {skillData.skillName}");
                return;
            }
            
            // Spawn projectile
            SpawnProjectile(targetPosition, targetObject);
        }
        
        /// <summary>
        /// Spawn projectile / Tạo đạn
        /// </summary>
        protected virtual void SpawnProjectile(Vector3 targetPosition, GameObject targetObject)
        {
            // Vị trí spawn (từ character)
            Vector3 spawnPosition = owner.transform.position + Vector3.up * 1.5f;
            
            // Hướng bắn
            Vector3 direction = (targetPosition - spawnPosition).normalized;
            if (direction == Vector3.zero)
            {
                direction = owner.transform.forward;
            }
            
            // Rotation
            Quaternion rotation = Quaternion.LookRotation(direction);
            
            // Spawn projectile
            GameObject projectile = Instantiate(skillData.projectilePrefab, spawnPosition, rotation);
            
            // Setup projectile component
            Projectile projComponent = projectile.GetComponent<Projectile>();
            if (projComponent == null)
            {
                projComponent = projectile.AddComponent<Projectile>();
            }
            
            projComponent.Initialize(
                owner,
                this,
                direction,
                projectileSpeed,
                skillData.castRange,
                canPierce,
                maxPierceTargets,
                homing,
                homingStrength,
                targetObject
            );
            
            Debug.Log($"Projectile spawned: {skillData.skillName}");
        }
        
        /// <summary>
        /// Gọi khi projectile hit target / Called when projectile hits target
        /// </summary>
        public virtual void OnProjectileHit(GameObject target, Vector3 hitPosition)
        {
            if (target == null) return;
            
            CharacterStats targetStats = target.GetComponent<CharacterStats>();
            if (targetStats == null) return;
            
            CharacterStats ownerStats = owner.GetComponent<CharacterStats>();
            if (ownerStats == null) return;
            
            // Tính và apply damage
            float damage = CalculateDamage(ownerStats, targetStats);
            targetStats.currentHP = Mathf.Max(0, targetStats.currentHP - damage);
            
            // Spawn impact effect
            if (skillData.impactEffect != null)
            {
                GameObject effect = Instantiate(skillData.impactEffect, hitPosition, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Play impact sound
            if (skillData.impactSound != null)
            {
                AudioSource.PlayClipAtPoint(skillData.impactSound, hitPosition);
            }
            
            // Apply knockback nếu có
            if (skillData.knockbackForce > 0f)
            {
                ApplyKnockback(target, hitPosition);
            }
            
            Debug.Log($"Projectile hit {target.name} for {damage} damage");
        }
        
        /// <summary>
        /// Tính damage / Calculate damage
        /// </summary>
        protected virtual float CalculateDamage(CharacterStats attacker, CharacterStats defender)
        {
            float baseDamage = GetDamage();
            
            float statBonus = (attacker.STR * skillData.strRatio) + 
                            (attacker.ENE * skillData.eneRatio) +
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
        protected virtual void ApplyKnockback(GameObject target, Vector3 hitPosition)
        {
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb == null) return;
            
            Vector3 knockbackDirection = (target.transform.position - owner.transform.position).normalized;
            rb.AddForce(knockbackDirection * skillData.knockbackForce, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// Component cho projectile / Projectile component
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private GameObject owner;
        private ProjectileSkill skill;
        private Vector3 direction;
        private float speed;
        private float maxDistance;
        private bool canPierce;
        private int maxPierceTargets;
        private int currentPierceCount = 0;
        private bool homing;
        private float homingStrength;
        private GameObject targetObject;
        
        private Vector3 startPosition;
        private bool hasHit = false;
        
        /// <summary>
        /// Khởi tạo projectile / Initialize projectile
        /// </summary>
        public void Initialize(GameObject owner, ProjectileSkill skill, Vector3 direction, 
            float speed, float maxDistance, bool canPierce, int maxPierceTargets,
            bool homing, float homingStrength, GameObject targetObject)
        {
            this.owner = owner;
            this.skill = skill;
            this.direction = direction;
            this.speed = speed;
            this.maxDistance = maxDistance;
            this.canPierce = canPierce;
            this.maxPierceTargets = maxPierceTargets;
            this.homing = homing;
            this.homingStrength = homingStrength;
            this.targetObject = targetObject;
            
            startPosition = transform.position;
        }
        
        private void Update()
        {
            if (owner == null || skill == null)
            {
                Destroy(gameObject);
                return;
            }
            
            // Homing behavior
            if (homing && targetObject != null)
            {
                Vector3 targetDirection = (targetObject.transform.position - transform.position).normalized;
                direction = Vector3.Lerp(direction, targetDirection, homingStrength * Time.deltaTime);
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
            // Di chuyển
            transform.position += direction * speed * Time.deltaTime;
            
            // Kiểm tra max distance
            if (Vector3.Distance(startPosition, transform.position) > maxDistance)
            {
                Destroy(gameObject);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Không hit chính owner
            if (other.gameObject == owner) return;
            
            // Kiểm tra có phải enemy không
            if (!other.CompareTag("Enemy") && !other.CompareTag("Monster")) return;
            
            // Notify skill về hit
            skill.OnProjectileHit(other.gameObject, transform.position);
            
            // Nếu pierce, tăng counter
            if (canPierce)
            {
                currentPierceCount++;
                if (currentPierceCount >= maxPierceTargets)
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // Không pierce thì destroy ngay
                Destroy(gameObject);
            }
        }
    }
}
