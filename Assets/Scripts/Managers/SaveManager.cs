using UnityEngine;
using System.IO;

namespace DarkLegend.Managers
{
    /// <summary>
    /// Save data structure
    /// Cấu trúc dữ liệu lưu game
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        // Character data
        public string characterName;
        public int characterClass;
        public int level;
        public long currentExp;
        
        // Stats
        public int strength;
        public int agility;
        public int vitality;
        public int energy;
        public int availableStatPoints;
        
        // Current HP/MP
        public int currentHP;
        public int currentMP;
        
        // Position
        public float posX;
        public float posY;
        public float posZ;
        
        // Inventory
        public int gold;
        public string[] itemIDs;
        public int[] itemStacks;
        
        // Equipment
        public string[] equippedItemIDs;
        
        // Skills
        public string[] learnedSkills;
        public string[] equippedSkills;
        
        // Timestamp
        public string saveTime;
    }
    
    /// <summary>
    /// Save/Load system using JSON
    /// Hệ thống lưu/tải sử dụng JSON
    /// </summary>
    public class SaveManager : Utils.Singleton<SaveManager>
    {
        [Header("Save Settings")]
        public string saveFileName = "save_slot_";
        public int maxSaveSlots = 3;
        
        private string SavePath => Path.Combine(Application.persistentDataPath, Utils.Constants.SAVE_FOLDER);
        
        protected override void Awake()
        {
            base.Awake();
            
            // Create save directory if it doesn't exist
            if (!Directory.Exists(SavePath))
            {
                Directory.CreateDirectory(SavePath);
            }
        }
        
        /// <summary>
        /// Save game to slot
        /// Lưu game vào slot
        /// </summary>
        public bool SaveGame(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots)
            {
                Debug.LogError("Invalid save slot!");
                return false;
            }
            
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
            if (player == null)
            {
                Debug.LogError("Player not found!");
                return false;
            }
            
            // Gather save data
            SaveData saveData = GatherSaveData(player);
            
            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);
            
            // Save to file
            string filePath = GetSaveFilePath(slotIndex);
            try
            {
                File.WriteAllText(filePath, json);
                Debug.Log($"Game saved to slot {slotIndex}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Load game from slot
        /// Tải game từ slot
        /// </summary>
        public bool LoadGame(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots)
            {
                Debug.LogError("Invalid save slot!");
                return false;
            }
            
            string filePath = GetSaveFilePath(slotIndex);
            
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Save file not found in slot {slotIndex}");
                return false;
            }
            
            try
            {
                // Read file
                string json = File.ReadAllText(filePath);
                
                // Parse JSON
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                // Apply save data
                ApplySaveData(saveData);
                
                Debug.Log($"Game loaded from slot {slotIndex}");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Check if save exists in slot
        /// Kiểm tra save có tồn tại trong slot không
        /// </summary>
        public bool SaveExists(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                return false;
            
            return File.Exists(GetSaveFilePath(slotIndex));
        }
        
        /// <summary>
        /// Delete save from slot
        /// Xóa save từ slot
        /// </summary>
        public bool DeleteSave(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                return false;
            
            string filePath = GetSaveFilePath(slotIndex);
            
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    Debug.Log($"Save deleted from slot {slotIndex}");
                    return true;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Failed to delete save: {e.Message}");
                    return false;
                }
            }
            
            return false;
        }
        
        /// <summary>
        /// Get save file info
        /// Lấy thông tin file save
        /// </summary>
        public SaveData GetSaveInfo(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxSaveSlots)
                return null;
            
            string filePath = GetSaveFilePath(slotIndex);
            
            if (!File.Exists(filePath))
                return null;
            
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Gather save data from player
        /// Thu thập dữ liệu save từ người chơi
        /// </summary>
        private SaveData GatherSaveData(GameObject player)
        {
            SaveData data = new SaveData();
            
            // Character stats
            Character.CharacterStats stats = player.GetComponent<Character.CharacterStats>();
            if (stats != null)
            {
                data.characterName = stats.characterName;
                data.characterClass = (int)stats.characterClass;
                data.strength = stats.strength;
                data.agility = stats.agility;
                data.vitality = stats.vitality;
                data.energy = stats.energy;
                data.availableStatPoints = stats.availableStatPoints;
                data.currentHP = stats.currentHP;
                data.currentMP = stats.currentMP;
            }
            
            // Level system
            Character.LevelSystem levelSystem = player.GetComponent<Character.LevelSystem>();
            if (levelSystem != null)
            {
                data.level = levelSystem.currentLevel;
                data.currentExp = levelSystem.currentExp;
            }
            
            // Position
            data.posX = player.transform.position.x;
            data.posY = player.transform.position.y;
            data.posZ = player.transform.position.z;
            
            // Inventory
            Inventory.InventorySystem inventory = player.GetComponent<Inventory.InventorySystem>();
            if (inventory != null)
            {
                data.gold = inventory.gold;
                
                // Save items
                System.Collections.Generic.List<string> itemIDs = new System.Collections.Generic.List<string>();
                System.Collections.Generic.List<int> itemStacks = new System.Collections.Generic.List<int>();
                
                foreach (var item in inventory.items)
                {
                    if (item != null)
                    {
                        itemIDs.Add(item.itemID);
                        itemStacks.Add(item.currentStack);
                    }
                    else
                    {
                        itemIDs.Add("");
                        itemStacks.Add(0);
                    }
                }
                
                data.itemIDs = itemIDs.ToArray();
                data.itemStacks = itemStacks.ToArray();
            }
            
            // Timestamp
            data.saveTime = System.DateTime.Now.ToString();
            
            return data;
        }
        
        /// <summary>
        /// Apply save data to player
        /// Áp dụng dữ liệu save cho người chơi
        /// </summary>
        private void ApplySaveData(SaveData data)
        {
            // Find or spawn player
            GameObject player = GameObject.FindGameObjectWithTag(Utils.Constants.TAG_PLAYER);
            if (player == null)
            {
                Debug.LogWarning("Player not found, spawning new player...");
                // Would need to spawn player here
                return;
            }
            
            // Apply character stats
            Character.CharacterStats stats = player.GetComponent<Character.CharacterStats>();
            if (stats != null)
            {
                stats.characterName = data.characterName;
                stats.characterClass = (Character.CharacterClass)data.characterClass;
                stats.strength = data.strength;
                stats.agility = data.agility;
                stats.vitality = data.vitality;
                stats.energy = data.energy;
                stats.availableStatPoints = data.availableStatPoints;
                stats.CalculateDerivedStats();
                stats.currentHP = data.currentHP;
                stats.currentMP = data.currentMP;
            }
            
            // Apply level
            Character.LevelSystem levelSystem = player.GetComponent<Character.LevelSystem>();
            if (levelSystem != null)
            {
                levelSystem.SetLevel(data.level);
                levelSystem.currentExp = data.currentExp;
            }
            
            // Apply position
            player.transform.position = new Vector3(data.posX, data.posY, data.posZ);
            
            // Apply inventory
            Inventory.InventorySystem inventory = player.GetComponent<Inventory.InventorySystem>();
            if (inventory != null)
            {
                inventory.gold = data.gold;
                // TODO: Load items from itemIDs
            }
        }
        
        /// <summary>
        /// Get save file path for slot
        /// Lấy đường dẫn file save cho slot
        /// </summary>
        private string GetSaveFilePath(int slotIndex)
        {
            return Path.Combine(SavePath, $"{saveFileName}{slotIndex}.json");
        }
    }
}
