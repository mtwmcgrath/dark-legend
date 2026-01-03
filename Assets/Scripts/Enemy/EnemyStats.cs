using UnityEngine;

namespace DarkLegend.Enemy
{
    /// <summary>
    /// Enemy statistics (similar to CharacterStats but for enemies)
    /// Chỉ số quái vật (tương tự CharacterStats nhưng cho quái)
    /// </summary>
    public class EnemyStats : MonoBehaviour
    {
        [Header("Enemy Info")]
        public string enemyName = "Monster";
        public int level = 1;
        
        [Header("Stats")]
        public int strength = 10;
        public int agility = 10;
        public int vitality = 10;
        public int energy = 10;
        
        [Header("Derived Stats")]
        public int currentHP;
        public int maxHP;
        public float physicalDamage;
        public float defense;
        public float moveSpeed = 3f;
        
        [Header("Rewards")]
        public long expReward = 50;
        public int goldReward = 10;
        
        // Events
        public System.Action<int, int> OnHPChanged; // current, max
        public System.Action<int> OnDamageTaken;
        public System.Action OnDeath;
        
        private bool isDead = false;
        
        public bool IsDead => isDead;
        
        private void Start()
        {
            CalculateStats();
            currentHP = maxHP;
        }
        
        /// <summary>
        /// Calculate all stats based on level and base stats
        /// Tính toán tất cả chỉ số dựa trên level và chỉ số cơ bản
        /// </summary>
        public void CalculateStats()
        {
            // Scale stats with level
            float levelMultiplier = 1f + (level * 0.1f);
            
            maxHP = Mathf.RoundToInt((100 + vitality * 10) * levelMultiplier);
            physicalDamage = (strength * 1.2f) * levelMultiplier;
            defense = (vitality * 0.5f + strength * 0.3f) * levelMultiplier;
            
            // Scale rewards with level
            expReward = (long)(50 * level * levelMultiplier);
            goldReward = Mathf.RoundToInt(10 * level * levelMultiplier);
        }
        
        /// <summary>
        /// Take damage
        /// Nhận sát thương
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (isDead) return;
            
            currentHP -= damage;
            currentHP = Mathf.Max(0, currentHP);
            
            OnHPChanged?.Invoke(currentHP, maxHP);
            OnDamageTaken?.Invoke(damage);
            
            if (currentHP <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// Handle enemy death
        /// Xử lý khi quái chết
        /// </summary>
        private void Die()
        {
            isDead = true;
            OnDeath?.Invoke();
            Debug.Log($"{enemyName} has been defeated!");
        }
        
        /// <summary>
        /// Initialize stats from EnemyData
        /// Khởi tạo chỉ số từ EnemyData
        /// </summary>
        public void InitializeFromData(EnemyData data)
        {
            if (data == null) return;
            
            enemyName = data.enemyName;
            level = data.level;
            strength = data.strength;
            agility = data.agility;
            vitality = data.vitality;
            energy = data.energy;
            moveSpeed = data.moveSpeed;
            
            CalculateStats();
            currentHP = maxHP;
        }
    }
}
