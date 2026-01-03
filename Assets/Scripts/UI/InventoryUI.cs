using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.UI
{
    /// <summary>
    /// Inventory UI panel
    /// Giao diện panel inventory
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject inventoryPanel;
        public Transform itemSlotContainer;
        public GameObject itemSlotPrefab;
        
        [Header("Item Info")]
        public TextMeshProUGUI itemNameText;
        public TextMeshProUGUI itemDescriptionText;
        public Image itemIconImage;
        
        [Header("Gold Display")]
        public TextMeshProUGUI goldText;
        
        [Header("References")]
        public Inventory.InventorySystem inventorySystem;
        
        private Inventory.Item selectedItem;
        private int selectedSlotIndex = -1;
        
        private void Start()
        {
            // Find player if not assigned
            if (inventorySystem == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
                if (player != null)
                {
                    inventorySystem = player.GetComponent<Inventory.InventorySystem>();
                }
            }
            
            // Subscribe to events
            if (inventorySystem != null)
            {
                inventorySystem.OnInventoryChanged += RefreshInventory;
                inventorySystem.OnGoldChanged += UpdateGold;
            }
            
            // Start with inventory closed
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }
            
            RefreshInventory();
        }
        
        private void Update()
        {
            // Toggle inventory with Tab key
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleInventory();
            }
        }
        
        /// <summary>
        /// Toggle inventory panel
        /// Bật/tắt panel inventory
        /// </summary>
        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                bool isActive = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(isActive);
                
                if (isActive)
                {
                    RefreshInventory();
                }
            }
        }
        
        /// <summary>
        /// Refresh inventory display
        /// Làm mới hiển thị inventory
        /// </summary>
        private void RefreshInventory()
        {
            if (inventorySystem == null || itemSlotContainer == null) return;
            
            // Clear existing slots
            foreach (Transform child in itemSlotContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create slots for each item
            for (int i = 0; i < inventorySystem.inventorySize; i++)
            {
                GameObject slotObj = Instantiate(itemSlotPrefab, itemSlotContainer);
                ItemSlotUI slotUI = slotObj.GetComponent<ItemSlotUI>();
                
                if (slotUI != null)
                {
                    Inventory.Item item = i < inventorySystem.items.Count ? inventorySystem.items[i] : null;
                    slotUI.SetItem(item, i);
                    
                    int slotIndex = i; // Capture for lambda
                    slotUI.OnSlotClick += () => OnItemSlotClick(slotIndex);
                }
            }
            
            UpdateGold(inventorySystem.gold);
        }
        
        /// <summary>
        /// Handle item slot click
        /// Xử lý click ô vật phẩm
        /// </summary>
        private void OnItemSlotClick(int slotIndex)
        {
            if (inventorySystem == null) return;
            
            if (slotIndex < inventorySystem.items.Count)
            {
                selectedItem = inventorySystem.items[slotIndex];
                selectedSlotIndex = slotIndex;
                DisplayItemInfo(selectedItem);
            }
        }
        
        /// <summary>
        /// Display item information
        /// Hiển thị thông tin vật phẩm
        /// </summary>
        private void DisplayItemInfo(Inventory.Item item)
        {
            if (item == null)
            {
                if (itemNameText != null) itemNameText.text = "";
                if (itemDescriptionText != null) itemDescriptionText.text = "";
                if (itemIconImage != null) itemIconImage.enabled = false;
                return;
            }
            
            if (itemNameText != null)
            {
                itemNameText.text = item.itemName;
                itemNameText.color = item.GetRarityColor();
            }
            
            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = item.description;
            }
            
            if (itemIconImage != null && item.icon != null)
            {
                itemIconImage.sprite = item.icon;
                itemIconImage.enabled = true;
            }
        }
        
        /// <summary>
        /// Use selected item
        /// Sử dụng vật phẩm đã chọn
        /// </summary>
        public void UseSelectedItem()
        {
            if (selectedSlotIndex >= 0 && inventorySystem != null)
            {
                inventorySystem.UseItem(selectedSlotIndex);
                selectedItem = null;
                selectedSlotIndex = -1;
            }
        }
        
        /// <summary>
        /// Drop selected item
        /// Vứt vật phẩm đã chọn
        /// </summary>
        public void DropSelectedItem()
        {
            if (selectedSlotIndex >= 0 && inventorySystem != null)
            {
                inventorySystem.RemoveItemFromSlot(selectedSlotIndex);
                selectedItem = null;
                selectedSlotIndex = -1;
            }
        }
        
        /// <summary>
        /// Update gold display
        /// Cập nhật hiển thị vàng
        /// </summary>
        private void UpdateGold(int amount)
        {
            if (goldText != null)
            {
                goldText.text = $"Gold: {amount:N0}";
            }
        }
        
        /// <summary>
        /// Sort inventory
        /// Sắp xếp inventory
        /// </summary>
        public void SortInventory()
        {
            if (inventorySystem != null)
            {
                inventorySystem.SortInventory();
            }
        }
        
        private void OnDestroy()
        {
            if (inventorySystem != null)
            {
                inventorySystem.OnInventoryChanged -= RefreshInventory;
                inventorySystem.OnGoldChanged -= UpdateGold;
            }
        }
    }
    
    /// <summary>
    /// Individual item slot UI
    /// UI ô vật phẩm riêng lẻ
    /// </summary>
    public class ItemSlotUI : MonoBehaviour
    {
        public Image itemIcon;
        public TextMeshProUGUI stackText;
        public Button button;
        
        private Inventory.Item item;
        private int slotIndex;
        
        public System.Action OnSlotClick;
        
        private void Start()
        {
            if (button != null)
            {
                button.onClick.AddListener(OnClick);
            }
        }
        
        /// <summary>
        /// Set item for this slot
        /// Đặt vật phẩm cho ô này
        /// </summary>
        public void SetItem(Inventory.Item newItem, int index)
        {
            item = newItem;
            slotIndex = index;
            
            if (item != null)
            {
                if (itemIcon != null)
                {
                    itemIcon.sprite = item.icon;
                    itemIcon.enabled = true;
                }
                
                if (stackText != null)
                {
                    if (item.isStackable && item.currentStack > 1)
                    {
                        stackText.text = item.currentStack.ToString();
                        stackText.enabled = true;
                    }
                    else
                    {
                        stackText.enabled = false;
                    }
                }
            }
            else
            {
                if (itemIcon != null) itemIcon.enabled = false;
                if (stackText != null) stackText.enabled = false;
            }
        }
        
        private void OnClick()
        {
            OnSlotClick?.Invoke();
        }
    }
}
