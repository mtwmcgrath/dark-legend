using UnityEngine;
using UnityEngine.UI;
using DarkLegend.Mobile.Input;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Skill grid UI (MU Origin style)
    /// UI skill dạng grid (kiểu MU Origin)
    /// </summary>
    public class SkillGridUI : MonoBehaviour
    {
        [Header("Grid Settings")]
        public int rows = 2;
        public int columns = 3;
        public float spacing = 10f;
        public Vector2 cellSize = new Vector2(80f, 80f);

        [Header("Skill Buttons")]
        public SkillButton[] skillButtons;
        public Transform gridContainer;
        public GameObject skillButtonPrefab;

        [Header("Scroll Settings")]
        public ScrollRect scrollRect;
        public bool allowScroll = true;
        public int maxVisibleSkills = 6;

        private GridLayoutGroup gridLayout;

        private void Start()
        {
            InitializeGrid();
        }

        /// <summary>
        /// Initialize skill grid
        /// Khởi tạo skill grid
        /// </summary>
        private void InitializeGrid()
        {
            // Setup grid layout
            if (gridContainer != null)
            {
                gridLayout = gridContainer.GetComponent<GridLayoutGroup>();
                if (gridLayout == null)
                {
                    gridLayout = gridContainer.gameObject.AddComponent<GridLayoutGroup>();
                }

                ConfigureGridLayout();
            }

            // Get or create skill buttons
            if (skillButtons == null || skillButtons.Length == 0)
            {
                skillButtons = GetComponentsInChildren<SkillButton>();
            }

            // Create skill buttons if needed
            if (skillButtons.Length == 0 && skillButtonPrefab != null)
            {
                CreateSkillButtons();
            }

            Debug.Log($"[SkillGridUI] Initialized with {rows}x{columns} grid ({skillButtons.Length} skills)");
        }

        /// <summary>
        /// Configure grid layout
        /// Cấu hình grid layout
        /// </summary>
        private void ConfigureGridLayout()
        {
            if (gridLayout == null)
                return;

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = columns;
            gridLayout.spacing = new Vector2(spacing, spacing);
            gridLayout.cellSize = cellSize;
            gridLayout.childAlignment = TextAnchor.UpperLeft;
        }

        /// <summary>
        /// Create skill buttons
        /// Tạo các nút skill
        /// </summary>
        private void CreateSkillButtons()
        {
            if (skillButtonPrefab == null || gridContainer == null)
                return;

            int totalSlots = rows * columns;
            skillButtons = new SkillButton[totalSlots];

            for (int i = 0; i < totalSlots; i++)
            {
                GameObject buttonObj = Instantiate(skillButtonPrefab, gridContainer);
                buttonObj.name = $"SkillButton_{i}";
                
                SkillButton button = buttonObj.GetComponent<SkillButton>();
                if (button != null)
                {
                    button.skillSlotIndex = i;
                    skillButtons[i] = button;
                }
            }
        }

        /// <summary>
        /// Set skill in slot
        /// Đặt skill vào slot
        /// </summary>
        public void SetSkill(int slotIndex, Sprite icon, float cooldown, int mpCost)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            SkillButton button = skillButtons[slotIndex];
            if (button != null)
            {
                button.SetSkillIcon(icon);
                button.SetSkillCooldown(cooldown);
                button.mpCost = mpCost;
            }
        }

        /// <summary>
        /// Clear skill slot
        /// Xóa skill slot
        /// </summary>
        public void ClearSkillSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            SkillButton button = skillButtons[slotIndex];
            if (button != null)
            {
                button.SetSkillIcon(null);
                button.SetEnabled(false);
            }
        }

        /// <summary>
        /// Enable/disable skill
        /// Bật/tắt skill
        /// </summary>
        public void SetSkillEnabled(int slotIndex, bool enabled)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            skillButtons[slotIndex]?.SetEnabled(enabled);
        }

        /// <summary>
        /// Change grid size
        /// Đổi kích thước grid
        /// </summary>
        public void SetGridSize(int newRows, int newColumns)
        {
            rows = newRows;
            columns = newColumns;

            if (gridLayout != null)
            {
                gridLayout.constraintCount = columns;
            }

            // Recreate buttons if needed
            int requiredSlots = rows * columns;
            if (skillButtons.Length != requiredSlots)
            {
                // Clear old buttons
                foreach (Transform child in gridContainer)
                {
                    Destroy(child.gameObject);
                }

                // Create new buttons
                CreateSkillButtons();
            }
        }

        /// <summary>
        /// Scroll to skill
        /// Cuộn đến skill
        /// </summary>
        public void ScrollToSkill(int slotIndex)
        {
            if (!allowScroll || scrollRect == null)
                return;

            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            // Calculate normalized position
            int row = slotIndex / columns;
            float normalizedPosition = 1f - ((float)row / (rows - 1));

            scrollRect.verticalNormalizedPosition = normalizedPosition;
        }

        /// <summary>
        /// Get skill at position
        /// Lấy skill tại vị trí
        /// </summary>
        public SkillButton GetSkillAt(int row, int column)
        {
            int index = row * columns + column;
            
            if (index >= 0 && index < skillButtons.Length)
            {
                return skillButtons[index];
            }

            return null;
        }

        /// <summary>
        /// Reset all cooldowns
        /// Reset tất cả cooldown
        /// </summary>
        public void ResetAllCooldowns()
        {
            foreach (SkillButton button in skillButtons)
            {
                if (button != null)
                {
                    button.ResetCooldown();
                }
            }
        }
    }
}
