using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.UI
{
    /// <summary>
    /// HUD Controller - displays HP, MP, EXP bars
    /// Điều khiển HUD - hiển thị thanh HP, MP, EXP
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Health Bar")]
        public Slider hpSlider;
        public TextMeshProUGUI hpText;
        public Image hpFillImage;
        public Color hpColor = Color.red;
        
        [Header("Mana Bar")]
        public Slider mpSlider;
        public TextMeshProUGUI mpText;
        public Image mpFillImage;
        public Color mpColor = Color.blue;
        
        [Header("Experience Bar")]
        public Slider expSlider;
        public TextMeshProUGUI expText;
        public Image expFillImage;
        public Color expColor = Color.yellow;
        
        [Header("Level Display")]
        public TextMeshProUGUI levelText;
        
        [Header("Character Name")]
        public TextMeshProUGUI characterNameText;
        
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
                characterStats.OnHPChanged += UpdateHealthBar;
                characterStats.OnMPChanged += UpdateManaBar;
            }
            
            if (levelSystem != null)
            {
                levelSystem.OnExpChanged += UpdateExpBar;
                levelSystem.OnLevelUp += UpdateLevel;
            }
            
            // Set colors
            if (hpFillImage != null) hpFillImage.color = hpColor;
            if (mpFillImage != null) mpFillImage.color = mpColor;
            if (expFillImage != null) expFillImage.color = expColor;
            
            // Initial update
            UpdateAllBars();
        }
        
        /// <summary>
        /// Update all bars
        /// Cập nhật tất cả các thanh
        /// </summary>
        private void UpdateAllBars()
        {
            if (characterStats != null)
            {
                UpdateHealthBar(characterStats.currentHP, characterStats.maxHP);
                UpdateManaBar(characterStats.currentMP, characterStats.maxMP);
                
                if (characterNameText != null)
                {
                    characterNameText.text = characterStats.characterName;
                }
            }
            
            if (levelSystem != null)
            {
                UpdateExpBar(levelSystem.currentExp, levelSystem.expToNextLevel);
                UpdateLevel(levelSystem.currentLevel);
            }
        }
        
        /// <summary>
        /// Update health bar display
        /// Cập nhật hiển thị thanh máu
        /// </summary>
        private void UpdateHealthBar(int current, int max)
        {
            if (hpSlider != null)
            {
                hpSlider.maxValue = max;
                hpSlider.value = current;
            }
            
            if (hpText != null)
            {
                hpText.text = $"{current} / {max}";
            }
        }
        
        /// <summary>
        /// Update mana bar display
        /// Cập nhật hiển thị thanh mana
        /// </summary>
        private void UpdateManaBar(int current, int max)
        {
            if (mpSlider != null)
            {
                mpSlider.maxValue = max;
                mpSlider.value = current;
            }
            
            if (mpText != null)
            {
                mpText.text = $"{current} / {max}";
            }
        }
        
        /// <summary>
        /// Update experience bar display
        /// Cập nhật hiển thị thanh kinh nghiệm
        /// </summary>
        private void UpdateExpBar(long current, long required)
        {
            if (expSlider != null)
            {
                expSlider.maxValue = required;
                expSlider.value = current;
            }
            
            if (expText != null)
            {
                float percentage = (float)current / required * 100f;
                expText.text = $"{percentage:F1}%";
            }
        }
        
        /// <summary>
        /// Update level display
        /// Cập nhật hiển thị level
        /// </summary>
        private void UpdateLevel(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Lv. {level}";
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (characterStats != null)
            {
                characterStats.OnHPChanged -= UpdateHealthBar;
                characterStats.OnMPChanged -= UpdateManaBar;
            }
            
            if (levelSystem != null)
            {
                levelSystem.OnExpChanged -= UpdateExpBar;
                levelSystem.OnLevelUp -= UpdateLevel;
            }
        }
    }
}
