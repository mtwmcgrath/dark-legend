using UnityEngine;
using UnityEngine.UI;
using DarkLegend.Mobile.Input;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Mobile skill bar (wheel or grid style)
    /// Thanh skill cho mobile (dạng wheel hoặc grid)
    /// </summary>
    public class MobileSkillBar : MonoBehaviour
    {
        [Header("Skill Bar Type")]
        public SkillBarType barType = SkillBarType.Grid;

        [Header("Skill Buttons")]
        public SkillButton[] skillButtons;
        public int maxSkills = 6;

        [Header("Grid Settings")]
        public int rows = 2;
        public int columns = 3;
        public float spacing = 10f;

        [Header("Wheel Settings")]
        public float wheelRadius = 150f;
        public Transform wheelCenter;

        public enum SkillBarType
        {
            Grid,   // 2x3 grid layout
            Wheel   // Circular layout around attack button
        }

        private void Start()
        {
            InitializeSkillBar();
        }

        /// <summary>
        /// Initialize skill bar
        /// Khởi tạo skill bar
        /// </summary>
        private void InitializeSkillBar()
        {
            if (skillButtons == null || skillButtons.Length == 0)
            {
                skillButtons = GetComponentsInChildren<SkillButton>();
            }

            // Arrange buttons based on type
            if (barType == SkillBarType.Grid)
            {
                ArrangeAsGrid();
            }
            else
            {
                ArrangeAsWheel();
            }

            Debug.Log($"[MobileSkillBar] Initialized with {skillButtons.Length} skill buttons ({barType} layout)");
        }

        /// <summary>
        /// Arrange buttons in grid layout
        /// Sắp xếp nút theo dạng grid
        /// </summary>
        private void ArrangeAsGrid()
        {
            GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = columns;
                gridLayout.spacing = new Vector2(spacing, spacing);
            }
        }

        /// <summary>
        /// Arrange buttons in wheel layout
        /// Sắp xếp nút theo dạng wheel (vòng tròn)
        /// </summary>
        private void ArrangeAsWheel()
        {
            if (wheelCenter == null || skillButtons == null)
                return;

            float angleStep = 360f / skillButtons.Length;
            float startAngle = 90f; // Start from top

            for (int i = 0; i < skillButtons.Length; i++)
            {
                float angle = startAngle - (angleStep * i);
                float radian = angle * Mathf.Deg2Rad;

                Vector2 position = new Vector2(
                    Mathf.Cos(radian) * wheelRadius,
                    Mathf.Sin(radian) * wheelRadius
                );

                RectTransform rectTransform = skillButtons[i].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = position;
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
        /// Reset all skill cooldowns
        /// Reset cooldown tất cả skill
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

        /// <summary>
        /// Enable/disable skill slot
        /// Bật/tắt skill slot
        /// </summary>
        public void SetSkillEnabled(int slotIndex, bool enabled)
        {
            if (slotIndex < 0 || slotIndex >= skillButtons.Length)
                return;

            skillButtons[slotIndex]?.SetEnabled(enabled);
        }

        /// <summary>
        /// Change skill bar type
        /// Đổi kiểu skill bar
        /// </summary>
        public void ChangeSkillBarType(SkillBarType newType)
        {
            barType = newType;
            InitializeSkillBar();
        }
    }
}
