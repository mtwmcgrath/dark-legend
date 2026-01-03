using UnityEngine;

namespace DarkLegend.Inventory
{
    /// <summary>
    /// Equipment slot types
    /// Loại ô trang bị
    /// </summary>
    public enum EquipmentSlotType
    {
        Weapon,         // Main weapon
        Helmet,         // Head armor
        Armor,          // Chest armor
        Gloves,         // Hand armor
        Pants,          // Leg armor
        Boots,          // Foot armor
        Wings,          // Wings (MU Online style)
        Accessory1,     // Ring/Necklace
        Accessory2      // Ring/Necklace
    }
    
    /// <summary>
    /// Equipment item class
    /// Class vật phẩm trang bị
    /// </summary>
    [System.Serializable]
    public class Equipment : Item
    {
        public EquipmentSlotType slotType;
        
        // Equipment stats
        public int damageBonus = 0;
        public int defenseBonus = 0;
        public int strengthBonus = 0;
        public int agilityBonus = 0;
        public int vitalityBonus = 0;
        public int energyBonus = 0;
        
        public int hpBonus = 0;
        public int mpBonus = 0;
        
        public float attackSpeedBonus = 0f;
        public float moveSpeedBonus = 0f;
        public float criticalChanceBonus = 0f;
        
        /// <summary>
        /// Equip this item to character
        /// Trang bị vật phẩm này cho nhân vật
        /// </summary>
        public void Equip(Character.CharacterStats stats)
        {
            if (stats == null) return;
            
            stats.strength += strengthBonus;
            stats.agility += agilityBonus;
            stats.vitality += vitalityBonus;
            stats.energy += energyBonus;
            
            stats.CalculateDerivedStats();
            
            Debug.Log($"Equipped: {itemName}");
        }
        
        /// <summary>
        /// Unequip this item from character
        /// Gỡ trang bị vật phẩm này khỏi nhân vật
        /// </summary>
        public void Unequip(Character.CharacterStats stats)
        {
            if (stats == null) return;
            
            stats.strength -= strengthBonus;
            stats.agility -= agilityBonus;
            stats.vitality -= vitalityBonus;
            stats.energy -= energyBonus;
            
            stats.CalculateDerivedStats();
            
            Debug.Log($"Unequipped: {itemName}");
        }
    }
}
