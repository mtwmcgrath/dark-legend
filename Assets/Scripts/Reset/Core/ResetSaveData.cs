using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset save data - Dữ liệu lưu reset
    /// Save data structure for reset system
    /// </summary>
    [System.Serializable]
    public class ResetSaveData
    {
        [Header("Reset Counts")]
        public int normalResetCount;
        public int grandResetCount;
        public bool hasMasterReset;

        [Header("Accumulated Bonuses")]
        public int totalBonusStats;
        public float totalDamageBonus;
        public float totalDefenseBonus;
        public float totalHPBonus;
        public float totalMPBonus;

        [Header("History")]
        public List<ResetHistoryEntry> history = new List<ResetHistoryEntry>();

        [Header("Timestamps")]
        public string lastResetTime; // Stored as string for serialization

        /// <summary>
        /// Create save data from character stats
        /// Tạo dữ liệu lưu từ character stats
        /// </summary>
        public static ResetSaveData CreateFromCharacter(CharacterStats character)
        {
            if (character == null)
                return new ResetSaveData();

            ResetSaveData saveData = new ResetSaveData
            {
                normalResetCount = character.normalResetCount,
                grandResetCount = character.grandResetCount,
                hasMasterReset = character.hasMasterReset,
                totalBonusStats = character.resetBonusStats,
                totalDamageBonus = character.resetDamageMultiplier - 1f,
                totalDefenseBonus = character.resetDefenseMultiplier - 1f,
                totalHPBonus = character.resetHPMultiplier - 1f,
                totalMPBonus = character.resetMPMultiplier - 1f
            };

            // Copy history
            if (character.resetHistory != null && character.resetHistory.Entries != null)
            {
                saveData.history = new List<ResetHistoryEntry>(character.resetHistory.Entries);
            }

            saveData.lastResetTime = DateTime.Now.ToString("o");

            return saveData;
        }

        /// <summary>
        /// Apply save data to character
        /// Áp dụng dữ liệu lưu lên nhân vật
        /// </summary>
        public void ApplyToCharacter(CharacterStats character)
        {
            if (character == null)
                return;

            character.normalResetCount = normalResetCount;
            character.grandResetCount = grandResetCount;
            character.hasMasterReset = hasMasterReset;
            character.resetBonusStats = totalBonusStats;
            character.resetDamageMultiplier = 1f + totalDamageBonus;
            character.resetDefenseMultiplier = 1f + totalDefenseBonus;
            character.resetHPMultiplier = 1f + totalHPBonus;
            character.resetMPMultiplier = 1f + totalMPBonus;

            // Restore history
            if (character.resetHistory == null)
                character.resetHistory = new ResetHistory();

            character.resetHistory.Entries = new List<ResetHistoryEntry>(history);
            character.resetHistory.TotalNormalResets = normalResetCount;
            character.resetHistory.TotalGrandResets = grandResetCount;
            character.resetHistory.HasMasterReset = hasMasterReset;
        }

        /// <summary>
        /// Serialize to JSON
        /// Chuyển thành JSON
        /// </summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }

        /// <summary>
        /// Deserialize from JSON
        /// Chuyển từ JSON
        /// </summary>
        public static ResetSaveData FromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return new ResetSaveData();

            try
            {
                return JsonUtility.FromJson<ResetSaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize ResetSaveData: {e.Message}");
                return new ResetSaveData();
            }
        }

        /// <summary>
        /// Get last reset time as DateTime
        /// Lấy thời gian reset cuối dưới dạng DateTime
        /// </summary>
        public DateTime GetLastResetTime()
        {
            if (string.IsNullOrEmpty(lastResetTime))
                return DateTime.MinValue;

            try
            {
                return DateTime.Parse(lastResetTime);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }

    /// <summary>
    /// Reset save manager - Quản lý lưu dữ liệu reset
    /// Manages saving and loading reset data
    /// </summary>
    public class ResetSaveManager : MonoBehaviour
    {
        private static ResetSaveManager _instance;
        public static ResetSaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("ResetSaveManager");
                    _instance = go.AddComponent<ResetSaveManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Save reset data for character
        /// Lưu dữ liệu reset cho nhân vật
        /// </summary>
        public bool SaveResetData(CharacterStats character, string saveKey)
        {
            if (character == null || string.IsNullOrEmpty(saveKey))
            {
                Debug.LogWarning("Cannot save reset data: invalid character or save key");
                return false;
            }

            try
            {
                ResetSaveData saveData = ResetSaveData.CreateFromCharacter(character);
                string json = saveData.ToJson();
                PlayerPrefs.SetString(saveKey, json);
                PlayerPrefs.Save();

                Debug.Log($"Reset data saved for {character.name}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save reset data: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Load reset data for character
        /// Tải dữ liệu reset cho nhân vật
        /// </summary>
        public bool LoadResetData(CharacterStats character, string saveKey)
        {
            if (character == null || string.IsNullOrEmpty(saveKey))
            {
                Debug.LogWarning("Cannot load reset data: invalid character or save key");
                return false;
            }

            if (!PlayerPrefs.HasKey(saveKey))
            {
                Debug.Log($"No save data found for key: {saveKey}");
                return false;
            }

            try
            {
                string json = PlayerPrefs.GetString(saveKey);
                ResetSaveData saveData = ResetSaveData.FromJson(json);
                saveData.ApplyToCharacter(character);

                Debug.Log($"Reset data loaded for {character.name}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load reset data: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete reset data
        /// Xóa dữ liệu reset
        /// </summary>
        public bool DeleteResetData(string saveKey)
        {
            if (string.IsNullOrEmpty(saveKey))
                return false;

            if (PlayerPrefs.HasKey(saveKey))
            {
                PlayerPrefs.DeleteKey(saveKey);
                PlayerPrefs.Save();
                Debug.Log($"Reset data deleted for key: {saveKey}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if save data exists
        /// Kiểm tra xem dữ liệu lưu có tồn tại không
        /// </summary>
        public bool HasResetData(string saveKey)
        {
            return !string.IsNullOrEmpty(saveKey) && PlayerPrefs.HasKey(saveKey);
        }

        /// <summary>
        /// Get default save key for character
        /// Lấy save key mặc định cho nhân vật
        /// </summary>
        public string GetDefaultSaveKey(CharacterStats character)
        {
            if (character == null)
                return "";

            return $"ResetData_{character.name}";
        }
    }
}
