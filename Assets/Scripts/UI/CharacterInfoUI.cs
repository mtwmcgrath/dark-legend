using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.UI
{
    /// <summary>
    /// Character info UI panel
    /// Giao diện panel thông tin nhân vật
    /// </summary>
    public class CharacterInfoUI : MonoBehaviour
    {
        [Header("UI Panel")]
        public GameObject characterPanel;
        
        [Header("Character Info")]
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI characterClassText;
        public TextMeshProUGUI characterLevelText;
        
        [Header("Stats Display")]
        public TextMeshProUGUI strengthText;
        public TextMeshProUGUI agilityText;
        public TextMeshProUGUI vitalityText;
        public TextMeshProUGUI energyText;
        
        [Header("Derived Stats")]
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI mpText;
        public TextMeshProUGUI physicalDamageText;
        public TextMeshProUGUI magicDamageText;
        public TextMeshProUGUI defenseText;
        public TextMeshProUGUI attackSpeedText;
        public TextMeshProUGUI moveSpeedText;
        
        [Header("Stat Points")]
        public TextMeshProUGUI availablePointsText;
        public Button strPlusButton;
        public Button agiPlusButton;
        public Button vitPlusButton;
        public Button enePlusButton;
        
        [Header("References")]
        public Character.CharacterStats characterStats;
        public Character.LevelSystem levelSystem;
        
        private void Start()
        {
            // Find player if not assigned
            if (characterStats == null || levelSystem == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
                if (player != null)
                {
                    characterStats = player.GetComponent<Character.CharacterStats>();
                    levelSystem = player.GetComponent<Character.LevelSystem>();
                }
            }
            
            // Subscribe to events
            if (characterStats != null)
            {
                characterStats.OnStatsChanged += RefreshStats;
            }
            
            if (levelSystem != null)
            {
                levelSystem.OnLevelUp += OnLevelUp;
            }
            
            // Setup buttons
            if (strPlusButton != null) strPlusButton.onClick.AddListener(() => AddStatPoint("strength"));
            if (agiPlusButton != null) agiPlusButton.onClick.AddListener(() => AddStatPoint("agility"));
            if (vitPlusButton != null) vitPlusButton.onClick.AddListener(() => AddStatPoint("vitality"));
            if (enePlusButton != null) enePlusButton.onClick.AddListener(() => AddStatPoint("energy"));
            
            // Start with panel closed
            if (characterPanel != null)
            {
                characterPanel.SetActive(false);
            }
            
            RefreshStats();
        }
        
        private void Update()
        {
            // Toggle character panel with C key
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleCharacterPanel();
            }
        }
        
        /// <summary>
        /// Toggle character panel
        /// Bật/tắt panel nhân vật
        /// </summary>
        public void ToggleCharacterPanel()
        {
            if (characterPanel != null)
            {
                bool isActive = !characterPanel.activeSelf;
                characterPanel.SetActive(isActive);
                
                if (isActive)
                {
                    RefreshStats();
                }
            }
        }
        
        /// <summary>
        /// Refresh all stats display
        /// Làm mới hiển thị tất cả chỉ số
        /// </summary>
        private void RefreshStats()
        {
            if (characterStats == null) return;
            
            // Basic info
            if (characterNameText != null)
                characterNameText.text = characterStats.characterName;
            
            if (characterClassText != null)
                characterClassText.text = characterStats.characterClass.ToString();
            
            if (levelSystem != null && characterLevelText != null)
                characterLevelText.text = $"Level {levelSystem.currentLevel}";
            
            // Core stats
            if (strengthText != null)
                strengthText.text = $"STR: {characterStats.strength}";
            
            if (agilityText != null)
                agilityText.text = $"AGI: {characterStats.agility}";
            
            if (vitalityText != null)
                vitalityText.text = $"VIT: {characterStats.vitality}";
            
            if (energyText != null)
                energyText.text = $"ENE: {characterStats.energy}";
            
            // Derived stats
            if (hpText != null)
                hpText.text = $"HP: {characterStats.maxHP}";
            
            if (mpText != null)
                mpText.text = $"MP: {characterStats.maxMP}";
            
            if (physicalDamageText != null)
                physicalDamageText.text = $"Physical Dmg: {characterStats.physicalDamage:F1}";
            
            if (magicDamageText != null)
                magicDamageText.text = $"Magic Dmg: {characterStats.magicDamage:F1}";
            
            if (defenseText != null)
                defenseText.text = $"Defense: {characterStats.defense:F1}";
            
            if (attackSpeedText != null)
                attackSpeedText.text = $"Atk Speed: {characterStats.attackSpeed:F2}";
            
            if (moveSpeedText != null)
                moveSpeedText.text = $"Move Speed: {characterStats.moveSpeed:F1}";
            
            // Available points
            if (availablePointsText != null)
                availablePointsText.text = $"Available Points: {characterStats.availableStatPoints}";
            
            // Update button interactability
            bool hasPoints = characterStats.availableStatPoints > 0;
            if (strPlusButton != null) strPlusButton.interactable = hasPoints;
            if (agiPlusButton != null) agiPlusButton.interactable = hasPoints;
            if (vitPlusButton != null) vitPlusButton.interactable = hasPoints;
            if (enePlusButton != null) enePlusButton.interactable = hasPoints;
        }
        
        /// <summary>
        /// Add stat point to specific stat
        /// Thêm điểm chỉ số vào chỉ số cụ thể
        /// </summary>
        private void AddStatPoint(string statName)
        {
            if (characterStats != null && characterStats.AddStatPoint(statName))
            {
                RefreshStats();
            }
        }
        
        /// <summary>
        /// Handle level up event
        /// Xử lý sự kiện tăng cấp
        /// </summary>
        private void OnLevelUp(int newLevel)
        {
            RefreshStats();
        }
        
        private void OnDestroy()
        {
            if (characterStats != null)
            {
                characterStats.OnStatsChanged -= RefreshStats;
            }
            
            if (levelSystem != null)
            {
                levelSystem.OnLevelUp -= OnLevelUp;
            }
        }
    }
}
