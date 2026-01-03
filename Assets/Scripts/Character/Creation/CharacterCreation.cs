using UnityEngine;
using System;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character creation system / Hệ thống tạo nhân vật
    /// </summary>
    public class CharacterCreation : MonoBehaviour
    {
        [Header("Creation Settings / Cài đặt tạo nhân vật")]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private int bonusStatPoints = 10;
        
        // Current creation state / Trạng thái tạo hiện tại
        private ClassData selectedClass;
        private CharacterAppearanceData appearanceData;
        private string characterName;
        private CharacterStats tempStats;
        
        // Events / Sự kiện
        public event Action<ClassData> OnClassSelected;
        public event Action<CharacterAppearanceData> OnAppearanceChanged;
        public event Action<string> OnNameChanged;
        public event Action<CharacterData> OnCharacterCreated;
        
        /// <summary>
        /// Select a class / Chọn class
        /// </summary>
        public bool SelectClass(ClassData classData)
        {
            if (classData == null)
                return false;
                
            // Check if class is unlocked / Kiểm tra class đã mở khóa
            if (classData.IsUnlockableClass())
            {
                // TODO: Check unlock requirements
                Debug.LogWarning($"Class {classData.ClassName} requires unlocking");
            }
            
            selectedClass = classData;
            
            // Initialize stats / Khởi tạo chỉ số
            InitializeStats();
            
            OnClassSelected?.Invoke(classData);
            return true;
        }
        
        /// <summary>
        /// Initialize stats for selected class / Khởi tạo chỉ số cho class đã chọn
        /// </summary>
        private void InitializeStats()
        {
            tempStats = new CharacterStats
            {
                Strength = selectedClass.BaseStrength,
                Agility = selectedClass.BaseAgility,
                Vitality = selectedClass.BaseVitality,
                Energy = selectedClass.BaseEnergy,
                Command = selectedClass.BaseCommand,
                Level = startingLevel,
                FreePoints = bonusStatPoints
            };
        }
        
        /// <summary>
        /// Customize appearance / Tùy chỉnh ngoại hình
        /// </summary>
        public void SetAppearance(CharacterAppearanceData appearance)
        {
            appearanceData = appearance;
            OnAppearanceChanged?.Invoke(appearance);
        }
        
        /// <summary>
        /// Set character name / Đặt tên nhân vật
        /// </summary>
        public bool SetCharacterName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Debug.LogWarning("Character name cannot be empty");
                return false;
            }
            
            if (name.Length < 3 || name.Length > 16)
            {
                Debug.LogWarning("Character name must be 3-16 characters");
                return false;
            }
            
            characterName = name;
            OnNameChanged?.Invoke(name);
            return true;
        }
        
        /// <summary>
        /// Allocate stat point / Phân bổ điểm chỉ số
        /// </summary>
        public bool AllocateStatPoint(string statName, int amount = 1)
        {
            if (tempStats == null)
                return false;
                
            return tempStats.AddStatPoint(statName, amount);
        }
        
        /// <summary>
        /// Get current stats / Lấy chỉ số hiện tại
        /// </summary>
        public CharacterStats GetCurrentStats()
        {
            return tempStats;
        }
        
        /// <summary>
        /// Create character / Tạo nhân vật
        /// </summary>
        public CharacterData CreateCharacter()
        {
            if (selectedClass == null)
            {
                Debug.LogError("No class selected");
                return null;
            }
            
            if (string.IsNullOrWhiteSpace(characterName))
            {
                Debug.LogError("No character name set");
                return null;
            }
            
            var characterData = new CharacterData
            {
                CharacterName = characterName,
                ClassType = selectedClass.ClassType,
                Stats = tempStats,
                Appearance = appearanceData,
                CreationDate = DateTime.Now
            };
            
            OnCharacterCreated?.Invoke(characterData);
            Debug.Log($"Character created: {characterName} ({selectedClass.ClassName})");
            
            // Reset creation state / Đặt lại trạng thái tạo
            ResetCreation();
            
            return characterData;
        }
        
        /// <summary>
        /// Reset creation state / Đặt lại trạng thái tạo
        /// </summary>
        public void ResetCreation()
        {
            selectedClass = null;
            appearanceData = new CharacterAppearanceData();
            characterName = string.Empty;
            tempStats = null;
        }
        
        /// <summary>
        /// Get selected class / Lấy class đã chọn
        /// </summary>
        public ClassData GetSelectedClass()
        {
            return selectedClass;
        }
    }
    
    /// <summary>
    /// Character data / Dữ liệu nhân vật
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        public string CharacterName;
        public CharacterClassType ClassType;
        public CharacterStats Stats;
        public CharacterAppearanceData Appearance;
        public DateTime CreationDate;
        
        // Inventory and equipment will be added later
        // Túi đồ và trang bị sẽ được thêm sau
    }
}
