using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Calculator for character stats / Máy tính chỉ số nhân vật
    /// </summary>
    public static class StatCalculator
    {
        // HP calculation constants / Hằng số tính HP
        private const int BASE_HP = 100;
        private const int HP_PER_VIT = 5;
        private const int HP_PER_LEVEL = 2;
        
        // MP calculation constants / Hằng số tính MP
        private const int BASE_MP = 50;
        private const int MP_PER_ENE = 3;
        private const int MP_PER_LEVEL = 1;
        
        // Damage calculation constants / Hằng số tính sát thương
        private const int STR_TO_PHYS_DMG = 4;
        private const int ENE_TO_MAGIC_DMG = 3;
        
        // Defense calculation constants / Hằng số tính phòng thủ
        private const int AGI_TO_DEF = 3;
        private const int VIT_TO_DEF = 5;
        private const int AGI_TO_DEF_RATE = 3;
        
        // Speed calculation constants / Hằng số tính tốc độ
        private const float BASE_ATTACK_SPEED = 1.0f;
        private const float AGI_TO_ATTACK_SPEED = 100f;
        private const float BASE_MOVE_SPEED = 5.0f;
        private const float AGI_TO_MOVE_SPEED = 50f;
        
        // Critical calculation constants / Hằng số tính chí mạng
        private const float AGI_TO_CRIT_RATE = 10f;
        private const float STR_TO_CRIT_RATE = 20f;
        private const float BASE_CRIT_DMG = 150f;
        private const float STR_TO_CRIT_DMG = 10f;
        
        /// <summary>
        /// Calculate maximum HP / Tính HP tối đa
        /// </summary>
        public static int CalculateMaxHP(int vitality, int level)
        {
            return BASE_HP + (vitality * HP_PER_VIT) + (level * HP_PER_LEVEL);
        }
        
        /// <summary>
        /// Calculate maximum MP / Tính MP tối đa
        /// </summary>
        public static int CalculateMaxMP(int energy, int level)
        {
            return BASE_MP + (energy * MP_PER_ENE) + (level * MP_PER_LEVEL);
        }
        
        /// <summary>
        /// Calculate physical damage / Tính sát thương vật lý
        /// </summary>
        public static int CalculatePhysicalDamage(int strength)
        {
            return strength / STR_TO_PHYS_DMG;
        }
        
        /// <summary>
        /// Calculate magic damage / Tính sát thương ma thuật
        /// </summary>
        public static int CalculateMagicDamage(int energy)
        {
            return energy / ENE_TO_MAGIC_DMG;
        }
        
        /// <summary>
        /// Calculate defense / Tính phòng thủ
        /// </summary>
        public static int CalculateDefense(int agility, int vitality)
        {
            return (agility / AGI_TO_DEF) + (vitality / VIT_TO_DEF);
        }
        
        /// <summary>
        /// Calculate defense rate / Tính tỷ lệ phòng thủ
        /// </summary>
        public static int CalculateDefenseRate(int agility)
        {
            return agility / AGI_TO_DEF_RATE;
        }
        
        /// <summary>
        /// Calculate attack speed / Tính tốc độ tấn công
        /// </summary>
        public static float CalculateAttackSpeed(int agility)
        {
            return BASE_ATTACK_SPEED + (agility / AGI_TO_ATTACK_SPEED);
        }
        
        /// <summary>
        /// Calculate movement speed / Tính tốc độ di chuyển
        /// </summary>
        public static float CalculateMovementSpeed(int agility)
        {
            return BASE_MOVE_SPEED + (agility / AGI_TO_MOVE_SPEED);
        }
        
        /// <summary>
        /// Calculate critical rate / Tính tỷ lệ chí mạng
        /// </summary>
        public static float CalculateCriticalRate(int agility, int strength)
        {
            return (agility / AGI_TO_CRIT_RATE) + (strength / STR_TO_CRIT_RATE);
        }
        
        /// <summary>
        /// Calculate critical damage / Tính sát thương chí mạng
        /// </summary>
        public static float CalculateCriticalDamage(int strength)
        {
            return BASE_CRIT_DMG + (strength / STR_TO_CRIT_DMG);
        }
        
        /// <summary>
        /// Calculate carry capacity / Tính sức mang vác
        /// </summary>
        public static int CalculateCarryCapacity(int strength)
        {
            return 100 + (strength * 2);
        }
        
        /// <summary>
        /// Calculate HP recovery per second / Tính hồi HP mỗi giây
        /// </summary>
        public static float CalculateHPRecovery(int vitality)
        {
            return vitality * 0.1f;
        }
        
        /// <summary>
        /// Calculate MP recovery per second / Tính hồi MP mỗi giây
        /// </summary>
        public static float CalculateMPRecovery(int energy)
        {
            return energy * 0.05f;
        }
        
        /// <summary>
        /// Calculate pet/summon damage (Dark Lord only) / Tính sát thương pet/triệu hồi (chỉ Dark Lord)
        /// </summary>
        public static int CalculatePetDamage(int command)
        {
            return command / 2;
        }
    }
}
