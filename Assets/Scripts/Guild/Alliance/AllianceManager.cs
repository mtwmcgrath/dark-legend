using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Alliance Manager - Manages all guild alliances
    /// Quản lý liên minh - Quản lý tất cả liên minh guild
    /// </summary>
    public class AllianceManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        // All alliances / Tất cả liên minh
        private Dictionary<string, GuildAlliance> alliances = new Dictionary<string, GuildAlliance>();
        
        // Guild to alliance mapping / Ánh xạ guild tới liên minh
        private Dictionary<string, string> guildToAlliance = new Dictionary<string, string>();
        
        // Alliance invitations / Lời mời liên minh
        private Dictionary<string, AllianceInvitation> pendingInvitations = new Dictionary<string, AllianceInvitation>();
        
        [Header("Settings")]
        [SerializeField] private int invitationExpirationHours = 48;
        
        /// <summary>
        /// Alliance invitation
        /// Lời mời liên minh
        /// </summary>
        [Serializable]
        public class AllianceInvitation
        {
            public string InvitationId;
            public string AllianceId;
            public string AllianceName;
            public string InvitingGuildId;
            public string TargetGuildId;
            public DateTime InvitationTime;
            
            public bool IsExpired(int expirationHours) => 
                (DateTime.Now - InvitationTime).TotalHours > expirationHours;
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        private void Update()
        {
            CleanupExpiredInvitations();
            UpdateAllianceStats();
        }
        
        /// <summary>
        /// Create new alliance
        /// Tạo liên minh mới
        /// </summary>
        public GuildAlliance CreateAlliance(string allianceName, string leaderGuildId, string requesterId)
        {
            Guild leaderGuild = guildManager.GetGuild(leaderGuildId);
            if (leaderGuild == null)
            {
                Debug.LogError("Guild not found.");
                return null;
            }
            
            // Check if requester is guild master
            GuildMember requester = leaderGuild.GetMember(requesterId);
            if (requester == null || !requester.GetPermissions().CanManageAlliance)
            {
                Debug.LogError("You don't have permission to create alliances.");
                return null;
            }
            
            // Check if guild is already in an alliance
            if (guildToAlliance.ContainsKey(leaderGuildId))
            {
                Debug.LogError("Guild is already in an alliance.");
                return null;
            }
            
            // Check if alliance name is taken
            if (alliances.Values.Any(a => a.AllianceName.Equals(allianceName, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.LogError("Alliance name is already taken.");
                return null;
            }
            
            // Create alliance
            string allianceId = Guid.NewGuid().ToString();
            GuildAlliance alliance = new GuildAlliance(allianceId, allianceName, leaderGuildId);
            
            alliances[allianceId] = alliance;
            guildToAlliance[leaderGuildId] = allianceId;
            leaderGuild.AllianceId = allianceId;
            
            Debug.Log($"Alliance '{allianceName}' created by guild '{leaderGuild.GuildName}'");
            
            return alliance;
        }
        
        /// <summary>
        /// Invite guild to alliance
        /// Mời guild vào liên minh
        /// </summary>
        public bool InviteGuild(string allianceId, string invitingGuildId, string targetGuildId, string requesterId)
        {
            GuildAlliance alliance = GetAlliance(allianceId);
            if (alliance == null)
            {
                Debug.LogError("Alliance not found.");
                return false;
            }
            
            Guild invitingGuild = guildManager.GetGuild(invitingGuildId);
            Guild targetGuild = guildManager.GetGuild(targetGuildId);
            
            if (invitingGuild == null || targetGuild == null)
            {
                Debug.LogError("One or both guilds not found.");
                return false;
            }
            
            // Check permissions
            GuildMember requester = invitingGuild.GetMember(requesterId);
            if (requester == null || !requester.GetPermissions().CanManageAlliance)
            {
                Debug.LogError("You don't have permission to invite guilds.");
                return false;
            }
            
            // Check if inviting guild is in the alliance
            if (!alliance.MemberGuildIds.Contains(invitingGuildId))
            {
                Debug.LogError("Your guild is not in this alliance.");
                return false;
            }
            
            // Check if target guild is already in an alliance
            if (guildToAlliance.ContainsKey(targetGuildId))
            {
                Debug.LogError("Target guild is already in an alliance.");
                return false;
            }
            
            // Check if alliance is full
            if (alliance.IsFull)
            {
                Debug.LogError("Alliance is full.");
                return false;
            }
            
            // Check if there's already a pending invitation
            if (pendingInvitations.ContainsKey(targetGuildId))
            {
                Debug.LogError("Guild already has a pending alliance invitation.");
                return false;
            }
            
            // Create invitation
            AllianceInvitation invitation = new AllianceInvitation
            {
                InvitationId = Guid.NewGuid().ToString(),
                AllianceId = allianceId,
                AllianceName = alliance.AllianceName,
                InvitingGuildId = invitingGuildId,
                TargetGuildId = targetGuildId,
                InvitationTime = DateTime.Now
            };
            
            pendingInvitations[targetGuildId] = invitation;
            
            Debug.Log($"Guild '{invitingGuild.GuildName}' invited '{targetGuild.GuildName}' to alliance '{alliance.AllianceName}'");
            
            return true;
        }
        
        /// <summary>
        /// Accept alliance invitation
        /// Chấp nhận lời mời liên minh
        /// </summary>
        public bool AcceptInvitation(string guildId, string accepterId)
        {
            if (!pendingInvitations.TryGetValue(guildId, out AllianceInvitation invitation))
            {
                Debug.LogError("No pending alliance invitation found.");
                return false;
            }
            
            if (invitation.IsExpired(invitationExpirationHours))
            {
                pendingInvitations.Remove(guildId);
                Debug.LogError("Invitation has expired.");
                return false;
            }
            
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return false;
            }
            
            GuildMember accepter = guild.GetMember(accepterId);
            if (accepter == null || !accepter.GetPermissions().CanManageAlliance)
            {
                Debug.LogError("You don't have permission to accept alliance invitations.");
                return false;
            }
            
            GuildAlliance alliance = GetAlliance(invitation.AllianceId);
            if (alliance == null)
            {
                pendingInvitations.Remove(guildId);
                Debug.LogError("Alliance no longer exists.");
                return false;
            }
            
            if (alliance.AddGuild(guildId))
            {
                guildToAlliance[guildId] = alliance.AllianceId;
                guild.AllianceId = alliance.AllianceId;
                pendingInvitations.Remove(guildId);
                
                Debug.Log($"Guild '{guild.GuildName}' joined alliance '{alliance.AllianceName}'");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Leave alliance
        /// Rời liên minh
        /// </summary>
        public bool LeaveAlliance(string guildId, string requesterId)
        {
            if (!guildToAlliance.TryGetValue(guildId, out string allianceId))
            {
                Debug.LogError("Guild is not in an alliance.");
                return false;
            }
            
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return false;
            }
            
            GuildMember requester = guild.GetMember(requesterId);
            if (requester == null || !requester.GetPermissions().CanManageAlliance)
            {
                Debug.LogError("You don't have permission to leave alliances.");
                return false;
            }
            
            GuildAlliance alliance = GetAlliance(allianceId);
            if (alliance == null)
            {
                return false;
            }
            
            // Leader cannot leave, must transfer or disband
            if (alliance.LeaderGuildId == guildId)
            {
                Debug.LogError("Alliance leader must transfer leadership or disband the alliance.");
                return false;
            }
            
            if (alliance.RemoveGuild(guildId))
            {
                guildToAlliance.Remove(guildId);
                guild.AllianceId = null;
                
                Debug.Log($"Guild '{guild.GuildName}' left alliance '{alliance.AllianceName}'");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Disband alliance
        /// Giải tán liên minh
        /// </summary>
        public bool DisbandAlliance(string allianceId, string requesterId)
        {
            GuildAlliance alliance = GetAlliance(allianceId);
            if (alliance == null)
            {
                Debug.LogError("Alliance not found.");
                return false;
            }
            
            Guild leaderGuild = guildManager.GetGuild(alliance.LeaderGuildId);
            if (leaderGuild == null)
            {
                return false;
            }
            
            GuildMember requester = leaderGuild.GetMember(requesterId);
            if (requester == null || !requester.GetPermissions().CanManageAlliance)
            {
                Debug.LogError("You don't have permission to disband alliances.");
                return false;
            }
            
            // Remove all guilds from alliance
            foreach (string guildId in alliance.MemberGuildIds)
            {
                Guild guild = guildManager.GetGuild(guildId);
                if (guild != null)
                {
                    guild.AllianceId = null;
                }
                guildToAlliance.Remove(guildId);
            }
            
            alliances.Remove(allianceId);
            
            Debug.Log($"Alliance '{alliance.AllianceName}' disbanded");
            
            return true;
        }
        
        /// <summary>
        /// Get alliance by ID
        /// Lấy liên minh theo ID
        /// </summary>
        public GuildAlliance GetAlliance(string allianceId)
        {
            return alliances.TryGetValue(allianceId, out GuildAlliance alliance) ? alliance : null;
        }
        
        /// <summary>
        /// Get guild's alliance
        /// Lấy liên minh của guild
        /// </summary>
        public GuildAlliance GetGuildAlliance(string guildId)
        {
            if (guildToAlliance.TryGetValue(guildId, out string allianceId))
            {
                return GetAlliance(allianceId);
            }
            return null;
        }
        
        /// <summary>
        /// Get all alliances
        /// Lấy tất cả liên minh
        /// </summary>
        public List<GuildAlliance> GetAllAlliances()
        {
            return alliances.Values.ToList();
        }
        
        /// <summary>
        /// Update alliance statistics
        /// Cập nhật thống kê liên minh
        /// </summary>
        private void UpdateAllianceStats()
        {
            foreach (var alliance in alliances.Values)
            {
                int totalMembers = 0;
                int totalLevel = 0;
                int guildCount = 0;
                
                foreach (string guildId in alliance.MemberGuildIds)
                {
                    Guild guild = guildManager.GetGuild(guildId);
                    if (guild != null)
                    {
                        totalMembers += guild.Members.Count;
                        totalLevel += guild.Level;
                        guildCount++;
                    }
                }
                
                alliance.TotalMembers = totalMembers;
                alliance.AverageLevel = guildCount > 0 ? totalLevel / guildCount : 0;
            }
        }
        
        /// <summary>
        /// Clean up expired invitations
        /// Dọn dẹp lời mời hết hạn
        /// </summary>
        private void CleanupExpiredInvitations()
        {
            List<string> expiredKeys = new List<string>();
            
            foreach (var kvp in pendingInvitations)
            {
                if (kvp.Value.IsExpired(invitationExpirationHours))
                {
                    expiredKeys.Add(kvp.Key);
                }
            }
            
            foreach (string key in expiredKeys)
            {
                pendingInvitations.Remove(key);
            }
        }
    }
}
