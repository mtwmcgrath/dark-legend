using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild save data for persistence
    /// Dữ liệu lưu guild để lưu trữ
    /// </summary>
    [Serializable]
    public class GuildSaveData
    {
        // Basic info / Thông tin cơ bản
        public string GuildID;
        public string GuildName;
        public byte[] GuildMark;
        public string GuildType;
        
        // Guild status / Trạng thái guild
        public int Level;
        public int Experience;
        public int Points;
        public string Notice;
        public string Description;
        public string CreationDate;
        
        // Master / Chủ guild
        public string GuildMasterID;
        
        // Members / Thành viên
        public List<GuildMemberData> Members;
        
        // Bank / Kho
        public List<GuildBankItemData> BankItems;
        public int BankZen;
        
        // Settings / Cài đặt
        public GuildSettingsData Settings;
        
        // Alliance / Liên minh
        public string AllianceID;
        
        // War history / Lịch sử chiến tranh
        public List<GuildWarHistoryData> WarHistory;
        
        // Stats / Thống kê
        public int TotalWins;
        public int TotalLosses;
        public int CastleSiegeWins;
        
        public GuildSaveData()
        {
            Members = new List<GuildMemberData>();
            BankItems = new List<GuildBankItemData>();
            WarHistory = new List<GuildWarHistoryData>();
            Settings = new GuildSettingsData();
        }
    }
    
    /// <summary>
    /// Member save data
    /// Dữ liệu lưu thành viên
    /// </summary>
    [Serializable]
    public class GuildMemberData
    {
        public string PlayerID;
        public string PlayerName;
        public int PlayerLevel;
        public string CharacterClass;
        public string Rank;
        public string JoinDate;
        public string LastOnline;
        public int TotalContribution;
        public int WeeklyContribution;
        public int GuildWarsParticipated;
        public int GuildQuestsCompleted;
        public int MonstersKilledForGuild;
    }
    
    /// <summary>
    /// Bank item save data
    /// Dữ liệu lưu vật phẩm kho
    /// </summary>
    [Serializable]
    public class GuildBankItemData
    {
        public string ItemID;
        public string ItemName;
        public string Rarity;
        public int Quantity;
        public string DepositedBy;
        public string DepositDate;
    }
    
    /// <summary>
    /// Guild settings save data
    /// Dữ liệu lưu cài đặt guild
    /// </summary>
    [Serializable]
    public class GuildSettingsData
    {
        public bool AutoAcceptMembers;
        public int MinLevelRequirement;
        public bool AllowAlliance;
        public bool PublicGuild;
    }
    
    /// <summary>
    /// War history save data
    /// Dữ liệu lưu lịch sử chiến tranh
    /// </summary>
    [Serializable]
    public class GuildWarHistoryData
    {
        public string WarID;
        public string OpponentGuildID;
        public string OpponentGuildName;
        public string WarDate;
        public bool IsVictory;
        public int FinalScore;
        public int OpponentScore;
        public int Rewards;
    }
    
    /// <summary>
    /// Guild data persistence manager
    /// Quản lý lưu trữ dữ liệu guild
    /// </summary>
    public class GuildDataPersistence : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Settings")]
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5 minutes
        
        private float autoSaveTimer;
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        private void Update()
        {
            if (autoSave)
            {
                autoSaveTimer += Time.deltaTime;
                if (autoSaveTimer >= autoSaveInterval)
                {
                    SaveAllGuilds();
                    autoSaveTimer = 0f;
                }
            }
        }
        
        /// <summary>
        /// Convert Guild to GuildSaveData
        /// Chuyển đổi Guild sang GuildSaveData
        /// </summary>
        public GuildSaveData ConvertToSaveData(Guild guild)
        {
            if (guild == null)
                return null;
            
            GuildSaveData saveData = new GuildSaveData
            {
                GuildID = guild.GuildId,
                GuildName = guild.GuildName,
                GuildMark = guild.GuildMark,
                GuildType = guild.Type.ToString(),
                Level = guild.Level,
                Experience = guild.CurrentExp,
                Points = guild.Points,
                Notice = guild.Notice,
                Description = guild.Description,
                CreationDate = guild.CreationDate.ToString("o"),
                GuildMasterID = guild.GuildMasterId,
                AllianceID = guild.AllianceId,
                TotalWins = guild.TotalWins,
                TotalLosses = guild.TotalLosses,
                CastleSiegeWins = guild.CastleSiegeWins
            };
            
            // Convert members
            foreach (var member in guild.Members)
            {
                saveData.Members.Add(new GuildMemberData
                {
                    PlayerID = member.PlayerId,
                    PlayerName = member.PlayerName,
                    PlayerLevel = member.PlayerLevel,
                    CharacterClass = member.CharacterClass,
                    Rank = member.Rank.ToString(),
                    JoinDate = member.JoinDate.ToString("o"),
                    LastOnline = member.LastOnline.ToString("o"),
                    TotalContribution = member.TotalContribution,
                    WeeklyContribution = member.WeeklyContribution,
                    GuildWarsParticipated = member.GuildWarsParticipated,
                    GuildQuestsCompleted = member.GuildQuestsCompleted,
                    MonstersKilledForGuild = member.MonstersKilledForGuild
                });
            }
            
            // Convert war history
            foreach (var record in guild.WarHistory)
            {
                saveData.WarHistory.Add(new GuildWarHistoryData
                {
                    WarID = record.WarId,
                    OpponentGuildID = record.OpponentGuildId,
                    OpponentGuildName = record.OpponentGuildName,
                    WarDate = record.WarDate.ToString("o"),
                    IsVictory = record.IsVictory,
                    FinalScore = record.FinalScore,
                    OpponentScore = record.OpponentScore,
                    Rewards = record.Rewards
                });
            }
            
            return saveData;
        }
        
        /// <summary>
        /// Convert GuildSaveData to Guild
        /// Chuyển đổi GuildSaveData sang Guild
        /// </summary>
        public Guild ConvertFromSaveData(GuildSaveData saveData)
        {
            if (saveData == null)
                return null;
            
            GuildType guildType = Enum.Parse<GuildType>(saveData.GuildType);
            
            Guild guild = new Guild(
                saveData.GuildID,
                saveData.GuildName,
                saveData.GuildMasterID,
                guildType
            )
            {
                GuildMark = saveData.GuildMark,
                Level = saveData.Level,
                CurrentExp = saveData.Experience,
                Points = saveData.Points,
                Notice = saveData.Notice,
                Description = saveData.Description,
                CreationDate = DateTime.Parse(saveData.CreationDate),
                AllianceId = saveData.AllianceID,
                TotalWins = saveData.TotalWins,
                TotalLosses = saveData.TotalLosses,
                CastleSiegeWins = saveData.CastleSiegeWins
            };
            
            // Restore members
            guild.Members.Clear();
            foreach (var memberData in saveData.Members)
            {
                GuildRank rank = Enum.Parse<GuildRank>(memberData.Rank);
                GuildMember member = new GuildMember(
                    memberData.PlayerID,
                    memberData.PlayerName,
                    memberData.PlayerLevel,
                    memberData.CharacterClass
                )
                {
                    Rank = rank,
                    JoinDate = DateTime.Parse(memberData.JoinDate),
                    LastOnline = DateTime.Parse(memberData.LastOnline),
                    TotalContribution = memberData.TotalContribution,
                    WeeklyContribution = memberData.WeeklyContribution,
                    GuildWarsParticipated = memberData.GuildWarsParticipated,
                    GuildQuestsCompleted = memberData.GuildQuestsCompleted,
                    MonstersKilledForGuild = memberData.MonstersKilledForGuild
                };
                guild.Members.Add(member);
            }
            
            // Restore war history
            guild.WarHistory.Clear();
            foreach (var warData in saveData.WarHistory)
            {
                guild.WarHistory.Add(new GuildWarRecord
                {
                    WarId = warData.WarID,
                    OpponentGuildId = warData.OpponentGuildID,
                    OpponentGuildName = warData.OpponentGuildName,
                    WarDate = DateTime.Parse(warData.WarDate),
                    IsVictory = warData.IsVictory,
                    FinalScore = warData.FinalScore,
                    OpponentScore = warData.OpponentScore,
                    Rewards = warData.Rewards
                });
            }
            
            return guild;
        }
        
        /// <summary>
        /// Save guild to JSON file
        /// Lưu guild vào file JSON
        /// </summary>
        public bool SaveGuild(Guild guild, string filePath = null)
        {
            try
            {
                GuildSaveData saveData = ConvertToSaveData(guild);
                string json = JsonUtility.ToJson(saveData, true);
                
                if (string.IsNullOrEmpty(filePath))
                {
                    filePath = Application.persistentDataPath + $"/Guilds/{guild.GuildId}.json";
                }
                
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.IO.File.WriteAllText(filePath, json);
                
                Debug.Log($"Guild '{guild.GuildName}' saved to {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to save guild: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Load guild from JSON file
        /// Tải guild từ file JSON
        /// </summary>
        public Guild LoadGuild(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                {
                    Debug.LogError($"Guild file not found: {filePath}");
                    return null;
                }
                
                string json = System.IO.File.ReadAllText(filePath);
                GuildSaveData saveData = JsonUtility.FromJson<GuildSaveData>(json);
                
                Guild guild = ConvertFromSaveData(saveData);
                Debug.Log($"Guild '{guild.GuildName}' loaded from {filePath}");
                
                return guild;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load guild: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Save all guilds
        /// Lưu tất cả guild
        /// </summary>
        public void SaveAllGuilds()
        {
            if (guildManager == null)
                return;
            
            List<Guild> guilds = guildManager.GetAllGuilds();
            int savedCount = 0;
            
            foreach (Guild guild in guilds)
            {
                if (SaveGuild(guild))
                {
                    savedCount++;
                }
            }
            
            Debug.Log($"Saved {savedCount} guilds");
        }
        
        /// <summary>
        /// Load all guilds from directory
        /// Tải tất cả guild từ thư mục
        /// </summary>
        public void LoadAllGuilds()
        {
            string guildDirectory = Application.persistentDataPath + "/Guilds/";
            
            if (!System.IO.Directory.Exists(guildDirectory))
            {
                Debug.Log("No guild directory found");
                return;
            }
            
            string[] guildFiles = System.IO.Directory.GetFiles(guildDirectory, "*.json");
            int loadedCount = 0;
            
            foreach (string filePath in guildFiles)
            {
                Guild guild = LoadGuild(filePath);
                if (guild != null)
                {
                    // Register guild with guild manager
                    // This would need integration with GuildManager
                    loadedCount++;
                }
            }
            
            Debug.Log($"Loaded {loadedCount} guilds");
        }
    }
}
