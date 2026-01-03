using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.NPCs
{
    /// <summary>
    /// NPC kho đồ / Storage/Warehouse NPC
    /// </summary>
    public class StorageNPC : NPCBase
    {
        [Header("Storage Configuration")]
        [Tooltip("Số slot kho / Storage slot count")]
        [SerializeField] private int storageSlots = 120;
        
        [Tooltip("Chi phí mở rộng / Expansion cost")]
        [SerializeField] private int expansionCost = 10000;
        
        [Tooltip("Số slot mở rộng mỗi lần / Slots per expansion")]
        [SerializeField] private int slotsPerExpansion = 20;
        
        [Tooltip("Số lần mở rộng tối đa / Maximum expansions")]
        [SerializeField] private int maxExpansions = 5;
        
        [Header("Features")]
        [Tooltip("Cho phép sort / Allow sorting")]
        [SerializeField] private bool allowSorting = true;
        
        [Tooltip("Shared storage (guild) / Shared storage")]
        [SerializeField] private bool isSharedStorage = false;
        
        private Dictionary<int, PlayerStorage> playerStorages = new Dictionary<int, PlayerStorage>();
        
        protected override void InitializeNPC()
        {
            base.InitializeNPC();
            
            Debug.Log($"[StorageNPC] Storage NPC initialized: {npcName}");
            Debug.Log($"[StorageNPC] Base slots: {storageSlots}");
        }
        
        protected override void OpenNPCUI(GameObject player)
        {
            Debug.Log($"[StorageNPC] Opening storage UI");
            
            // Get or create player storage
            PlayerStorage storage = GetPlayerStorage(player);
            
            // Open storage UI
            OpenStorageUI(player, storage);
        }
        
        /// <summary>
        /// Mở UI kho / Open storage UI
        /// </summary>
        private void OpenStorageUI(GameObject player, PlayerStorage storage)
        {
            Debug.Log($"[StorageNPC] Displaying storage with {storage.maxSlots} slots");
            // TODO: Show storage UI
        }
        
        /// <summary>
        /// Lấy kho của player / Get player storage
        /// </summary>
        private PlayerStorage GetPlayerStorage(GameObject player)
        {
            int playerId = player.GetInstanceID();
            
            if (!playerStorages.ContainsKey(playerId))
            {
                // Create new storage
                playerStorages[playerId] = new PlayerStorage
                {
                    maxSlots = storageSlots,
                    currentExpansions = 0,
                    items = new List<StorageItem>()
                };
            }
            
            return playerStorages[playerId];
        }
        
        /// <summary>
        /// Lưu item vào kho / Store item
        /// </summary>
        public bool StoreItem(GameObject player, string itemName, int quantity = 1)
        {
            PlayerStorage storage = GetPlayerStorage(player);
            
            // Check if storage has space
            if (storage.items.Count >= storage.maxSlots)
            {
                ShowDialog("Kho đã đầy! Hãy mở rộng hoặc lấy đồ ra.");
                return false;
            }
            
            // Add item to storage
            StorageItem item = new StorageItem
            {
                itemName = itemName,
                quantity = quantity,
                storedTime = System.DateTime.Now
            };
            
            storage.items.Add(item);
            
            Debug.Log($"[StorageNPC] Stored {quantity}x {itemName}");
            ShowDialog($"Đã lưu {quantity}x {itemName} vào kho.");
            
            return true;
        }
        
        /// <summary>
        /// Lấy item từ kho / Retrieve item from storage
        /// </summary>
        public bool RetrieveItem(GameObject player, string itemName, int quantity = 1)
        {
            PlayerStorage storage = GetPlayerStorage(player);
            
            // Find item in storage
            StorageItem item = storage.items.Find(i => i.itemName == itemName);
            
            if (item == null)
            {
                ShowDialog($"{itemName} không có trong kho!");
                return false;
            }
            
            // TODO: Check if player inventory has space
            
            // Remove item from storage
            if (item.quantity <= quantity)
            {
                storage.items.Remove(item);
            }
            else
            {
                item.quantity -= quantity;
            }
            
            Debug.Log($"[StorageNPC] Retrieved {quantity}x {itemName}");
            ShowDialog($"Đã lấy {quantity}x {itemName} từ kho.");
            
            return true;
        }
        
        /// <summary>
        /// Mở rộng kho / Expand storage
        /// </summary>
        public bool ExpandStorage(GameObject player)
        {
            PlayerStorage storage = GetPlayerStorage(player);
            
            // Check if can expand
            if (storage.currentExpansions >= maxExpansions)
            {
                ShowDialog("Kho đã đạt kích thước tối đa!");
                return false;
            }
            
            // TODO: Check if player has enough Zen
            
            // Expand storage
            storage.maxSlots += slotsPerExpansion;
            storage.currentExpansions++;
            
            Debug.Log($"[StorageNPC] Expanded storage to {storage.maxSlots} slots");
            ShowDialog($"Đã mở rộng kho! Slots: {storage.maxSlots}");
            
            return true;
        }
        
        /// <summary>
        /// Sort kho / Sort storage
        /// </summary>
        public void SortStorage(GameObject player)
        {
            if (!allowSorting)
            {
                ShowDialog("Chức năng sort không khả dụng!");
                return;
            }
            
            PlayerStorage storage = GetPlayerStorage(player);
            
            // Sort items by name
            storage.items.Sort((a, b) => a.itemName.CompareTo(b.itemName));
            
            Debug.Log($"[StorageNPC] Storage sorted");
            ShowDialog("Đã sắp xếp kho!");
        }
        
        /// <summary>
        /// Lấy danh sách items trong kho / Get storage items
        /// </summary>
        public List<StorageItem> GetStorageItems(GameObject player)
        {
            PlayerStorage storage = GetPlayerStorage(player);
            return new List<StorageItem>(storage.items);
        }
        
        /// <summary>
        /// Kiểm tra kho có đầy không / Check if storage is full
        /// </summary>
        public bool IsStorageFull(GameObject player)
        {
            PlayerStorage storage = GetPlayerStorage(player);
            return storage.items.Count >= storage.maxSlots;
        }
    }
    
    /// <summary>
    /// Kho của player / Player storage
    /// </summary>
    public class PlayerStorage
    {
        public int maxSlots;
        public int currentExpansions;
        public List<StorageItem> items;
    }
    
    /// <summary>
    /// Item trong kho / Storage item
    /// </summary>
    [System.Serializable]
    public class StorageItem
    {
        public string itemName;
        public int quantity;
        public System.DateTime storedTime;
        public string itemData; // Serialized item data
    }
}
