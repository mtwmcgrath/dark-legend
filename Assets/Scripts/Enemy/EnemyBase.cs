using UnityEngine;

namespace DarkLegend.Enemy
{
    /// <summary>
    /// Base class for all enemies
    /// Lớp cơ sở cho tất cả quái vật
    /// </summary>
    [RequireComponent(typeof(EnemyStats))]
    [RequireComponent(typeof(EnemyAI))]
    public class EnemyBase : MonoBehaviour
    {
        [Header("Enemy Data")]
        public EnemyData enemyData;
        
        [Header("References")]
        public EnemyStats stats;
        public EnemyAI ai;
        public Animator animator;
        
        [Header("Components")]
        public Collider mainCollider;
        public Rigidbody rb;
        
        private void Awake()
        {
            // Get components
            stats = GetComponent<EnemyStats>();
            ai = GetComponent<EnemyAI>();
            
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            
            if (mainCollider == null)
            {
                mainCollider = GetComponent<Collider>();
            }
            
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
        }
        
        private void Start()
        {
            InitializeEnemy();
            
            // Subscribe to death event
            if (stats != null)
            {
                stats.OnDeath += OnDeath;
            }
        }
        
        /// <summary>
        /// Initialize enemy from data
        /// Khởi tạo quái vật từ dữ liệu
        /// </summary>
        private void InitializeEnemy()
        {
            if (enemyData != null)
            {
                // Set tag
                gameObject.tag = Utils.Constants.TAG_ENEMY;
                
                // Initialize stats
                if (stats != null)
                {
                    stats.InitializeFromData(enemyData);
                }
                
                // Initialize AI
                if (ai != null)
                {
                    ai.InitializeFromData(enemyData);
                }
                
                // Set animator controller
                if (animator != null && enemyData.animatorController != null)
                {
                    animator.runtimeAnimatorController = enemyData.animatorController;
                }
            }
        }
        
        /// <summary>
        /// Handle enemy death
        /// Xử lý khi quái vật chết
        /// </summary>
        private void OnDeath()
        {
            // Disable AI
            if (ai != null)
            {
                ai.enabled = false;
            }
            
            // Play death animation
            if (animator != null)
            {
                animator.SetBool(Utils.Constants.ANIM_IS_DEAD, true);
            }
            
            // Disable collider
            if (mainCollider != null)
            {
                mainCollider.enabled = false;
            }
            
            // Disable rigidbody
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            
            // Give rewards to player
            GiveRewards();
            
            // Destroy after delay
            Destroy(gameObject, 5f);
        }
        
        /// <summary>
        /// Give experience and loot to player
        /// Trao kinh nghiệm và phần thưởng cho người chơi
        /// </summary>
        private void GiveRewards()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
            if (player != null)
            {
                // Give EXP
                Character.LevelSystem levelSystem = player.GetComponent<Character.LevelSystem>();
                if (levelSystem != null && stats != null)
                {
                    levelSystem.AddExp(stats.expReward);
                    Debug.Log($"Player gained {stats.expReward} EXP!");
                }
                
                // TODO: Give gold and items when inventory system is implemented
            }
        }
        
        /// <summary>
        /// Take damage (public method for other systems to call)
        /// Nhận sát thương (phương thức public cho các hệ thống khác gọi)
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (stats != null)
            {
                stats.TakeDamage(damage);
                
                // Notify AI
                if (ai != null)
                {
                    ai.OnTakeDamage();
                }
            }
        }
        
        private void OnDestroy()
        {
            if (stats != null)
            {
                stats.OnDeath -= OnDeath;
            }
        }
    }
}
