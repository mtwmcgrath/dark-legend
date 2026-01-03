using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Character
{
    /// <summary>
    /// Character slot management / Quản lý slot nhân vật
    /// </summary>
    public class CharacterSlot : MonoBehaviour
    {
        [Header("Slot Settings / Cài đặt slot")]
        [SerializeField] private int maxSlots = 5;
        
        // Character slots / Các slot nhân vật
        private CharacterData[] characterSlots;
        private int currentSlotIndex = -1;
        
        // Singleton instance
        private static CharacterSlot instance;
        public static CharacterSlot Instance => instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSlots();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize character slots / Khởi tạo các slot nhân vật
        /// </summary>
        private void InitializeSlots()
        {
            characterSlots = new CharacterData[maxSlots];
        }
        
        /// <summary>
        /// Create character in slot / Tạo nhân vật trong slot
        /// </summary>
        public bool CreateCharacterInSlot(int slotIndex, CharacterData characterData)
        {
            if (slotIndex < 0 || slotIndex >= maxSlots)
            {
                Debug.LogError($"Invalid slot index: {slotIndex}");
                return false;
            }
            
            if (characterSlots[slotIndex] != null)
            {
                Debug.LogWarning($"Slot {slotIndex} is already occupied");
                return false;
            }
            
            characterSlots[slotIndex] = characterData;
            Debug.Log($"Character created in slot {slotIndex}: {characterData.CharacterName}");
            return true;
        }
        
        /// <summary>
        /// Delete character from slot / Xóa nhân vật khỏi slot
        /// </summary>
        public bool DeleteCharacter(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSlots)
            {
                Debug.LogError($"Invalid slot index: {slotIndex}");
                return false;
            }
            
            if (characterSlots[slotIndex] == null)
            {
                Debug.LogWarning($"Slot {slotIndex} is already empty");
                return false;
            }
            
            var characterName = characterSlots[slotIndex].CharacterName;
            characterSlots[slotIndex] = null;
            Debug.Log($"Character deleted from slot {slotIndex}: {characterName}");
            
            return true;
        }
        
        /// <summary>
        /// Select character slot / Chọn slot nhân vật
        /// </summary>
        public bool SelectCharacter(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSlots)
            {
                Debug.LogError($"Invalid slot index: {slotIndex}");
                return false;
            }
            
            if (characterSlots[slotIndex] == null)
            {
                Debug.LogWarning($"Slot {slotIndex} is empty");
                return false;
            }
            
            currentSlotIndex = slotIndex;
            Debug.Log($"Selected character: {characterSlots[slotIndex].CharacterName}");
            return true;
        }
        
        /// <summary>
        /// Get character from slot / Lấy nhân vật từ slot
        /// </summary>
        public CharacterData GetCharacter(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSlots)
                return null;
                
            return characterSlots[slotIndex];
        }
        
        /// <summary>
        /// Get current selected character / Lấy nhân vật đang chọn
        /// </summary>
        public CharacterData GetCurrentCharacter()
        {
            if (currentSlotIndex < 0)
                return null;
                
            return characterSlots[currentSlotIndex];
        }
        
        /// <summary>
        /// Get all characters / Lấy tất cả nhân vật
        /// </summary>
        public CharacterData[] GetAllCharacters()
        {
            return characterSlots;
        }
        
        /// <summary>
        /// Check if slot is empty / Kiểm tra slot trống
        /// </summary>
        public bool IsSlotEmpty(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSlots)
                return true;
                
            return characterSlots[slotIndex] == null;
        }
        
        /// <summary>
        /// Get number of characters / Lấy số lượng nhân vật
        /// </summary>
        public int GetCharacterCount()
        {
            int count = 0;
            foreach (var character in characterSlots)
            {
                if (character != null)
                    count++;
            }
            return count;
        }
        
        /// <summary>
        /// Get available slot index / Lấy chỉ số slot còn trống
        /// </summary>
        public int GetAvailableSlotIndex()
        {
            for (int i = 0; i < maxSlots; i++)
            {
                if (characterSlots[i] == null)
                    return i;
            }
            return -1;
        }
        
        /// <summary>
        /// Check if slots are full / Kiểm tra slots đã đầy
        /// </summary>
        public bool AreSlotsFullCheck()
        {
            return GetCharacterCount() >= maxSlots;
        }
        
        /// <summary>
        /// Get max slots / Lấy số slots tối đa
        /// </summary>
        public int GetMaxSlots()
        {
            return maxSlots;
        }
    }
}
