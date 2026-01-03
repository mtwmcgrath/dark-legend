using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.Character
{
    /// <summary>
    /// Manager for class selection and changes / Quản lý chọn và đổi class
    /// </summary>
    public class ClassManager : MonoBehaviour
    {
        [Header("Class Data / Dữ liệu class")]
        [SerializeField] private List<ClassData> allClasses = new List<ClassData>();
        
        [Header("Current Character / Nhân vật hiện tại")]
        [SerializeField] private ClassData currentClass;
        [SerializeField] private CharacterStats currentStats;
        
        // Singleton instance
        private static ClassManager instance;
        public static ClassManager Instance => instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Get all available base classes / Lấy tất cả classes cơ bản có sẵn
        /// </summary>
        public List<ClassData> GetAvailableBaseClasses()
        {
            return allClasses.Where(c => c.IsBaseClass() && !c.IsUnlockableClass()).ToList();
        }
        
        /// <summary>
        /// Get all unlockable classes / Lấy tất cả classes có thể mở khóa
        /// </summary>
        public List<ClassData> GetUnlockableClasses()
        {
            return allClasses.Where(c => c.IsUnlockableClass()).ToList();
        }
        
        /// <summary>
        /// Check if class is unlocked / Kiểm tra class đã mở khóa
        /// </summary>
        public bool IsClassUnlocked(ClassData classData)
        {
            // Base classes always unlocked / Classes cơ bản luôn mở khóa
            if (!classData.IsUnlockableClass())
                return true;
                
            // Check if any character meets level requirement
            // For now, always return true for demo purposes
            // TODO: Implement actual unlock checking
            return true;
        }
        
        /// <summary>
        /// Select a class / Chọn một class
        /// </summary>
        public bool SelectClass(ClassData classData)
        {
            if (!IsClassUnlocked(classData))
            {
                Debug.LogWarning($"Class {classData.ClassName} is not unlocked");
                return false;
            }
            
            currentClass = classData;
            InitializeStatsForClass(classData);
            Debug.Log($"Selected class: {classData.ClassName}");
            return true;
        }
        
        /// <summary>
        /// Initialize stats for selected class / Khởi tạo chỉ số cho class đã chọn
        /// </summary>
        private void InitializeStatsForClass(ClassData classData)
        {
            if (currentStats == null)
                currentStats = new CharacterStats();
                
            currentStats.Strength = classData.BaseStrength;
            currentStats.Agility = classData.BaseAgility;
            currentStats.Vitality = classData.BaseVitality;
            currentStats.Energy = classData.BaseEnergy;
            currentStats.Command = classData.BaseCommand;
            currentStats.Level = 1;
            currentStats.FreePoints = 0;
        }
        
        /// <summary>
        /// Get class data by type / Lấy dữ liệu class theo loại
        /// </summary>
        public ClassData GetClassData(CharacterClassType classType)
        {
            return allClasses.FirstOrDefault(c => c.ClassType == classType);
        }
        
        /// <summary>
        /// Check if can evolve current class / Kiểm tra có thể tiến hóa class hiện tại
        /// </summary>
        public bool CanEvolveCurrentClass()
        {
            if (currentClass == null || currentClass.NextEvolution == null)
                return false;
                
            if (currentStats.Level < currentClass.NextEvolution.UnlockLevel)
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Evolve current class / Tiến hóa class hiện tại
        /// </summary>
        public bool EvolveClass()
        {
            if (!CanEvolveCurrentClass())
                return false;
                
            var nextEvolution = currentClass.NextEvolution;
            if (nextEvolution == null)
                return false;
                
            // Save current stats / Lưu chỉ số hiện tại
            var stats = currentStats;
            
            // Change to evolved class / Đổi sang class tiến hóa
            currentClass = nextEvolution;
            
            // Keep stats but potentially add bonuses / Giữ chỉ số nhưng có thể thêm bonus
            // TODO: Add evolution bonuses
            
            Debug.Log($"Evolved to: {currentClass.ClassName}");
            return true;
        }
        
        /// <summary>
        /// Get current class / Lấy class hiện tại
        /// </summary>
        public ClassData GetCurrentClass()
        {
            return currentClass;
        }
        
        /// <summary>
        /// Get current stats / Lấy chỉ số hiện tại
        /// </summary>
        public CharacterStats GetCurrentStats()
        {
            return currentStats;
        }
        
        /// <summary>
        /// Add a class data to the manager / Thêm dữ liệu class vào manager
        /// </summary>
        public void RegisterClassData(ClassData classData)
        {
            if (!allClasses.Contains(classData))
            {
                allClasses.Add(classData);
            }
        }
        
        /// <summary>
        /// Get all registered classes / Lấy tất cả classes đã đăng ký
        /// </summary>
        public List<ClassData> GetAllClasses()
        {
            return allClasses;
        }
    }
}
