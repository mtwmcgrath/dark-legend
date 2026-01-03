using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Mobile inventory UI
    /// UI inventory cho mobile
    /// </summary>
    public class MobileInventoryUI : MonoBehaviour
    {
        [Header("Inventory Settings")]
        public int inventorySize = 20;
        public int columns = 4;
        public int rows = 5;

        [Header("UI Components")]
        public GameObject inventoryPanel;
        public Transform itemContainer;
        public GameObject itemSlotPrefab;

        [Header("Item Info")]
        public Text itemNameText;
        public Text itemDescriptionText;
        public Image itemIconImage;
        public Button useButton;
        public Button dropButton;

        private GameObject[] itemSlots;
        private int selectedSlotIndex = -1;
        private bool isOpen = false;

        private void Start()
        {
            InitializeInventory();
            Close();
        }

        /// <summary>
        /// Initialize inventory
        /// Khởi tạo inventory
        /// </summary>
        private void InitializeInventory()
        {
            if (itemSlotPrefab == null || itemContainer == null)
                return;

            itemSlots = new GameObject[inventorySize];

            // Create item slots
            for (int i = 0; i < inventorySize; i++)
            {
                GameObject slot = Instantiate(itemSlotPrefab, itemContainer);
                slot.name = $"ItemSlot_{i}";
                itemSlots[i] = slot;

                // Add click listener
                int index = i;
                Button button = slot.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => OnItemSlotClicked(index));
                }
            }

            // Setup grid layout
            GridLayoutGroup gridLayout = itemContainer.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = columns;
            }

            // Setup action buttons
            if (useButton != null)
            {
                useButton.onClick.AddListener(OnUseItem);
            }

            if (dropButton != null)
            {
                dropButton.onClick.AddListener(OnDropItem);
            }

            Debug.Log($"[MobileInventoryUI] Inventory initialized with {inventorySize} slots");
        }

        /// <summary>
        /// Toggle inventory
        /// Bật/tắt inventory
        /// </summary>
        public void Toggle()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        /// <summary>
        /// Open inventory
        /// Mở inventory
        /// </summary>
        public void Open()
        {
            isOpen = true;
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }
            Debug.Log("[MobileInventoryUI] Inventory opened");
        }

        /// <summary>
        /// Close inventory
        /// Đóng inventory
        /// </summary>
        public void Close()
        {
            isOpen = false;
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }
            selectedSlotIndex = -1;
            ClearItemInfo();
            Debug.Log("[MobileInventoryUI] Inventory closed");
        }

        /// <summary>
        /// Item slot clicked
        /// Slot item được click
        /// </summary>
        private void OnItemSlotClicked(int slotIndex)
        {
            selectedSlotIndex = slotIndex;
            Debug.Log($"[MobileInventoryUI] Item slot {slotIndex} selected");

            // TODO: Get item data and display info
            // ItemData item = InventorySystem.GetItem(slotIndex);
            // if (item != null)
            // {
            //     DisplayItemInfo(item);
            // }
        }

        /// <summary>
        /// Display item info
        /// Hiển thị thông tin item
        /// </summary>
        private void DisplayItemInfo(string name, string description, Sprite icon)
        {
            if (itemNameText != null)
            {
                itemNameText.text = name;
            }

            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = description;
            }

            if (itemIconImage != null)
            {
                itemIconImage.sprite = icon;
                itemIconImage.enabled = true;
            }
        }

        /// <summary>
        /// Clear item info
        /// Xóa thông tin item
        /// </summary>
        private void ClearItemInfo()
        {
            if (itemNameText != null)
            {
                itemNameText.text = "";
            }

            if (itemDescriptionText != null)
            {
                itemDescriptionText.text = "";
            }

            if (itemIconImage != null)
            {
                itemIconImage.enabled = false;
            }
        }

        /// <summary>
        /// Use item button clicked
        /// Nút dùng item được click
        /// </summary>
        private void OnUseItem()
        {
            if (selectedSlotIndex < 0)
                return;

            Debug.Log($"[MobileInventoryUI] Using item from slot {selectedSlotIndex}");
            // TODO: Call inventory system to use item
            // InventorySystem.UseItem(selectedSlotIndex);
        }

        /// <summary>
        /// Drop item button clicked
        /// Nút vứt item được click
        /// </summary>
        private void OnDropItem()
        {
            if (selectedSlotIndex < 0)
                return;

            Debug.Log($"[MobileInventoryUI] Dropping item from slot {selectedSlotIndex}");
            // TODO: Call inventory system to drop item
            // InventorySystem.DropItem(selectedSlotIndex);
        }

        /// <summary>
        /// Add item to inventory
        /// Thêm item vào inventory
        /// </summary>
        public void AddItem(int slotIndex, Sprite icon)
        {
            if (slotIndex < 0 || slotIndex >= itemSlots.Length)
                return;

            Image slotImage = itemSlots[slotIndex].GetComponentInChildren<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = icon;
                slotImage.enabled = true;
            }
        }

        /// <summary>
        /// Remove item from inventory
        /// Xóa item khỏi inventory
        /// </summary>
        public void RemoveItem(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= itemSlots.Length)
                return;

            Image slotImage = itemSlots[slotIndex].GetComponentInChildren<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = null;
                slotImage.enabled = false;
            }
        }
    }
}
