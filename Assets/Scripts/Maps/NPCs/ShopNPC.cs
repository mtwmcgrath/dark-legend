using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.NPCs
{
    /// <summary>
    /// NPC bán đồ / Shop vendor NPC
    /// </summary>
    public class ShopNPC : NPCBase
    {
        [Header("Shop Configuration")]
        [Tooltip("Loại shop / Shop type")]
        [SerializeField] private ShopType shopType = ShopType.General;
        
        [Tooltip("Danh sách items bán / Items for sale")]
        [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
        
        [Tooltip("Giảm giá / Discount percentage")]
        [Range(0f, 1f)]
        [SerializeField] private float discountPercentage = 0f;
        
        [Tooltip("Mua lại items / Buy back items")]
        [SerializeField] private bool buyBackItems = true;
        
        [Tooltip("Tỷ lệ mua lại / Buy back rate")]
        [Range(0f, 1f)]
        [SerializeField] private float buyBackRate = 0.5f;
        
        [Header("Shop Hours")]
        [Tooltip("Mở cả ngày / Always open")]
        [SerializeField] private bool alwaysOpen = true;
        
        [Tooltip("Giờ mở cửa / Opening hour")]
        [SerializeField] private int openingHour = 6;
        
        [Tooltip("Giờ đóng cửa / Closing hour")]
        [SerializeField] private int closingHour = 22;
        
        private List<ShopItem> soldItems = new List<ShopItem>();
        
        protected override void InitializeNPC()
        {
            base.InitializeNPC();
            
            // Initialize shop items
            InitializeShop();
            
            Debug.Log($"[ShopNPC] Shop initialized: {npcName} ({shopType})");
        }
        
        /// <summary>
        /// Khởi tạo shop / Initialize shop
        /// </summary>
        private void InitializeShop()
        {
            // Validate shop items
            if (shopItems.Count == 0)
            {
                Debug.LogWarning($"[ShopNPC] {npcName} has no items to sell!");
            }
            
            Debug.Log($"[ShopNPC] Loaded {shopItems.Count} items for {npcName}");
        }
        
        protected override void OpenNPCUI(GameObject player)
        {
            // Check if shop is open
            if (!IsShopOpen())
            {
                ShowDialog($"Xin lỗi, shop đang đóng cửa. Hãy quay lại từ {openingHour}:00 đến {closingHour}:00!");
                return;
            }
            
            Debug.Log($"[ShopNPC] Opening shop UI for {shopType} shop");
            // TODO: Open shop UI with items
            DisplayShopUI(player);
        }
        
        /// <summary>
        /// Hiển thị UI shop / Display shop UI
        /// </summary>
        private void DisplayShopUI(GameObject player)
        {
            // TODO: Create and show shop UI
            Debug.Log($"[ShopNPC] Displaying {shopItems.Count} items");
        }
        
        /// <summary>
        /// Kiểm tra shop có mở không / Check if shop is open
        /// </summary>
        private bool IsShopOpen()
        {
            if (alwaysOpen) return true;
            
            // TODO: Get current game time
            int currentHour = 12; // Placeholder
            
            return currentHour >= openingHour && currentHour < closingHour;
        }
        
        /// <summary>
        /// Mua item / Buy item
        /// </summary>
        public bool BuyItem(GameObject player, ShopItem item)
        {
            // TODO: Check if player has enough Zen
            // TODO: Check if player has inventory space
            // TODO: Add item to player inventory
            // TODO: Deduct Zen from player
            
            int finalPrice = CalculateFinalPrice(item.price);
            Debug.Log($"[ShopNPC] Player bought {item.itemName} for {finalPrice} Zen");
            
            // Add to sold items for buy-back
            if (buyBackItems)
            {
                soldItems.Add(item);
            }
            
            return true;
        }
        
        /// <summary>
        /// Bán item cho NPC / Sell item to NPC
        /// </summary>
        public bool SellItem(GameObject player, string itemName, int basePrice)
        {
            if (!buyBackItems)
            {
                ShowDialog("Xin lỗi, tôi không mua items từ players!");
                return false;
            }
            
            // Calculate buy-back price
            int sellPrice = Mathf.RoundToInt(basePrice * buyBackRate);
            
            // TODO: Remove item from player inventory
            // TODO: Add Zen to player
            
            Debug.Log($"[ShopNPC] Bought {itemName} from player for {sellPrice} Zen");
            return true;
        }
        
        /// <summary>
        /// Tính giá cuối cùng / Calculate final price
        /// </summary>
        private int CalculateFinalPrice(int basePrice)
        {
            float discount = 1f - discountPercentage;
            return Mathf.RoundToInt(basePrice * discount);
        }
        
        /// <summary>
        /// Repair item / Sửa item
        /// </summary>
        public bool RepairItem(GameObject player, string itemName, int repairCost)
        {
            // TODO: Check if player has enough Zen
            // TODO: Repair item durability
            // TODO: Deduct Zen
            
            Debug.Log($"[ShopNPC] Repaired {itemName} for {repairCost} Zen");
            return true;
        }
        
        /// <summary>
        /// Lấy danh sách items / Get shop items
        /// </summary>
        public List<ShopItem> GetShopItems()
        {
            return new List<ShopItem>(shopItems);
        }
        
        /// <summary>
        /// Thêm item vào shop / Add item to shop
        /// </summary>
        public void AddShopItem(ShopItem item)
        {
            shopItems.Add(item);
            Debug.Log($"[ShopNPC] Added {item.itemName} to shop");
        }
        
        /// <summary>
        /// Xóa item khỏi shop / Remove item from shop
        /// </summary>
        public void RemoveShopItem(ShopItem item)
        {
            shopItems.Remove(item);
            Debug.Log($"[ShopNPC] Removed {item.itemName} from shop");
        }
    }
    
    /// <summary>
    /// Loại shop / Shop types
    /// </summary>
    public enum ShopType
    {
        General,        // Tổng hợp
        Weapon,         // Vũ khí
        Armor,          // Giáp
        Accessory,      // Phụ kiện
        Potion,         // Thuốc
        Scroll,         // Cuộn
        Material,       // Nguyên liệu
        Special         // Đặc biệt
    }
    
    /// <summary>
    /// Item trong shop / Shop item
    /// </summary>
    [System.Serializable]
    public class ShopItem
    {
        [Tooltip("Tên item / Item name")]
        public string itemName;
        
        [Tooltip("Mô tả / Description")]
        public string description;
        
        [Tooltip("Giá / Price in Zen")]
        public int price;
        
        [Tooltip("Level yêu cầu / Required level")]
        public int requiredLevel = 1;
        
        [Tooltip("Số lượng tồn kho / Stock quantity (-1 = unlimited)")]
        public int stockQuantity = -1;
        
        [Tooltip("Prefab item / Item prefab")]
        public GameObject itemPrefab;
        
        [Tooltip("Icon / Item icon")]
        public Sprite icon;
    }
}
