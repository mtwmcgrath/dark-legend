using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild invitation system
    /// Hệ thống mời vào guild
    /// </summary>
    public class GuildInvite : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Settings")]
        [SerializeField] private float inviteExpirationTime = 60f; // seconds
        
        // Pending invitations / Lời mời đang chờ
        private Dictionary<string, GuildInvitation> pendingInvites = new Dictionary<string, GuildInvitation>();
        
        /// <summary>
        /// Guild invitation data
        /// Dữ liệu lời mời guild
        /// </summary>
        [Serializable]
        public class GuildInvitation
        {
            public string InviteId;
            public string GuildId;
            public string GuildName;
            public string InviterId;
            public string InviterName;
            public string TargetPlayerId;
            public string TargetPlayerName;
            public DateTime InviteTime;
            public float ExpirationTime;
            
            public bool IsExpired => (DateTime.Now - InviteTime).TotalSeconds > ExpirationTime;
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
            // Clean up expired invitations
            CleanupExpiredInvitations();
        }
        
        /// <summary>
        /// Send guild invitation to player
        /// Gửi lời mời guild cho người chơi
        /// </summary>
        public bool SendInvitation(string guildId, string inviterId, string targetPlayerId, string targetPlayerName)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            // Check if inviter has permission
            GuildMember inviter = guild.GetMember(inviterId);
            if (inviter == null)
            {
                Debug.LogError("Inviter is not a member of the guild.");
                return false;
            }
            
            GuildRankPermissions permissions = inviter.GetPermissions();
            if (!permissions.CanInviteMembers)
            {
                Debug.LogError("You don't have permission to invite members.");
                return false;
            }
            
            // Check if target is already in a guild
            if (guildManager.GetPlayerGuild(targetPlayerId) != null)
            {
                Debug.LogError("Player is already in a guild.");
                return false;
            }
            
            // Check if guild is full
            GuildData data = guildManager.GetGuildData();
            if (guild.Members.Count >= data.GetMaxMembers(guild.Level))
            {
                Debug.LogError("Guild is full.");
                return false;
            }
            
            // Check if player already has a pending invite from this guild
            if (pendingInvites.ContainsKey(targetPlayerId))
            {
                GuildInvitation existing = pendingInvites[targetPlayerId];
                if (existing.GuildId == guildId && !existing.IsExpired)
                {
                    Debug.LogError("Player already has a pending invitation from this guild.");
                    return false;
                }
            }
            
            // Create invitation
            GuildInvitation invitation = new GuildInvitation
            {
                InviteId = Guid.NewGuid().ToString(),
                GuildId = guildId,
                GuildName = guild.GuildName,
                InviterId = inviterId,
                InviterName = inviter.PlayerName,
                TargetPlayerId = targetPlayerId,
                TargetPlayerName = targetPlayerName,
                InviteTime = DateTime.Now,
                ExpirationTime = inviteExpirationTime
            };
            
            pendingInvites[targetPlayerId] = invitation;
            
            Debug.Log($"Guild invitation sent to {targetPlayerName}");
            OnInvitationSent(invitation);
            
            return true;
        }
        
        /// <summary>
        /// Accept guild invitation
        /// Chấp nhận lời mời guild
        /// </summary>
        public bool AcceptInvitation(string playerId, string playerName, int playerLevel, string characterClass)
        {
            if (!pendingInvites.TryGetValue(playerId, out GuildInvitation invitation))
            {
                Debug.LogError("No pending invitation found.");
                return false;
            }
            
            if (invitation.IsExpired)
            {
                pendingInvites.Remove(playerId);
                Debug.LogError("Invitation has expired.");
                return false;
            }
            
            Guild guild = guildManager.GetGuild(invitation.GuildId);
            if (guild == null)
            {
                pendingInvites.Remove(playerId);
                Debug.LogError("Guild no longer exists.");
                return false;
            }
            
            // Create new member
            GuildMember newMember = new GuildMember(playerId, playerName, playerLevel, characterClass);
            
            // Add member to guild
            if (guildManager.AddMemberToGuild(invitation.GuildId, newMember))
            {
                pendingInvites.Remove(playerId);
                Debug.Log($"{playerName} joined guild {guild.GuildName}");
                OnInvitationAccepted(invitation, newMember);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Decline guild invitation
        /// Từ chối lời mời guild
        /// </summary>
        public bool DeclineInvitation(string playerId)
        {
            if (!pendingInvites.TryGetValue(playerId, out GuildInvitation invitation))
            {
                Debug.LogError("No pending invitation found.");
                return false;
            }
            
            pendingInvites.Remove(playerId);
            Debug.Log($"Invitation declined by {invitation.TargetPlayerName}");
            OnInvitationDeclined(invitation);
            
            return true;
        }
        
        /// <summary>
        /// Cancel sent invitation
        /// Hủy lời mời đã gửi
        /// </summary>
        public bool CancelInvitation(string guildId, string inviterId, string targetPlayerId)
        {
            if (!pendingInvites.TryGetValue(targetPlayerId, out GuildInvitation invitation))
            {
                Debug.LogError("No pending invitation found.");
                return false;
            }
            
            if (invitation.GuildId != guildId)
            {
                Debug.LogError("Invitation is not from your guild.");
                return false;
            }
            
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return false;
            }
            
            GuildMember member = guild.GetMember(inviterId);
            if (member == null || !member.GetPermissions().CanInviteMembers)
            {
                Debug.LogError("You don't have permission to cancel invitations.");
                return false;
            }
            
            pendingInvites.Remove(targetPlayerId);
            Debug.Log("Invitation cancelled.");
            
            return true;
        }
        
        /// <summary>
        /// Get pending invitation for player
        /// Lấy lời mời đang chờ cho người chơi
        /// </summary>
        public GuildInvitation GetPendingInvitation(string playerId)
        {
            if (pendingInvites.TryGetValue(playerId, out GuildInvitation invitation))
            {
                if (!invitation.IsExpired)
                {
                    return invitation;
                }
                pendingInvites.Remove(playerId);
            }
            return null;
        }
        
        /// <summary>
        /// Clean up expired invitations
        /// Dọn dẹp các lời mời hết hạn
        /// </summary>
        private void CleanupExpiredInvitations()
        {
            List<string> expiredKeys = new List<string>();
            
            foreach (var kvp in pendingInvites)
            {
                if (kvp.Value.IsExpired)
                {
                    expiredKeys.Add(kvp.Key);
                }
            }
            
            foreach (string key in expiredKeys)
            {
                pendingInvites.Remove(key);
            }
        }
        
        #region Events / Sự kiện
        
        private void OnInvitationSent(GuildInvitation invitation)
        {
            // Send notification to target player
            // Update UI
        }
        
        private void OnInvitationAccepted(GuildInvitation invitation, GuildMember newMember)
        {
            // Notify guild members
            // Send welcome message
            // Update UI
        }
        
        private void OnInvitationDeclined(GuildInvitation invitation)
        {
            // Notify inviter
        }
        
        #endregion
    }
}
