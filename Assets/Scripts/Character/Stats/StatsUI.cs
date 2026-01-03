using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.Character
{
    /// <summary>
    /// Stats UI display / Hiển thị UI chỉ số
    /// </summary>
    public class StatsUI : MonoBehaviour
    {
        [Header("Stats Text Elements / Phần tử text chỉ số")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private TextMeshProUGUI agilityText;
        [SerializeField] private TextMeshProUGUI vitalityText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI commandText;
        [SerializeField] private TextMeshProUGUI freePointsText;
        
        [Header("Derived Stats / Chỉ số phái sinh")]
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI mpText;
        [SerializeField] private TextMeshProUGUI physDamageText;
        [SerializeField] private TextMeshProUGUI magicDamageText;
        [SerializeField] private TextMeshProUGUI defenseText;
        [SerializeField] private TextMeshProUGUI defenseRateText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI critRateText;
        [SerializeField] private TextMeshProUGUI critDamageText;
        
        [Header("Buttons / Nút")]
        [SerializeField] private Button strPlusButton;
        [SerializeField] private Button agiPlusButton;
        [SerializeField] private Button vitPlusButton;
        [SerializeField] private Button enePlusButton;
        [SerializeField] private Button cmdPlusButton;
        
        private CharacterStats currentStats;
        
        private void Start()
        {
            SetupButtons();
        }
        
        /// <summary>
        /// Setup button listeners / Thiết lập listener cho nút
        /// </summary>
        private void SetupButtons()
        {
            if (strPlusButton != null)
                strPlusButton.onClick.AddListener(() => AddStatPoint("Strength"));
                
            if (agiPlusButton != null)
                agiPlusButton.onClick.AddListener(() => AddStatPoint("Agility"));
                
            if (vitPlusButton != null)
                vitPlusButton.onClick.AddListener(() => AddStatPoint("Vitality"));
                
            if (enePlusButton != null)
                enePlusButton.onClick.AddListener(() => AddStatPoint("Energy"));
                
            if (cmdPlusButton != null)
                cmdPlusButton.onClick.AddListener(() => AddStatPoint("Command"));
        }
        
        /// <summary>
        /// Set stats to display / Đặt chỉ số để hiển thị
        /// </summary>
        public void SetStats(CharacterStats stats)
        {
            currentStats = stats;
            
            if (stats != null)
            {
                stats.OnStatChanged += OnStatChanged;
                stats.OnLevelUp += OnLevelUp;
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Update UI / Cập nhật UI
        /// </summary>
        private void UpdateUI()
        {
            if (currentStats == null)
                return;
                
            // Base stats / Chỉ số cơ bản
            SetText(levelText, $"Level: {currentStats.Level}");
            SetText(strengthText, $"STR: {currentStats.Strength}");
            SetText(agilityText, $"AGI: {currentStats.Agility}");
            SetText(vitalityText, $"VIT: {currentStats.Vitality}");
            SetText(energyText, $"ENE: {currentStats.Energy}");
            SetText(commandText, $"CMD: {currentStats.Command}");
            SetText(freePointsText, $"Free Points: {currentStats.FreePoints}");
            
            // Derived stats / Chỉ số phái sinh
            SetText(hpText, $"HP: {currentStats.MaxHP}");
            SetText(mpText, $"MP: {currentStats.MaxMP}");
            SetText(physDamageText, $"Phys DMG: {currentStats.PhysicalDamage}");
            SetText(magicDamageText, $"Magic DMG: {currentStats.MagicDamage}");
            SetText(defenseText, $"DEF: {currentStats.Defense}");
            SetText(defenseRateText, $"DEF Rate: {currentStats.DefenseRate}");
            SetText(attackSpeedText, $"ATK Speed: {currentStats.AttackSpeed:F2}");
            SetText(moveSpeedText, $"Move Speed: {currentStats.MovementSpeed:F2}");
            SetText(critRateText, $"Crit Rate: {currentStats.CriticalRate:F1}%");
            SetText(critDamageText, $"Crit DMG: {currentStats.CriticalDamage:F1}%");
            
            // Update button states / Cập nhật trạng thái nút
            UpdateButtonStates();
        }
        
        /// <summary>
        /// Update button states / Cập nhật trạng thái nút
        /// </summary>
        private void UpdateButtonStates()
        {
            bool hasPoints = currentStats != null && currentStats.FreePoints > 0;
            
            if (strPlusButton != null)
                strPlusButton.interactable = hasPoints;
                
            if (agiPlusButton != null)
                agiPlusButton.interactable = hasPoints;
                
            if (vitPlusButton != null)
                vitPlusButton.interactable = hasPoints;
                
            if (enePlusButton != null)
                enePlusButton.interactable = hasPoints;
                
            if (cmdPlusButton != null)
                cmdPlusButton.interactable = hasPoints;
        }
        
        /// <summary>
        /// Add stat point / Thêm điểm chỉ số
        /// </summary>
        private void AddStatPoint(string statName)
        {
            if (currentStats == null)
                return;
                
            currentStats.AddStatPoint(statName);
        }
        
        /// <summary>
        /// Handle stat changed event / Xử lý sự kiện chỉ số thay đổi
        /// </summary>
        private void OnStatChanged(string statName, int amount)
        {
            UpdateUI();
        }
        
        /// <summary>
        /// Handle level up event / Xử lý sự kiện lên cấp
        /// </summary>
        private void OnLevelUp(int newLevel)
        {
            UpdateUI();
        }
        
        /// <summary>
        /// Set text safely / Đặt text an toàn
        /// </summary>
        private void SetText(TextMeshProUGUI textElement, string text)
        {
            if (textElement != null)
                textElement.text = text;
        }
        
        private void OnDestroy()
        {
            if (currentStats != null)
            {
                currentStats.OnStatChanged -= OnStatChanged;
                currentStats.OnLevelUp -= OnLevelUp;
            }
        }
    }
}
