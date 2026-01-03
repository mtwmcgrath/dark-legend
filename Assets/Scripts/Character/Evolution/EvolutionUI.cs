using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.Character
{
    /// <summary>
    /// Evolution UI / Giao diện tiến hóa
    /// </summary>
    public class EvolutionUI : MonoBehaviour
    {
        [Header("UI Elements / Phần tử UI")]
        [SerializeField] private GameObject evolutionPanel;
        [SerializeField] private TextMeshProUGUI evolutionNameText;
        [SerializeField] private TextMeshProUGUI requirementsText;
        [SerializeField] private TextMeshProUGUI bonusesText;
        [SerializeField] private Button evolveButton;
        [SerializeField] private Button cancelButton;
        
        private EvolutionSystem evolutionSystem;
        private CharacterClassType currentClass;
        private CharacterStats currentStats;
        
        private void Start()
        {
            SetupButtons();
            evolutionPanel?.SetActive(false);
        }
        
        /// <summary>
        /// Setup button listeners / Thiết lập listener cho nút
        /// </summary>
        private void SetupButtons()
        {
            if (evolveButton != null)
                evolveButton.onClick.AddListener(OnEvolveClicked);
                
            if (cancelButton != null)
                cancelButton.onClick.AddListener(OnCancelClicked);
        }
        
        /// <summary>
        /// Show evolution UI / Hiển thị UI tiến hóa
        /// </summary>
        public void ShowEvolution(EvolutionSystem system, CharacterClassType classType, CharacterStats stats)
        {
            evolutionSystem = system;
            currentClass = classType;
            currentStats = stats;
            
            UpdateUI();
            evolutionPanel?.SetActive(true);
        }
        
        /// <summary>
        /// Hide evolution UI / Ẩn UI tiến hóa
        /// </summary>
        public void Hide()
        {
            evolutionPanel?.SetActive(false);
        }
        
        /// <summary>
        /// Update UI / Cập nhật UI
        /// </summary>
        private void UpdateUI()
        {
            if (evolutionSystem == null)
                return;
                
            var requirements = evolutionSystem.GetRequirements(currentClass);
            var bonuses = evolutionSystem.GetBonuses(currentClass);
            
            if (requirements != null)
            {
                SetText(requirementsText, FormatRequirements(requirements));
            }
            
            if (bonuses != null)
            {
                SetText(bonusesText, FormatBonuses(bonuses));
            }
        }
        
        /// <summary>
        /// Format requirements / Định dạng yêu cầu
        /// </summary>
        private string FormatRequirements(EvolutionRequirement req)
        {
            string text = "Requirements / Yêu cầu:\n";
            text += $"- Level: {req.RequiredLevel}\n";
            
            if (!string.IsNullOrEmpty(req.EvolutionQuestId))
                text += $"- Quest: {req.EvolutionQuestId}\n";
                
            if (req.RequiredZen > 0)
                text += $"- Zen: {req.RequiredZen}\n";
                
            return text;
        }
        
        /// <summary>
        /// Format bonuses / Định dạng phần thưởng
        /// </summary>
        private string FormatBonuses(EvolutionBonus bonus)
        {
            string text = "Bonuses / Phần thưởng:\n";
            text += $"- Free Points: +{bonus.BonusStatPoints}\n";
            
            if (bonus.BonusStrength > 0)
                text += $"- STR: +{bonus.BonusStrength}\n";
                
            if (bonus.BonusAgility > 0)
                text += $"- AGI: +{bonus.BonusAgility}\n";
                
            return text;
        }
        
        /// <summary>
        /// Handle evolve button clicked / Xử lý nút tiến hóa được nhấn
        /// </summary>
        private void OnEvolveClicked()
        {
            if (evolutionSystem != null)
            {
                // TODO: Check actual requirements
                evolutionSystem.Evolve(ref currentClass, currentStats, false, false, 0);
            }
            Hide();
        }
        
        /// <summary>
        /// Handle cancel button clicked / Xử lý nút hủy được nhấn
        /// </summary>
        private void OnCancelClicked()
        {
            Hide();
        }
        
        /// <summary>
        /// Set text safely / Đặt text an toàn
        /// </summary>
        private void SetText(TextMeshProUGUI textElement, string text)
        {
            if (textElement != null)
                textElement.text = text;
        }
    }
}
