using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Party
{
    /// <summary>
    /// Party Manager - Central management for all parties
    /// Quản lý nhóm - Quản lý trung tâm cho tất cả nhóm
    /// </summary>
    public class PartyManager : MonoBehaviour
    {
        private static PartyManager _instance;
        public static PartyManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PartyManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PartyManager");
                        _instance = go.AddComponent<PartyManager>();
                    }
                }
                return _instance;
            }
        }
        
        // All parties / Tất cả nhóm
        private Dictionary<string, Party> parties = new Dictionary<string, Party>();
        
        // Player to party mapping / Ánh xạ người chơi tới nhóm
        private Dictionary<string, string> playerToParty = new Dictionary<string, string>();
        
        // Party invitations / Lời mời nhóm
        private Dictionary<string, PartyInvitation> pendingInvitations = new Dictionary<string, PartyInvitation>();
        
        [Header("Settings")]
        [SerializeField] private float invitationExpirationTime = 60f; // seconds
        
        /// <summary>
        /// Party invitation
        /// Lời mời nhóm
        /// </summary>
        [Serializable]
        public class PartyInvitation
        {
            public string InviteId;
            public string PartyId;
            public string InviterId;
            public string InviterName;
            public string TargetPlayerId;
            public DateTime InviteTime;
            public float ExpirationTime;
            
            public bool IsExpired => (DateTime.Now - InviteTime).TotalSeconds > ExpirationTime;
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Update()
        {
            CleanupExpiredInvitations();
        }
        
        /// <summary>
        /// Create a new party
        /// Tạo nhóm mới
        /// </summary>
        public Party CreateParty(string leaderId, string partyName = "")
        {
            // Check if player is already in a party
            if (playerToParty.ContainsKey(leaderId))
            {
                Debug.LogError("Player is already in a party.");
                return null;
            }
            
            string partyId = Guid.NewGuid().ToString();
            Party party = new Party(partyId, leaderId, partyName);
            
            // Add leader as first member
            PartyMember leader = new PartyMember(leaderId, "Leader", 0, "");
            party.AddMember(leader);
            
            parties[partyId] = party;
            playerToParty[leaderId] = partyId;
            
            Debug.Log($"Party '{party.PartyName}' created.");
            
            return party;
        }
        
        /// <summary>
        /// Disband party
        /// Giải tán nhóm
        /// </summary>
        public bool DisbandParty(string partyId, string requesterId)
        {
            Party party = GetParty(partyId);
            if (party == null)
            {
                Debug.LogError("Party not found.");
                return false;
            }
            
            // Only leader can disband
            if (party.LeaderId != requesterId)
            {
                Debug.LogError("Only party leader can disband the party.");
                return false;
            }
            
            // Remove all members from mapping
            foreach (var member in party.Members)
            {
                playerToParty.Remove(member.PlayerId);
            }
            
            parties.Remove(partyId);
            
            Debug.Log($"Party '{party.PartyName}' disbanded.");
            
            return true;
        }
        
        /// <summary>
        /// Send party invitation
        /// Gửi lời mời nhóm
        /// </summary>
        public bool SendInvitation(string partyId, string inviterId, string targetPlayerId)
        {
            Party party = GetParty(partyId);
            if (party == null)
            {
                Debug.LogError("Party not found.");
                return false;
            }
            
            // Check if inviter is in the party
            PartyMember inviter = party.Members.FirstOrDefault(m => m.PlayerId == inviterId);
            if (inviter == null)
            {
                Debug.LogError("Inviter is not in the party.");
                return false;
            }
            
            // Check if party is full
            if (party.IsFull)
            {
                Debug.LogError("Party is full.");
                return false;
            }
            
            // Check if target is already in a party
            if (playerToParty.ContainsKey(targetPlayerId))
            {
                Debug.LogError("Target player is already in a party.");
                return false;
            }
            
            // Check if target already has a pending invite
            if (pendingInvitations.ContainsKey(targetPlayerId))
            {
                Debug.LogError("Target player already has a pending party invitation.");
                return false;
            }
            
            PartyInvitation invitation = new PartyInvitation
            {
                InviteId = Guid.NewGuid().ToString(),
                PartyId = partyId,
                InviterId = inviterId,
                InviterName = inviter.PlayerName,
                TargetPlayerId = targetPlayerId,
                InviteTime = DateTime.Now,
                ExpirationTime = invitationExpirationTime
            };
            
            pendingInvitations[targetPlayerId] = invitation;
            
            Debug.Log($"Party invitation sent to player.");
            
            return true;
        }
        
        /// <summary>
        /// Accept party invitation
        /// Chấp nhận lời mời nhóm
        /// </summary>
        public bool AcceptInvitation(string playerId, string playerName, int level, string characterClass)
        {
            if (!pendingInvitations.TryGetValue(playerId, out PartyInvitation invitation))
            {
                Debug.LogError("No pending invitation found.");
                return false;
            }
            
            if (invitation.IsExpired)
            {
                pendingInvitations.Remove(playerId);
                Debug.LogError("Invitation has expired.");
                return false;
            }
            
            Party party = GetParty(invitation.PartyId);
            if (party == null)
            {
                pendingInvitations.Remove(playerId);
                Debug.LogError("Party no longer exists.");
                return false;
            }
            
            PartyMember member = new PartyMember(playerId, playerName, level, characterClass);
            
            if (party.AddMember(member))
            {
                playerToParty[playerId] = party.PartyId;
                pendingInvitations.Remove(playerId);
                
                Debug.Log($"{playerName} joined party '{party.PartyName}'");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Decline party invitation
        /// Từ chối lời mời nhóm
        /// </summary>
        public bool DeclineInvitation(string playerId)
        {
            if (pendingInvitations.Remove(playerId))
            {
                Debug.Log("Party invitation declined.");
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Leave party
        /// Rời nhóm
        /// </summary>
        public bool LeaveParty(string playerId)
        {
            if (!playerToParty.TryGetValue(playerId, out string partyId))
            {
                Debug.LogError("Player is not in a party.");
                return false;
            }
            
            Party party = GetParty(partyId);
            if (party == null)
            {
                return false;
            }
            
            if (party.RemoveMember(playerId))
            {
                playerToParty.Remove(playerId);
                
                // Disband if no members left
                if (party.Members.Count == 0)
                {
                    parties.Remove(partyId);
                }
                
                Debug.Log("Player left the party.");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Kick member from party
        /// Đuổi thành viên khỏi nhóm
        /// </summary>
        public bool KickMember(string partyId, string leaderId, string targetPlayerId)
        {
            Party party = GetParty(partyId);
            if (party == null)
            {
                Debug.LogError("Party not found.");
                return false;
            }
            
            // Only leader can kick
            if (party.LeaderId != leaderId)
            {
                Debug.LogError("Only party leader can kick members.");
                return false;
            }
            
            if (party.RemoveMember(targetPlayerId))
            {
                playerToParty.Remove(targetPlayerId);
                Debug.Log("Member kicked from party.");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Transfer party leadership
        /// Chuyển quyền trưởng nhóm
        /// </summary>
        public bool TransferLeadership(string partyId, string currentLeaderId, string newLeaderId)
        {
            Party party = GetParty(partyId);
            if (party == null)
            {
                Debug.LogError("Party not found.");
                return false;
            }
            
            // Only current leader can transfer
            if (party.LeaderId != currentLeaderId)
            {
                Debug.LogError("Only party leader can transfer leadership.");
                return false;
            }
            
            // Check if new leader is in party
            if (!party.Members.Any(m => m.PlayerId == newLeaderId))
            {
                Debug.LogError("Target player is not in the party.");
                return false;
            }
            
            party.LeaderId = newLeaderId;
            Debug.Log("Party leadership transferred.");
            
            return true;
        }
        
        /// <summary>
        /// Get party by ID
        /// Lấy nhóm theo ID
        /// </summary>
        public Party GetParty(string partyId)
        {
            return parties.TryGetValue(partyId, out Party party) ? party : null;
        }
        
        /// <summary>
        /// Get player's party
        /// Lấy nhóm của người chơi
        /// </summary>
        public Party GetPlayerParty(string playerId)
        {
            if (playerToParty.TryGetValue(playerId, out string partyId))
            {
                return GetParty(partyId);
            }
            return null;
        }
        
        /// <summary>
        /// Get all parties
        /// Lấy tất cả nhóm
        /// </summary>
        public List<Party> GetAllParties()
        {
            return parties.Values.ToList();
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
                if (kvp.Value.IsExpired)
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
