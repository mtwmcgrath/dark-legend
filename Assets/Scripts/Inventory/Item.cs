using UnityEngine;

namespace DarkLegend.Inventory
{
    /// <summary>
    /// Item types
    /// Loại vật phẩm
    /// </summary>
    public enum ItemType
    {
        Consumable,     // Potions, scrolls, etc.
        Equipment,      // Weapons, armor
        Material,       // Crafting materials
        Quest,          // Quest items
        Misc            // Other items
    }
    
    /// <summary>
    /// Item rarity
    /// Độ hiếm của vật phẩm
    /// </summary>
    public enum ItemRarity
    {
        Common,         // White
        Uncommon,       // Green
        Rare,           // Blue
        Epic,           // Purple
        Legendary       // Orange/Gold
    }
    
    /// <summary>
    /// Base item class
    /// Class cơ bản cho vật phẩm
    /// </summary>
    [System.Serializable]
    public class Item
    {
        public string itemID;
        public string itemName;
        public string description;
        public ItemType itemType;
        public ItemRarity rarity;
        public Sprite icon;
        
        public int stackSize = 1;
        public int currentStack = 1;
        public bool isStackable = false;
        
        public int buyPrice = 10;
        public int sellPrice = 5;
        
        public int requiredLevel = 1;
        
        /// <summary>
        /// Check if item can be stacked with another
        /// Kiểm tra vật phẩm có thể xếp chồng với vật phẩm khác không
        /// </summary>
        public bool CanStackWith(Item other)
        {
            if (other == null) return false;
            return isStackable && itemID == other.itemID && currentStack < stackSize;
        }
        
        /// <summary>
        /// Use/consume the item
        /// Sử dụng/tiêu thụ vật phẩm
        /// </summary>
        public virtual void Use(GameObject user)
        {
            Debug.Log($"Used item: {itemName}");
        }
        
        /// <summary>
        /// Get color based on rarity
        /// Lấy màu dựa trên độ hiếm
        /// </summary>
        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return Color.white;
                case ItemRarity.Uncommon:
                    return Color.green;
                case ItemRarity.Rare:
                    return Color.blue;
                case ItemRarity.Epic:
                    return new Color(0.6f, 0.2f, 0.8f); // Purple
                case ItemRarity.Legendary:
                    return new Color(1f, 0.6f, 0f); // Orange
                default:
                    return Color.white;
            }
        }
    }
}
