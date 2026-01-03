using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// UI hiển thị combo
    /// UI for displaying combos
    /// </summary>
    public class ComboUI : MonoBehaviour
    {
        [Header("References")]
        public ComboSystem comboSystem;
        
        [Header("UI Elements")]
        public GameObject comboPanel;
        public Text comboCountText;
        public Text comboNameText;
        public Image comboTimerBar;
        public Animator comboAnimator;
        
        [Header("Combo List")]
        public Transform comboListContainer;
        public GameObject comboItemPrefab;
        
        [Header("Settings")]
        public bool showComboList = true;
        public Color[] comboColors = new Color[] {
            Color.white,    // 1-3 hits
            Color.yellow,   // 4-6 hits
            Color.cyan,     // 7-9 hits
            Color.magenta   // 10+ hits
        };
        
        private int lastComboCount = 0;
        
        /// <summary>
        /// Initialize / Khởi tạo
        /// </summary>
        private void Start()
        {
            if (comboSystem == null)
            {
                comboSystem = FindObjectOfType<ComboSystem>();
            }
            
            if (showComboList)
            {
                PopulateComboList();
            }
            
            if (comboPanel != null)
            {
                comboPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// Update UI / Cập nhật UI
        /// </summary>
        private void Update()
        {
            if (comboSystem == null) return;
            
            UpdateComboDisplay();
            UpdateTimerBar();
        }
        
        /// <summary>
        /// Update combo display / Cập nhật hiển thị combo
        /// </summary>
        private void UpdateComboDisplay()
        {
            bool hasCombo = comboSystem.currentComboCount > 0;
            
            // Show/hide panel
            if (comboPanel != null && comboPanel.activeSelf != hasCombo)
            {
                comboPanel.SetActive(hasCombo);
            }
            
            if (!hasCombo) return;
            
            // Update combo count
            if (comboCountText != null)
            {
                comboCountText.text = $"{comboSystem.currentComboCount} HIT COMBO!";
                
                // Change color based on combo count
                Color textColor = GetComboColor(comboSystem.currentComboCount);
                comboCountText.color = textColor;
            }
            
            // Trigger animation nếu combo tăng
            if (comboSystem.currentComboCount > lastComboCount && comboAnimator != null)
            {
                comboAnimator.SetTrigger("ComboHit");
            }
            
            lastComboCount = comboSystem.currentComboCount;
        }
        
        /// <summary>
        /// Update timer bar / Cập nhật thanh timer
        /// </summary>
        private void UpdateTimerBar()
        {
            if (comboTimerBar == null || comboSystem == null) return;
            
            float fillAmount = comboSystem.currentComboTimer / comboSystem.comboWindow;
            comboTimerBar.fillAmount = fillAmount;
        }
        
        /// <summary>
        /// Lấy màu dựa trên combo count / Get color based on combo count
        /// </summary>
        private Color GetComboColor(int comboCount)
        {
            if (comboCount <= 3)
            {
                return comboColors[0];
            }
            else if (comboCount <= 6)
            {
                return comboColors[1];
            }
            else if (comboCount <= 9)
            {
                return comboColors[2];
            }
            else
            {
                return comboColors[3];
            }
        }
        
        /// <summary>
        /// Populate combo list / Điền danh sách combo
        /// </summary>
        private void PopulateComboList()
        {
            if (comboListContainer == null || comboItemPrefab == null || comboSystem == null)
            {
                return;
            }
            
            // Clear existing items
            foreach (Transform child in comboListContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create combo items
            foreach (ComboData combo in comboSystem.availableCombos)
            {
                CreateComboListItem(combo);
            }
        }
        
        /// <summary>
        /// Tạo combo list item / Create combo list item
        /// </summary>
        private void CreateComboListItem(ComboData combo)
        {
            GameObject item = Instantiate(comboItemPrefab, comboListContainer);
            
            // Set combo name
            Text nameText = item.transform.Find("Name")?.GetComponent<Text>();
            if (nameText != null)
            {
                nameText.text = combo.comboName;
            }
            
            // Set combo sequence
            Text sequenceText = item.transform.Find("Sequence")?.GetComponent<Text>();
            if (sequenceText != null)
            {
                sequenceText.text = GetSequenceString(combo);
            }
            
            // Set combo icon
            Image iconImage = item.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null && combo.comboIcon != null)
            {
                iconImage.sprite = combo.comboIcon;
            }
            
            // Update progress
            UpdateComboProgress(item, combo);
        }
        
        /// <summary>
        /// Lấy string sequence / Get sequence string
        /// </summary>
        private string GetSequenceString(ComboData combo)
        {
            if (combo.skillSequence.Count == 0) return "";
            
            string sequence = "";
            for (int i = 0; i < combo.skillSequence.Count; i++)
            {
                sequence += combo.skillSequence[i];
                if (i < combo.skillSequence.Count - 1)
                {
                    sequence += " → ";
                }
            }
            
            return sequence;
        }
        
        /// <summary>
        /// Update combo progress / Cập nhật tiến độ combo
        /// </summary>
        private void UpdateComboProgress(GameObject item, ComboData combo)
        {
            if (comboSystem == null) return;
            
            float progress = combo.GetProgress(comboSystem.currentComboSequence);
            
            Image progressBar = item.transform.Find("Progress")?.GetComponent<Image>();
            if (progressBar != null)
            {
                progressBar.fillAmount = progress;
            }
            
            Text progressText = item.transform.Find("ProgressText")?.GetComponent<Text>();
            if (progressText != null)
            {
                int current = Mathf.RoundToInt(progress * combo.skillSequence.Count);
                progressText.text = $"{current}/{combo.skillSequence.Count}";
            }
        }
    }
}
