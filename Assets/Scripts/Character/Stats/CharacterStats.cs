using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character stats system / Hệ thống chỉ số nhân vật
    /// </summary>
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Base Stats / Chỉ số cơ bản")]
        public int Strength;      // Physical damage, carry capacity / Sát thương vật lý, sức mang vác
        public int Agility;       // Attack speed, defense rate, speed / Tốc độ tấn công, tỷ lệ né, tốc độ
        public int Vitality;      // HP, HP recovery / HP, hồi HP
        public int Energy;        // MP, magic damage, MP recovery / MP, sát thương ma thuật, hồi MP
        public int Command;       // Pet/summon power (Dark Lord only) / Sức mạnh pet/triệu hồi (chỉ Dark Lord)
        
        [Header("Level & Points / Cấp độ & Điểm")]
        public int Level = 1;
        public int FreePoints;    // Points to allocate / Điểm để phân bổ
        public int PointsPerLevel = 5;
        
        [Header("Modifiers / Bổ trợ")]
        public List<StatModifier> Modifiers = new List<StatModifier>();
        
        // Events for stat changes / Sự kiện khi chỉ số thay đổi
        public event Action<string, int> OnStatChanged;
        public event Action<int> OnLevelUp;
        
        /// <summary>
        /// Calculate maximum HP / Tính HP tối đa
        /// </summary>
        public int MaxHP => CalculateMaxHP();
        
        /// <summary>
        /// Calculate maximum MP / Tính MP tối đa
        /// </summary>
        public int MaxMP => CalculateMaxMP();
        
        /// <summary>
        /// Calculate physical damage / Tính sát thương vật lý
        /// </summary>
        public int PhysicalDamage => CalculatePhysDamage();
        
        /// <summary>
        /// Calculate magic damage / Tính sát thương ma thuật
        /// </summary>
        public int MagicDamage => CalculateMagicDamage();
        
        /// <summary>
        /// Calculate defense / Tính phòng thủ
        /// </summary>
        public int Defense => CalculateDefense();
        
        /// <summary>
        /// Calculate defense rate / Tính tỷ lệ phòng thủ
        /// </summary>
        public int DefenseRate => CalculateDefenseRate();
        
        /// <summary>
        /// Calculate attack speed / Tính tốc độ tấn công
        /// </summary>
        public float AttackSpeed => CalculateAttackSpeed();
        
        /// <summary>
        /// Calculate movement speed / Tính tốc độ di chuyển
        /// </summary>
        public float MovementSpeed => CalculateMoveSpeed();
        
        /// <summary>
        /// Calculate critical rate / Tính tỷ lệ chí mạng
        /// </summary>
        public float CriticalRate => CalculateCritRate();
        
        /// <summary>
        /// Calculate critical damage / Tính sát thương chí mạng
        /// </summary>
        public float CriticalDamage => CalculateCritDamage();
        
        private int CalculateMaxHP()
        {
            int baseHP = 100 + (Vitality * 5) + (Level * 2);
            return ApplyModifiers("MaxHP", baseHP);
        }
        
        private int CalculateMaxMP()
        {
            int baseMP = 50 + (Energy * 3) + (Level * 1);
            return ApplyModifiers("MaxMP", baseMP);
        }
        
        private int CalculatePhysDamage()
        {
            int baseDamage = Strength / 4;
            return ApplyModifiers("PhysicalDamage", baseDamage);
        }
        
        private int CalculateMagicDamage()
        {
            int baseDamage = Energy / 3;
            return ApplyModifiers("MagicDamage", baseDamage);
        }
        
        private int CalculateDefense()
        {
            int baseDefense = (Agility / 3) + (Vitality / 5);
            return ApplyModifiers("Defense", baseDefense);
        }
        
        private int CalculateDefenseRate()
        {
            int baseRate = Agility / 3;
            return ApplyModifiers("DefenseRate", baseRate);
        }
        
        private float CalculateAttackSpeed()
        {
            float baseSpeed = 1.0f + (Agility / 100f);
            return ApplyModifiersFloat("AttackSpeed", baseSpeed);
        }
        
        private float CalculateMoveSpeed()
        {
            float baseSpeed = 5.0f + (Agility / 50f);
            return ApplyModifiersFloat("MovementSpeed", baseSpeed);
        }
        
        private float CalculateCritRate()
        {
            float baseRate = (Agility / 10f) + (Strength / 20f);
            return ApplyModifiersFloat("CriticalRate", baseRate);
        }
        
        private float CalculateCritDamage()
        {
            float baseDamage = 150f + (Strength / 10f);
            return ApplyModifiersFloat("CriticalDamage", baseDamage);
        }
        
        private int ApplyModifiers(string statName, int baseValue)
        {
            float finalValue = baseValue;
            foreach (var modifier in Modifiers)
            {
                if (modifier.StatName == statName)
                {
                    finalValue = modifier.ApplyModifier(finalValue);
                }
            }
            return Mathf.RoundToInt(finalValue);
        }
        
        private float ApplyModifiersFloat(string statName, float baseValue)
        {
            float finalValue = baseValue;
            foreach (var modifier in Modifiers)
            {
                if (modifier.StatName == statName)
                {
                    finalValue = modifier.ApplyModifier(finalValue);
                }
            }
            return finalValue;
        }
        
        /// <summary>
        /// Add stat points / Thêm điểm chỉ số
        /// </summary>
        public bool AddStatPoint(string statName, int amount = 1)
        {
            if (FreePoints < amount)
                return false;
                
            switch (statName)
            {
                case "Strength":
                    Strength += amount;
                    break;
                case "Agility":
                    Agility += amount;
                    break;
                case "Vitality":
                    Vitality += amount;
                    break;
                case "Energy":
                    Energy += amount;
                    break;
                case "Command":
                    Command += amount;
                    break;
                default:
                    return false;
            }
            
            FreePoints -= amount;
            OnStatChanged?.Invoke(statName, amount);
            return true;
        }
        
        /// <summary>
        /// Level up character / Tăng cấp nhân vật
        /// </summary>
        public void LevelUp()
        {
            Level++;
            FreePoints += PointsPerLevel;
            OnLevelUp?.Invoke(Level);
        }
        
        /// <summary>
        /// Add modifier / Thêm bổ trợ
        /// </summary>
        public void AddModifier(StatModifier modifier)
        {
            Modifiers.Add(modifier);
            OnStatChanged?.Invoke(modifier.StatName, 0);
        }
        
        /// <summary>
        /// Remove modifier / Xóa bổ trợ
        /// </summary>
        public void RemoveModifier(StatModifier modifier)
        {
            Modifiers.Remove(modifier);
            OnStatChanged?.Invoke(modifier.StatName, 0);
        }
        
        /// <summary>
        /// Clear all modifiers / Xóa tất cả bổ trợ
        /// </summary>
        public void ClearModifiers()
        {
            Modifiers.Clear();
        }
    }
}
