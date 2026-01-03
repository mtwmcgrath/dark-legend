using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Quick slot UI for potions/items
    /// UI slot nhanh cho potion/items
    /// </summary>
    public class QuickSlotUI : MonoBehaviour
    {
        [Header("Quick Slots")]
        public QuickSlot[] quickSlots;
        public int maxSlots = 4;

        [System.Serializable]
        public class QuickSlot
        {
            public Button button;
            public Image iconImage;
            public Text countText;
            public KeyCode hotkey = KeyCode.None;
            
            [HideInInspector] public int itemId = -1;
            [HideInInspector] public int count = 0;
        }

        private void Start()
        {
            InitializeQuickSlots();
        }

        private void Update()
        {
            CheckHotkeys();
        }

        /// <summary>
        /// Initialize quick slots
        /// Khởi tạo quick slot
        /// </summary>
        private void InitializeQuickSlots()
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                int index = i;
                QuickSlot slot = quickSlots[i];

                if (slot.button != null)
                {
                    slot.button.onClick.AddListener(() => OnQuickSlotClicked(index));
                }

                UpdateSlotVisual(i);
            }

            Debug.Log($"[QuickSlotUI] Initialized {quickSlots.Length} quick slots");
        }

        /// <summary>
        /// Check hotkeys
        /// Kiểm tra phím tắt
        /// </summary>
        private void CheckHotkeys()
        {
            for (int i = 0; i < quickSlots.Length; i++)
            {
                QuickSlot slot = quickSlots[i];
                
                if (slot.hotkey != KeyCode.None && Input.GetKeyDown(slot.hotkey))
                {
                    OnQuickSlotClicked(i);
                }
            }
        }

        /// <summary>
        /// Quick slot clicked
        /// Quick slot được click
        /// </summary>
        private void OnQuickSlotClicked(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= quickSlots.Length)
                return;

            QuickSlot slot = quickSlots[slotIndex];

            if (slot.itemId < 0 || slot.count <= 0)
                return;

            UseQuickSlot(slotIndex);
        }

        /// <summary>
        /// Use quick slot item
        /// Dùng item trong quick slot
        /// </summary>
        private void UseQuickSlot(int slotIndex)
        {
            QuickSlot slot = quickSlots[slotIndex];

            Debug.Log($"[QuickSlotUI] Using item from quick slot {slotIndex} (ID: {slot.itemId})");

            // Decrease count
            slot.count--;

            if (slot.count <= 0)
            {
                // Clear slot
                slot.itemId = -1;
                slot.count = 0;
            }

            UpdateSlotVisual(slotIndex);

            // TODO: Use item from inventory system
            // InventorySystem.UseItem(slot.itemId);
        }

        /// <summary>
        /// Set quick slot item
        /// Đặt item vào quick slot
        /// </summary>
        public void SetQuickSlot(int slotIndex, int itemId, Sprite icon, int count)
        {
            if (slotIndex < 0 || slotIndex >= quickSlots.Length)
                return;

            QuickSlot slot = quickSlots[slotIndex];
            slot.itemId = itemId;
            slot.count = count;

            if (slot.iconImage != null)
            {
                slot.iconImage.sprite = icon;
                slot.iconImage.enabled = icon != null;
            }

            UpdateSlotVisual(slotIndex);
        }

        /// <summary>
        /// Clear quick slot
        /// Xóa quick slot
        /// </summary>
        public void ClearQuickSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= quickSlots.Length)
                return;

            QuickSlot slot = quickSlots[slotIndex];
            slot.itemId = -1;
            slot.count = 0;

            UpdateSlotVisual(slotIndex);
        }

        /// <summary>
        /// Update slot visual
        /// Cập nhật hiển thị slot
        /// </summary>
        private void UpdateSlotVisual(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= quickSlots.Length)
                return;

            QuickSlot slot = quickSlots[slotIndex];

            // Update count text
            if (slot.countText != null)
            {
                if (slot.count > 0)
                {
                    slot.countText.text = slot.count.ToString();
                    slot.countText.enabled = true;
                }
                else
                {
                    slot.countText.enabled = false;
                }
            }

            // Update icon
            if (slot.iconImage != null && slot.itemId < 0)
            {
                slot.iconImage.enabled = false;
            }
        }

        /// <summary>
        /// Update quick slot count
        /// Cập nhật số lượng trong quick slot
        /// </summary>
        public void UpdateQuickSlotCount(int slotIndex, int count)
        {
            if (slotIndex < 0 || slotIndex >= quickSlots.Length)
                return;

            quickSlots[slotIndex].count = count;
            UpdateSlotVisual(slotIndex);
        }
    }
}
