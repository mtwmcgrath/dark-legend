using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Hệ thống combo cho skills
    /// Combo system for skills
    /// </summary>
    public class ComboSystem : MonoBehaviour
    {
        [Header("Combo Settings")]
        public float comboWindow = 2f;           // Thời gian để thực hiện combo
        public float comboDamageMultiplier = 1.5f; // Damage bonus khi combo
        public int maxComboCount = 10;
        
        [Header("Combo Data")]
        public List<ComboData> availableCombos = new List<ComboData>();
        
        [Header("Current State")]
        public int currentComboCount = 0;
        public float currentComboTimer = 0f;
        public List<string> currentComboSequence = new List<string>();
        
        private ComboData activeCombo = null;
        private GameObject owner;
        
        /// <summary>
        /// Initialize / Khởi tạo
        /// </summary>
        private void Awake()
        {
            owner = gameObject;
        }
        
        /// <summary>
        /// Update để xử lý combo timer / Update to handle combo timer
        /// </summary>
        private void Update()
        {
            if (currentComboTimer > 0f)
            {
                currentComboTimer -= Time.deltaTime;
                
                if (currentComboTimer <= 0f)
                {
                    ResetCombo();
                }
            }
        }
        
        /// <summary>
        /// Register một skill được sử dụng / Register a skill used
        /// </summary>
        public void RegisterSkillUse(string skillName)
        {
            // Reset timer
            currentComboTimer = comboWindow;
            
            // Thêm vào sequence
            currentComboSequence.Add(skillName);
            
            // Tăng combo count
            currentComboCount++;
            if (currentComboCount > maxComboCount)
            {
                currentComboCount = maxComboCount;
            }
            
            // Check có match combo nào không
            CheckForCombo();
            
            Debug.Log($"Combo: {currentComboCount} hits");
        }
        
        /// <summary>
        /// Kiểm tra sequence có match combo nào không / Check if sequence matches a combo
        /// </summary>
        private void CheckForCombo()
        {
            foreach (ComboData combo in availableCombos)
            {
                if (combo.IsSequenceMatch(currentComboSequence))
                {
                    ExecuteCombo(combo);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Thực hiện combo / Execute combo
        /// </summary>
        private void ExecuteCombo(ComboData combo)
        {
            activeCombo = combo;
            
            Debug.Log($"COMBO ACTIVATED: {combo.comboName}!");
            
            // Execute combo finisher nếu có
            if (combo.finisherSkill != null)
            {
                var skillManager = owner.GetComponent<SkillManager>();
                if (skillManager != null)
                {
                    // TODO: Execute finisher skill
                }
            }
            
            // Apply combo bonuses
            ApplyComboBonuses(combo);
            
            // Reset sequence nhưng giữ combo count
            currentComboSequence.Clear();
        }
        
        /// <summary>
        /// Áp dụng combo bonuses / Apply combo bonuses
        /// </summary>
        private void ApplyComboBonuses(ComboData combo)
        {
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats == null) return;
            
            // Apply damage bonus
            float bonusDamage = combo.damageBonus * currentComboCount;
            // TODO: Apply temporary damage bonus
            
            Debug.Log($"Combo bonus: +{bonusDamage} damage");
        }
        
        /// <summary>
        /// Lấy damage multiplier từ combo / Get damage multiplier from combo
        /// </summary>
        public float GetDamageMultiplier()
        {
            if (currentComboCount <= 1) return 1f;
            
            float multiplier = 1f + ((currentComboCount - 1) * 0.1f); // +10% per combo hit
            
            if (activeCombo != null)
            {
                multiplier *= activeCombo.damageMultiplier;
            }
            
            return multiplier;
        }
        
        /// <summary>
        /// Reset combo / Reset combo
        /// </summary>
        public void ResetCombo()
        {
            currentComboCount = 0;
            currentComboTimer = 0f;
            currentComboSequence.Clear();
            activeCombo = null;
            
            Debug.Log("Combo reset");
        }
        
        /// <summary>
        /// Break combo khi bị hit / Break combo when hit
        /// </summary>
        public void BreakCombo()
        {
            if (currentComboCount > 0)
            {
                Debug.Log($"Combo broken at {currentComboCount} hits!");
                ResetCombo();
            }
        }
    }
}
