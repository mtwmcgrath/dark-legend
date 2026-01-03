using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Inventory
{
    /// <summary>
    /// Equipment slot management
    /// Quản lý ô trang bị
    /// </summary>
    [System.Serializable]
    public class EquipmentSlot
    {
        public EquipmentSlotType slotType;
        public Equipment equippedItem;
        
        public bool IsEmpty => equippedItem == null;
        
        /// <summary>
        /// Check if item can be equipped in this slot
        /// Kiểm tra vật phẩm có thể được trang bị vào ô này không
        /// </summary>
        public bool CanEquip(Equipment equipment)
        {
            if (equipment == null) return false;
            return equipment.slotType == slotType;
        }
        
        /// <summary>
        /// Equip item to this slot
        /// Trang bị vật phẩm vào ô này
        /// </summary>
        public Equipment Equip(Equipment equipment, Character.CharacterStats stats)
        {
            if (!CanEquip(equipment)) return null;
            
            Equipment previousItem = equippedItem;
            
            // Unequip previous item
            if (previousItem != null)
            {
                previousItem.Unequip(stats);
            }
            
            // Equip new item
            equippedItem = equipment;
            equipment.Equip(stats);
            
            return previousItem;
        }
        
        /// <summary>
        /// Unequip item from this slot
        /// Gỡ trang bị khỏi ô này
        /// </summary>
        public Equipment Unequip(Character.CharacterStats stats)
        {
            if (equippedItem == null) return null;
            
            Equipment item = equippedItem;
            item.Unequip(stats);
            equippedItem = null;
            
            return item;
        }
    }
    
    /// <summary>
    /// Equipment slot manager
    /// Quản lý các ô trang bị
    /// </summary>
    public class EquipmentSlotManager : MonoBehaviour
    {
        [Header("Equipment Slots")]
        public List<EquipmentSlot> equipmentSlots = new List<EquipmentSlot>();
        
        [Header("References")]
        public Character.CharacterStats characterStats;
        
        // Events
        public System.Action<EquipmentSlotType, Equipment> OnEquipmentChanged;
        
        private void Start()
        {
            if (characterStats == null)
            {
                characterStats = GetComponent<Character.CharacterStats>();
            }
            
            InitializeSlots();
        }
        
        /// <summary>
        /// Initialize equipment slots
        /// Khởi tạo các ô trang bị
        /// </summary>
        private void InitializeSlots()
        {
            if (equipmentSlots.Count == 0)
            {
                // Create default slots
                foreach (EquipmentSlotType slotType in System.Enum.GetValues(typeof(EquipmentSlotType)))
                {
                    equipmentSlots.Add(new EquipmentSlot { slotType = slotType });
                }
            }
        }
        
        /// <summary>
        /// Equip an item
        /// Trang bị một vật phẩm
        /// </summary>
        public Equipment EquipItem(Equipment equipment)
        {
            if (equipment == null) return null;
            
            EquipmentSlot slot = GetSlot(equipment.slotType);
            if (slot == null) return null;
            
            Equipment previousItem = slot.Equip(equipment, characterStats);
            OnEquipmentChanged?.Invoke(equipment.slotType, equipment);
            
            return previousItem;
        }
        
        /// <summary>
        /// Unequip an item from slot
        /// Gỡ trang bị khỏi ô
        /// </summary>
        public Equipment UnequipItem(EquipmentSlotType slotType)
        {
            EquipmentSlot slot = GetSlot(slotType);
            if (slot == null) return null;
            
            Equipment item = slot.Unequip(characterStats);
            OnEquipmentChanged?.Invoke(slotType, null);
            
            return item;
        }
        
        /// <summary>
        /// Get equipment slot by type
        /// Lấy ô trang bị theo loại
        /// </summary>
        public EquipmentSlot GetSlot(EquipmentSlotType slotType)
        {
            foreach (var slot in equipmentSlots)
            {
                if (slot.slotType == slotType)
                {
                    return slot;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Get equipped item from slot
        /// Lấy vật phẩm đã trang bị từ ô
        /// </summary>
        public Equipment GetEquippedItem(EquipmentSlotType slotType)
        {
            EquipmentSlot slot = GetSlot(slotType);
            return slot?.equippedItem;
        }
        
        /// <summary>
        /// Check if slot is empty
        /// Kiểm tra ô có trống không
        /// </summary>
        public bool IsSlotEmpty(EquipmentSlotType slotType)
        {
            EquipmentSlot slot = GetSlot(slotType);
            return slot?.IsEmpty ?? true;
        }
        
        /// <summary>
        /// Unequip all items
        /// Gỡ tất cả trang bị
        /// </summary>
        public List<Equipment> UnequipAll()
        {
            List<Equipment> items = new List<Equipment>();
            
            foreach (var slot in equipmentSlots)
            {
                if (!slot.IsEmpty)
                {
                    Equipment item = slot.Unequip(characterStats);
                    if (item != null)
                    {
                        items.Add(item);
                    }
                }
            }
            
            return items;
        }
    }
}
