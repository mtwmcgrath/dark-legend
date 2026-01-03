using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.Inventory
{
    /// <summary>
    /// Inventory system management
    /// Quản lý hệ thống inventory
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        [Header("Inventory Settings")]
        public int inventorySize = 64; // 8x8 grid
        public List<Item> items = new List<Item>();
        
        [Header("Currency")]
        public int gold = 0;
        
        [Header("References")]
        public Character.CharacterStats characterStats;
        public EquipmentSlotManager equipmentManager;
        
        // Events
        public System.Action OnInventoryChanged;
        public System.Action<Item> OnItemAdded;
        public System.Action<Item> OnItemRemoved;
        public System.Action<int> OnGoldChanged;
        
        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<Character.CharacterStats>();
            }
            
            if (equipmentManager == null)
            {
                equipmentManager = GetComponent<EquipmentSlotManager>();
            }
            
            // Initialize inventory with empty slots
            while (items.Count < inventorySize)
            {
                items.Add(null);
            }
        }
        
        /// <summary>
        /// Add item to inventory
        /// Thêm vật phẩm vào inventory
        /// </summary>
        public bool AddItem(Item item)
        {
            if (item == null) return false;
            
            // Try to stack with existing items
            if (item.isStackable)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null && items[i].CanStackWith(item))
                    {
                        int spaceLeft = items[i].stackSize - items[i].currentStack;
                        int amountToAdd = Mathf.Min(spaceLeft, item.currentStack);
                        
                        items[i].currentStack += amountToAdd;
                        item.currentStack -= amountToAdd;
                        
                        if (item.currentStack <= 0)
                        {
                            OnItemAdded?.Invoke(item);
                            OnInventoryChanged?.Invoke();
                            return true;
                        }
                    }
                }
            }
            
            // Find empty slot
            int emptySlot = FindEmptySlot();
            if (emptySlot >= 0)
            {
                items[emptySlot] = item;
                OnItemAdded?.Invoke(item);
                OnInventoryChanged?.Invoke();
                return true;
            }
            
            Debug.Log("Inventory is full!");
            return false;
        }
        
        /// <summary>
        /// Remove item from inventory
        /// Xóa vật phẩm khỏi inventory
        /// </summary>
        public bool RemoveItem(Item item, int amount = 1)
        {
            if (item == null) return false;
            
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null && items[i].itemID == item.itemID)
                {
                    if (items[i].isStackable)
                    {
                        items[i].currentStack -= amount;
                        
                        if (items[i].currentStack <= 0)
                        {
                            items[i] = null;
                        }
                    }
                    else
                    {
                        items[i] = null;
                    }
                    
                    OnItemRemoved?.Invoke(item);
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Remove item from specific slot
        /// Xóa vật phẩm từ ô cụ thể
        /// </summary>
        public Item RemoveItemFromSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Count)
                return null;
            
            Item item = items[slotIndex];
            if (item != null)
            {
                items[slotIndex] = null;
                OnItemRemoved?.Invoke(item);
                OnInventoryChanged?.Invoke();
            }
            
            return item;
        }
        
        /// <summary>
        /// Use item from inventory
        /// Sử dụng vật phẩm từ inventory
        /// </summary>
        public void UseItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= items.Count)
                return;
            
            Item item = items[slotIndex];
            if (item == null) return;
            
            // Check if it's equipment
            if (item is Equipment equipment)
            {
                EquipItem(equipment, slotIndex);
            }
            else
            {
                // Use consumable
                item.Use(gameObject);
                
                // Remove consumed item
                if (item.itemType == ItemType.Consumable)
                {
                    RemoveItem(item, 1);
                }
            }
        }
        
        /// <summary>
        /// Equip item from inventory
        /// Trang bị vật phẩm từ inventory
        /// </summary>
        private void EquipItem(Equipment equipment, int slotIndex)
        {
            if (equipmentManager == null) return;
            
            // Check level requirement
            if (characterStats != null && equipment.requiredLevel > equipmentManager.GetComponent<Character.LevelSystem>()?.currentLevel)
            {
                Debug.Log($"Level {equipment.requiredLevel} required to equip {equipment.itemName}");
                return;
            }
            
            // Equip item (returns previously equipped item if any)
            Equipment previousItem = equipmentManager.EquipItem(equipment);
            
            // Remove from inventory
            items[slotIndex] = null;
            
            // Add previous item back to inventory
            if (previousItem != null)
            {
                AddItem(previousItem);
            }
            
            OnInventoryChanged?.Invoke();
        }
        
        /// <summary>
        /// Unequip item to inventory
        /// Gỡ trang bị vào inventory
        /// </summary>
        public bool UnequipItem(EquipmentSlotType slotType)
        {
            if (equipmentManager == null) return false;
            
            Equipment item = equipmentManager.UnequipItem(slotType);
            if (item != null)
            {
                return AddItem(item);
            }
            
            return false;
        }
        
        /// <summary>
        /// Find empty slot index
        /// Tìm index ô trống
        /// </summary>
        private int FindEmptySlot()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Check if inventory has item
        /// Kiểm tra inventory có vật phẩm không
        /// </summary>
        public bool HasItem(string itemID, int amount = 1)
        {
            int count = 0;
            foreach (var item in items)
            {
                if (item != null && item.itemID == itemID)
                {
                    count += item.currentStack;
                    if (count >= amount)
                        return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Get item count in inventory
        /// Lấy số lượng vật phẩm trong inventory
        /// </summary>
        public int GetItemCount(string itemID)
        {
            int count = 0;
            foreach (var item in items)
            {
                if (item != null && item.itemID == itemID)
                {
                    count += item.currentStack;
                }
            }
            return count;
        }
        
        /// <summary>
        /// Sort inventory
        /// Sắp xếp inventory
        /// </summary>
        public void SortInventory()
        {
            // Remove nulls and sort by type, then rarity
            var sortedItems = items.Where(item => item != null)
                                  .OrderBy(item => item.itemType)
                                  .ThenBy(item => item.rarity)
                                  .ToList();
            
            items.Clear();
            items.AddRange(sortedItems);
            
            // Fill remaining slots with null
            while (items.Count < inventorySize)
            {
                items.Add(null);
            }
            
            OnInventoryChanged?.Invoke();
        }
        
        /// <summary>
        /// Add gold
        /// Thêm vàng
        /// </summary>
        public void AddGold(int amount)
        {
            gold += amount;
            OnGoldChanged?.Invoke(gold);
        }
        
        /// <summary>
        /// Remove gold
        /// Trừ vàng
        /// </summary>
        public bool RemoveGold(int amount)
        {
            if (gold < amount) return false;
            
            gold -= amount;
            OnGoldChanged?.Invoke(gold);
            return true;
        }
        
        /// <summary>
        /// Clear inventory
        /// Xóa toàn bộ inventory
        /// </summary>
        public void ClearInventory()
        {
            items.Clear();
            while (items.Count < inventorySize)
            {
                items.Add(null);
            }
            OnInventoryChanged?.Invoke();
        }
    }
}
