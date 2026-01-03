using UnityEngine;
using System.Collections;

namespace DarkLegend.Mobile
{
    /// <summary>
    /// Auto-play system for mobile RPG
    /// Hệ thống tự động chơi cho RPG mobile
    /// </summary>
    public class AutoPlaySystem : MonoBehaviour
    {
        #region Singleton
        private static AutoPlaySystem instance;
        public static AutoPlaySystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AutoPlaySystem>();
                }
                return instance;
            }
        }
        #endregion

        [Header("Auto Features")]
        public bool autoMove = true;
        public bool autoAttack = true;
        public bool autoSkill = true;
        public bool autoPotion = true;
        public bool autoPickup = true;

        [Header("Auto Skill Settings")]
        public bool useSkillsInOrder = true;
        public float skillDelay = 0.5f;
        public bool saveUltimateForBoss = true;
        public int ultimateSkillIndex = 3;

        [Header("Auto Potion Settings")]
        public float hpPotionThreshold = 0.3f;
        public float mpPotionThreshold = 0.2f;

        [Header("Auto Pickup Settings")]
        public ItemRarity minPickupRarity = ItemRarity.Common;
        public bool pickupZen = true;
        public bool pickupJewels = true;

        [Header("Detection Settings")]
        public float detectionRadius = 15f;
        public float attackRange = 3f;
        public LayerMask enemyLayer;
        public LayerMask itemLayer;

        // State
        private bool isAutoPlayEnabled = false;
        private Transform playerTransform;
        private Transform currentTarget;
        private float nextSkillTime = 0f;
        private int currentSkillIndex = 0;

        public enum ItemRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        private void Update()
        {
            if (!isAutoPlayEnabled || playerTransform == null)
                return;

            AutoPlayUpdate();
        }

        /// <summary>
        /// Auto-play update loop
        /// Vòng lặp cập nhật auto-play
        /// </summary>
        private void AutoPlayUpdate()
        {
            // Auto pickup
            if (autoPickup)
            {
                CheckForPickup();
            }

            // Auto combat
            if (autoAttack || autoSkill)
            {
                if (currentTarget == null || !IsTargetValid(currentTarget))
                {
                    FindNearestEnemy();
                }

                if (currentTarget != null)
                {
                    // Auto move to target
                    if (autoMove)
                    {
                        MoveToTarget(currentTarget);
                    }

                    // Auto attack
                    if (autoAttack && IsInAttackRange(currentTarget))
                    {
                        PerformAttack();
                    }

                    // Auto skill
                    if (autoSkill && IsInAttackRange(currentTarget))
                    {
                        UseSkills();
                    }
                }
            }

            // Auto potion
            if (autoPotion)
            {
                CheckAndUsePotion();
            }
        }

        /// <summary>
        /// Find nearest enemy
        /// Tìm quái gần nhất
        /// </summary>
        private void FindNearestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, detectionRadius, enemyLayer);
            
            float closestDistance = float.MaxValue;
            Transform closestEnemy = null;

            foreach (Collider col in colliders)
            {
                float distance = Vector3.Distance(playerTransform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = col.transform;
                }
            }

            currentTarget = closestEnemy;
        }

        /// <summary>
        /// Check if target is valid
        /// Kiểm tra target có hợp lệ không
        /// </summary>
        private bool IsTargetValid(Transform target)
        {
            if (target == null)
                return false;

            // Check if still alive
            // TODO: Check enemy health component
            // EnemyHealth health = target.GetComponent<EnemyHealth>();
            // return health != null && !health.IsDead();

            return target.gameObject.activeSelf;
        }

        /// <summary>
        /// Move to target
        /// Di chuyển đến mục tiêu
        /// </summary>
        private void MoveToTarget(Transform target)
        {
            if (target == null)
                return;

            float distance = Vector3.Distance(playerTransform.position, target.position);
            
            if (distance > attackRange)
            {
                // Move towards target
                Vector3 direction = (target.position - playerTransform.position).normalized;
                
                // TODO: Use player movement system
                // PlayerMovement.Move(direction);
                Debug.Log($"[AutoPlaySystem] Moving to target at distance {distance:F1}");
            }
        }

        /// <summary>
        /// Check if in attack range
        /// Kiểm tra có trong tầm đánh không
        /// </summary>
        private bool IsInAttackRange(Transform target)
        {
            if (target == null)
                return false;

            float distance = Vector3.Distance(playerTransform.position, target.position);
            return distance <= attackRange;
        }

        /// <summary>
        /// Perform attack
        /// Thực hiện tấn công
        /// </summary>
        private void PerformAttack()
        {
            // TODO: Use player attack system
            // PlayerCombat.Attack();
            Debug.Log("[AutoPlaySystem] Auto attack");
        }

        /// <summary>
        /// Use skills
        /// Dùng skill
        /// </summary>
        private void UseSkills()
        {
            if (Time.time < nextSkillTime)
                return;

            // Check if should save ultimate for boss
            if (saveUltimateForBoss && currentSkillIndex == ultimateSkillIndex)
            {
                // TODO: Check if target is boss
                // bool isBoss = currentTarget.GetComponent<Boss>() != null;
                // if (!isBoss) return;
            }

            // Use skill
            // TODO: Use player skill system
            // PlayerSkillSystem.UseSkill(currentSkillIndex);
            Debug.Log($"[AutoPlaySystem] Using skill {currentSkillIndex}");

            nextSkillTime = Time.time + skillDelay;

            // Move to next skill
            if (useSkillsInOrder)
            {
                currentSkillIndex = (currentSkillIndex + 1) % 4; // Assume 4 skills
            }
        }

        /// <summary>
        /// Check and use potion
        /// Kiểm tra và dùng potion
        /// </summary>
        private void CheckAndUsePotion()
        {
            // TODO: Check player stats
            // float hpPercent = PlayerStats.CurrentHP / PlayerStats.MaxHP;
            // float mpPercent = PlayerStats.CurrentMP / PlayerStats.MaxMP;

            // Simulate for now
            float hpPercent = 1.0f;
            float mpPercent = 1.0f;

            if (hpPercent < hpPotionThreshold)
            {
                UseHPPotion();
            }

            if (mpPercent < mpPotionThreshold)
            {
                UseMPPotion();
            }
        }

        /// <summary>
        /// Use HP potion
        /// Dùng potion HP
        /// </summary>
        private void UseHPPotion()
        {
            // TODO: Use inventory system
            // InventorySystem.UsePotion(PotionType.HP);
            Debug.Log("[AutoPlaySystem] Using HP potion");
        }

        /// <summary>
        /// Use MP potion
        /// Dùng potion MP
        /// </summary>
        private void UseMPPotion()
        {
            // TODO: Use inventory system
            // InventorySystem.UsePotion(PotionType.MP);
            Debug.Log("[AutoPlaySystem] Using MP potion");
        }

        /// <summary>
        /// Check for pickup
        /// Kiểm tra vật phẩm để nhặt
        /// </summary>
        private void CheckForPickup()
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, attackRange, itemLayer);
            
            foreach (Collider col in colliders)
            {
                // TODO: Check item rarity and type
                // Item item = col.GetComponent<Item>();
                // if (item != null && ShouldPickupItem(item))
                // {
                //     item.Pickup();
                // }
                
                Debug.Log($"[AutoPlaySystem] Picking up item: {col.name}");
            }
        }

        /// <summary>
        /// Enable auto-play
        /// Bật auto-play
        /// </summary>
        public void EnableAutoPlay()
        {
            isAutoPlayEnabled = true;
            Debug.Log("[AutoPlaySystem] Auto-play enabled");
        }

        /// <summary>
        /// Disable auto-play
        /// Tắt auto-play
        /// </summary>
        public void DisableAutoPlay()
        {
            isAutoPlayEnabled = false;
            currentTarget = null;
            Debug.Log("[AutoPlaySystem] Auto-play disabled");
        }

        /// <summary>
        /// Toggle auto-play
        /// Bật/tắt auto-play
        /// </summary>
        public void ToggleAutoPlay()
        {
            if (isAutoPlayEnabled)
            {
                DisableAutoPlay();
            }
            else
            {
                EnableAutoPlay();
            }
        }

        /// <summary>
        /// Is auto-play enabled
        /// Auto-play có được bật không
        /// </summary>
        public bool IsAutoPlayEnabled()
        {
            return isAutoPlayEnabled;
        }
    }
}
