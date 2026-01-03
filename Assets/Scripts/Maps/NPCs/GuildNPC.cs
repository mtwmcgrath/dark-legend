using UnityEngine;

namespace DarkLegend.Maps.NPCs
{
    /// <summary>
    /// NPC guild / Guild master NPC
    /// </summary>
    public class GuildNPC : NPCBase
    {
        [Header("Guild Configuration")]
        [Tooltip("Chi phí tạo guild / Guild creation cost")]
        [SerializeField] private int guildCreationCost = 1000000;
        
        [Tooltip("Level tối thiểu / Minimum level")]
        [SerializeField] private int minLevelToCreate = 100;
        
        [Tooltip("Số member tối đa / Max members")]
        [SerializeField] private int maxGuildMembers = 50;
        
        [Header("Guild Features")]
        [Tooltip("Cho phép guild alliance / Allow alliances")]
        [SerializeField] private bool allowAlliances = true;
        
        [Tooltip("Cho phép guild war / Allow guild wars")]
        [SerializeField] private bool allowGuildWars = true;
        
        protected override void InitializeNPC()
        {
            base.InitializeNPC();
            
            Debug.Log($"[GuildNPC] Guild NPC initialized: {npcName}");
        }
        
        protected override void OpenNPCUI(GameObject player)
        {
            Debug.Log($"[GuildNPC] Opening guild UI");
            
            // Check if player is in guild
            bool isInGuild = IsPlayerInGuild(player);
            
            if (isInGuild)
            {
                ShowGuildManagementUI(player);
            }
            else
            {
                ShowGuildCreationUI(player);
            }
        }
        
        /// <summary>
        /// Hiển thị UI tạo guild / Show guild creation UI
        /// </summary>
        private void ShowGuildCreationUI(GameObject player)
        {
            ShowDialog($"Tạo guild với {guildCreationCost} Zen (Cần level {minLevelToCreate})");
            // TODO: Show creation UI
        }
        
        /// <summary>
        /// Hiển thị UI quản lý guild / Show guild management UI
        /// </summary>
        private void ShowGuildManagementUI(GameObject player)
        {
            Debug.Log($"[GuildNPC] Showing guild management UI");
            // TODO: Show management UI
        }
        
        /// <summary>
        /// Tạo guild / Create guild
        /// </summary>
        public bool CreateGuild(GameObject player, string guildName)
        {
            // Check level requirement
            // TODO: Get player level
            int playerLevel = 100; // Placeholder
            
            if (playerLevel < minLevelToCreate)
            {
                ShowDialog($"Bạn cần level {minLevelToCreate} để tạo guild!");
                return false;
            }
            
            // TODO: Check if player has enough Zen
            // TODO: Check if guild name is valid and unique
            // TODO: Create guild in database
            
            Debug.Log($"[GuildNPC] Guild created: {guildName}");
            ShowDialog($"Chúc mừng! Guild '{guildName}' đã được tạo!");
            
            return true;
        }
        
        /// <summary>
        /// Mời vào guild / Invite to guild
        /// </summary>
        public bool InviteToGuild(GameObject inviter, GameObject invitee)
        {
            // TODO: Check if inviter is guild master or officer
            // TODO: Check if invitee is not in guild
            // TODO: Check if guild has space
            
            Debug.Log($"[GuildNPC] Guild invitation sent");
            return true;
        }
        
        /// <summary>
        /// Rời guild / Leave guild
        /// </summary>
        public bool LeaveGuild(GameObject player)
        {
            // TODO: Check if player is guild master
            // TODO: Remove player from guild
            
            Debug.Log($"[GuildNPC] Player left guild");
            ShowDialog("Bạn đã rời guild.");
            
            return true;
        }
        
        /// <summary>
        /// Đá khỏi guild / Kick from guild
        /// </summary>
        public bool KickFromGuild(GameObject kicker, string memberName)
        {
            // TODO: Check if kicker has permission
            // TODO: Remove member from guild
            
            Debug.Log($"[GuildNPC] Kicked {memberName} from guild");
            return true;
        }
        
        /// <summary>
        /// Tạo alliance / Create alliance
        /// </summary>
        public bool CreateAlliance(GameObject player, string targetGuild)
        {
            if (!allowAlliances)
            {
                ShowDialog("Alliance không khả dụng!");
                return false;
            }
            
            // TODO: Check if player is guild master
            // TODO: Send alliance request
            
            Debug.Log($"[GuildNPC] Alliance request sent to {targetGuild}");
            return true;
        }
        
        /// <summary>
        /// Tuyên chiến / Declare war
        /// </summary>
        public bool DeclareWar(GameObject player, string targetGuild)
        {
            if (!allowGuildWars)
            {
                ShowDialog("Guild war không khả dụng!");
                return false;
            }
            
            // TODO: Check if player is guild master
            // TODO: Declare war
            
            Debug.Log($"[GuildNPC] War declared against {targetGuild}");
            ShowDialog($"Đã tuyên chiến với guild {targetGuild}!");
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra player có trong guild / Check if player is in guild
        /// </summary>
        private bool IsPlayerInGuild(GameObject player)
        {
            // TODO: Check player's guild status
            return false;
        }
        
        /// <summary>
        /// Nâng cấp guild / Upgrade guild
        /// </summary>
        public bool UpgradeGuild(GameObject player)
        {
            // TODO: Implement guild upgrade system
            Debug.Log($"[GuildNPC] Guild upgraded");
            return true;
        }
    }
}
