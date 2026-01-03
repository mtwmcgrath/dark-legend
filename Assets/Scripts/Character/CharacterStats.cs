using UnityEngine;
using System;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character statistics system
    /// Hệ thống chỉ số nhân vật
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Character Info")]
        public string characterName = "Hero";
        public CharacterClass characterClass;
        public CharacterClassData classData;
        
        [Header("Core Stats")]
        public int strength;      // STR - Physical damage, carry weight
        public int agility;       // AGI - Attack speed, defense rate, movement
        public int vitality;      // VIT - HP, HP recovery
        public int energy;        // ENE - MP, magic damage, MP recovery
        
        [Header("Derived Stats")]
        public int currentHP;
        public int maxHP;
        public int currentMP;
        public int maxMP;
        
        [Header("Combat Stats")]
        public float physicalDamage;
        public float magicDamage;
        public float attackSpeed;
        public float defense;
        public float defenseRate;
        public float criticalChance;
        public float moveSpeed;
        
        [Header("Stat Points")]
        public int availableStatPoints = 0;
        
        // Events
        public event Action<int, int> OnHPChanged;  // current, max
        public event Action<int, int> OnMPChanged;  // current, max
        public event Action<int> OnDamageTaken;
        public event Action OnDeath;
        public event Action OnStatsChanged;
        
        private bool isDead = false;
        
        public bool IsDead => isDead;
        
        private void Start()
        {
            InitializeStats();
        }
        
        /// <summary>
        /// Initialize character stats based on class data
        /// Khởi tạo chỉ số nhân vật dựa trên dữ liệu class
        /// </summary>
        public void InitializeStats()
        {
            if (classData != null)
            {
                strength = classData.baseStrength;
                agility = classData.baseAgility;
                vitality = classData.baseVitality;
                energy = classData.baseEnergy;
            }
            
            CalculateDerivedStats();
            currentHP = maxHP;
            currentMP = maxMP;
        }
        
        /// <summary>
        /// Calculate all derived stats from core stats
        /// Tính toán tất cả chỉ số phát sinh từ chỉ số cơ bản
        /// </summary>
        public void CalculateDerivedStats()
        {
            // HP and MP
            maxHP = Mathf.RoundToInt(100 + (vitality * Utils.Constants.HP_PER_VITALITY));
            maxMP = Mathf.RoundToInt(50 + (energy * Utils.Constants.MP_PER_ENERGY));
            
            // Physical Damage
            float baseDamage = strength * Utils.Constants.DAMAGE_PER_STRENGTH;
            physicalDamage = classData != null ? baseDamage * classData.physicalDamageMultiplier : baseDamage;
            
            // Magic Damage
            float baseMagicDmg = energy * 1.5f;
            magicDamage = classData != null ? baseMagicDmg * classData.magicDamageMultiplier : baseMagicDmg;
            
            // Attack Speed
            float baseAttackSpeed = 1.0f + (agility * 0.01f);
            attackSpeed = classData != null ? baseAttackSpeed * classData.attackSpeedMultiplier : baseAttackSpeed;
            
            // Defense
            float baseDef = vitality * 0.5f + strength * 0.3f;
            defense = classData != null ? baseDef * classData.defenseMultiplier : baseDef;
            
            // Defense Rate (% chance to reduce damage)
            defenseRate = agility * 0.3f;
            
            // Critical Chance
            criticalChance = Utils.Constants.CRITICAL_HIT_CHANCE + (agility * 0.001f);
            
            // Move Speed
            moveSpeed = Utils.Constants.DEFAULT_MOVE_SPEED + (agility * 0.01f);
            
            OnStatsChanged?.Invoke();
        }
        
        /// <summary>
        /// Add points to a specific stat
        /// Thêm điểm vào một chỉ số cụ thể
        /// </summary>
        public bool AddStatPoint(string statName, int points = 1)
        {
            if (availableStatPoints < points) return false;
            
            switch (statName.ToLower())
            {
                case "strength":
                case "str":
                    strength += points;
                    break;
                case "agility":
                case "agi":
                    agility += points;
                    break;
                case "vitality":
                case "vit":
                    vitality += points;
                    break;
                case "energy":
                case "ene":
                    energy += points;
                    break;
                default:
                    return false;
            }
            
            availableStatPoints -= points;
            CalculateDerivedStats();
            return true;
        }
        
        /// <summary>
        /// Take damage and handle death
        /// Nhận sát thương và xử lý tử vong
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
        /// Heal HP
        /// Hồi máu
        /// </summary>
        public void Heal(int amount)
        {
            if (isDead) return;
            
            currentHP += amount;
            currentHP = Mathf.Min(currentHP, maxHP);
            OnHPChanged?.Invoke(currentHP, maxHP);
        }
        
        /// <summary>
        /// Consume MP for skills
        /// Tiêu hao MP cho skills
        /// </summary>
        public bool ConsumeMP(int amount)
        {
            if (currentMP < amount) return false;
            
            currentMP -= amount;
            OnMPChanged?.Invoke(currentMP, maxMP);
            return true;
        }
        
        /// <summary>
        /// Restore MP
        /// Hồi phục MP
        /// </summary>
        public void RestoreMP(int amount)
        {
            currentMP += amount;
            currentMP = Mathf.Min(currentMP, maxMP);
            OnMPChanged?.Invoke(currentMP, maxMP);
        }
        
        /// <summary>
        /// Handle character death
        /// Xử lý khi nhân vật chết
        /// </summary>
        private void Die()
        {
            isDead = true;
            OnDeath?.Invoke();
            Debug.Log($"{characterName} has died!");
        }
        
        /// <summary>
        /// Revive character
        /// Hồi sinh nhân vật
        /// </summary>
        public void Revive()
        {
            isDead = false;
            currentHP = maxHP;
            currentMP = maxMP;
            OnHPChanged?.Invoke(currentHP, maxHP);
            OnMPChanged?.Invoke(currentMP, maxMP);
        }
        
        /// <summary>
        /// Get stat value by name
        /// Lấy giá trị chỉ số theo tên
        /// </summary>
        public int GetStat(string statName)
        {
            switch (statName.ToLower())
            {
                case "strength":
                case "str":
                    return strength;
                case "agility":
                case "agi":
                    return agility;
                case "vitality":
                case "vit":
                    return vitality;
                case "energy":
                case "ene":
                    return energy;
                default:
                    return 0;
            }
        }
    }
}
