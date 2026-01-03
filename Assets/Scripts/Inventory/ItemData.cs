using UnityEngine;

namespace DarkLegend.Inventory
{
    /// <summary>
    /// ScriptableObject for item data configuration
    /// ScriptableObject cho cấu hình dữ liệu vật phẩm
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Dark Legend/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemID;
        public string itemName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        
        [Header("Item Properties")]
        public ItemType itemType;
        public ItemRarity rarity;
        
        [Header("Stack Settings")]
        public bool isStackable = false;
        public int maxStackSize = 99;
        
        [Header("Economy")]
        public int buyPrice = 10;
        public int sellPrice = 5;
        
        [Header("Requirements")]
        public int requiredLevel = 1;
        public Character.CharacterClass[] allowedClasses;
        
        [Header("Consumable Settings")]
        public bool isConsumable = false;
        public int hpRestore = 0;
        public int mpRestore = 0;
        
        [Header("Prefab")]
        public GameObject worldPrefab; // 3D model when dropped in world
        
        /// <summary>
        /// Create item instance from this data
        /// Tạo instance vật phẩm từ dữ liệu này
        /// </summary>
        public Item CreateItemInstance()
        {
            Item item = new Item
            {
                itemID = this.itemID,
                itemName = this.itemName,
                description = this.description,
                itemType = this.itemType,
                rarity = this.rarity,
                icon = this.icon,
                isStackable = this.isStackable,
                stackSize = this.maxStackSize,
                currentStack = 1,
                buyPrice = this.buyPrice,
                sellPrice = this.sellPrice,
                requiredLevel = this.requiredLevel
            };
            
            return item;
        }
        
        /// <summary>
        /// Check if character class can use this item
        /// Kiểm tra class nhân vật có thể dùng vật phẩm này không
        /// </summary>
        public bool CanClassUse(Character.CharacterClass characterClass)
        {
            if (allowedClasses == null || allowedClasses.Length == 0)
            {
                return true; // No class restriction
            }
            
            foreach (var allowedClass in allowedClasses)
            {
                if (allowedClass == characterClass)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
