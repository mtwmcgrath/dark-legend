using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Party
{
    /// <summary>
    /// Party Finder system with search and auto-match
    /// Hệ thống tìm nhóm với tìm kiếm và ghép tự động
    /// </summary>
    public class PartyFinder : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PartyManager partyManager;
        
        // Players looking for party / Người chơi đang tìm nhóm
        private Dictionary<string, LFPRegistration> lookingForParty = 
            new Dictionary<string, LFPRegistration>();
        
        // Public parties / Nhóm công khai
        private List<Party> publicParties = new List<Party>();
        
        /// <summary>
        /// Looking For Party registration
        /// Đăng ký tìm nhóm
        /// </summary>
        [Serializable]
        public class LFPRegistration
        {
            public string PlayerId;
            public string PlayerName;
            public int Level;
            public string CharacterClass;
            public LFPSettings Settings;
            public DateTime RegistrationTime;
        }
        
        /// <summary>
        /// Looking For Party settings
        /// Cài đặt tìm nhóm
        /// </summary>
        [Serializable]
        public class LFPSettings
        {
            public List<string> PreferredClasses;  // Preferred party composition
            public int MinLevel;
            public int MaxLevel;
            public string PreferredActivity; // "Dungeon", "Boss", "Leveling", "PvP"
            public bool AutoJoin;            // Automatically join matching party
        }
        
        /// <summary>
        /// Party search filter
        /// Bộ lọc tìm kiếm nhóm
        /// </summary>
        [Serializable]
        public class PartySearchFilter
        {
            public int? MinLevel;
            public int? MaxLevel;
            public string Activity;
            public bool? HasSpace;
            public string PreferredClass;
        }
        
        private void Awake()
        {
            if (partyManager == null)
            {
                partyManager = GetComponent<PartyManager>();
            }
        }
        
        private void Update()
        {
            ProcessAutoMatch();
        }
        
        /// <summary>
        /// Register player looking for party
        /// Đăng ký người chơi tìm nhóm
        /// </summary>
        public bool RegisterLFP(string playerId, string playerName, int level, string characterClass, LFPSettings settings)
        {
            // Check if player is already in a party
            if (partyManager != null && partyManager.GetPlayerParty(playerId) != null)
            {
                Debug.LogError("Player is already in a party.");
                return false;
            }
            
            // Check if already registered
            if (lookingForParty.ContainsKey(playerId))
            {
                Debug.LogError("Player is already registered for LFP.");
                return false;
            }
            
            LFPRegistration registration = new LFPRegistration
            {
                PlayerId = playerId,
                PlayerName = playerName,
                Level = level,
                CharacterClass = characterClass,
                Settings = settings,
                RegistrationTime = DateTime.Now
            };
            
            lookingForParty[playerId] = registration;
            
            Debug.Log($"{playerName} registered for party finder.");
            
            // Try auto-match if enabled
            if (settings.AutoJoin)
            {
                AutoMatchParty(playerId);
            }
            
            return true;
        }
        
        /// <summary>
        /// Unregister player from LFP
        /// Hủy đăng ký người chơi khỏi LFP
        /// </summary>
        public bool UnregisterLFP(string playerId)
        {
            if (lookingForParty.Remove(playerId))
            {
                Debug.Log("Player unregistered from party finder.");
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Search for parties matching filter
        /// Tìm kiếm nhóm khớp với bộ lọc
        /// </summary>
        public List<Party> SearchParties(PartySearchFilter filter)
        {
            IEnumerable<Party> results = publicParties;
            
            if (filter.MinLevel.HasValue)
            {
                results = results.Where(p => 
                    p.Settings.MinLevel == 0 || p.Settings.MinLevel <= filter.MinLevel.Value);
            }
            
            if (filter.MaxLevel.HasValue)
            {
                results = results.Where(p => 
                    p.Settings.MaxLevel == 0 || p.Settings.MaxLevel >= filter.MaxLevel.Value);
            }
            
            if (!string.IsNullOrEmpty(filter.Activity))
            {
                results = results.Where(p => 
                    string.IsNullOrEmpty(p.Settings.PreferredActivity) || 
                    p.Settings.PreferredActivity.Equals(filter.Activity, StringComparison.OrdinalIgnoreCase));
            }
            
            if (filter.HasSpace == true)
            {
                results = results.Where(p => !p.IsFull);
            }
            
            if (!string.IsNullOrEmpty(filter.PreferredClass))
            {
                results = results.Where(p => 
                    !p.Members.Any(m => m.CharacterClass.Equals(filter.PreferredClass, StringComparison.OrdinalIgnoreCase)));
            }
            
            return results.OrderByDescending(p => p.Members.Count).ToList();
        }
        
        /// <summary>
        /// Auto-match player to a suitable party
        /// Tự động ghép người chơi vào nhóm phù hợp
        /// </summary>
        public Party AutoMatchParty(string playerId)
        {
            if (!lookingForParty.TryGetValue(playerId, out LFPRegistration registration))
            {
                return null;
            }
            
            // Find matching party
            PartySearchFilter filter = new PartySearchFilter
            {
                MinLevel = registration.Settings.MinLevel > 0 ? registration.Settings.MinLevel : null,
                MaxLevel = registration.Settings.MaxLevel > 0 ? registration.Settings.MaxLevel : null,
                Activity = registration.Settings.PreferredActivity,
                HasSpace = true
            };
            
            List<Party> matches = SearchParties(filter);
            
            foreach (Party party in matches)
            {
                // Check if party accepts auto-join
                if (!party.Settings.AutoAccept)
                    continue;
                
                // Check level range
                if (registration.Level < party.Settings.MinLevel || 
                    (party.Settings.MaxLevel > 0 && registration.Level > party.Settings.MaxLevel))
                    continue;
                
                // Try to join
                PartyMember member = new PartyMember(
                    registration.PlayerId,
                    registration.PlayerName,
                    registration.Level,
                    registration.CharacterClass
                );
                
                if (party.AddMember(member))
                {
                    lookingForParty.Remove(playerId);
                    Debug.Log($"{registration.PlayerName} auto-matched to party '{party.PartyName}'");
                    return party;
                }
            }
            
            // No match found, create new party
            if (registration.Settings.AutoJoin)
            {
                Party newParty = CreateAutoParty(registration);
                return newParty;
            }
            
            return null;
        }
        
        /// <summary>
        /// Create auto-generated party
        /// Tạo nhóm tự động
        /// </summary>
        private Party CreateAutoParty(LFPRegistration registration)
        {
            if (partyManager == null)
                return null;
            
            Party party = partyManager.CreateParty(
                registration.PlayerId,
                $"Auto - {registration.Settings.PreferredActivity}"
            );
            
            if (party != null)
            {
                party.Settings.PreferredActivity = registration.Settings.PreferredActivity;
                party.Settings.MinLevel = registration.Settings.MinLevel;
                party.Settings.MaxLevel = registration.Settings.MaxLevel;
                party.Settings.AutoAccept = true;
                party.Settings.IsPublic = true;
                
                RegisterPublicParty(party);
                lookingForParty.Remove(registration.PlayerId);
                
                Debug.Log($"Auto-party created: {party.PartyName}");
            }
            
            return party;
        }
        
        /// <summary>
        /// Register party as public (visible in finder)
        /// Đăng ký nhóm công khai (hiển thị trong tìm kiếm)
        /// </summary>
        public void RegisterPublicParty(Party party)
        {
            if (!publicParties.Contains(party) && party.Settings.IsPublic)
            {
                publicParties.Add(party);
            }
        }
        
        /// <summary>
        /// Unregister public party
        /// Hủy đăng ký nhóm công khai
        /// </summary>
        public void UnregisterPublicParty(Party party)
        {
            publicParties.Remove(party);
        }
        
        /// <summary>
        /// Get players looking for party
        /// Lấy người chơi đang tìm nhóm
        /// </summary>
        public List<LFPRegistration> GetLFPPlayers(string activity = null)
        {
            IEnumerable<LFPRegistration> results = lookingForParty.Values;
            
            if (!string.IsNullOrEmpty(activity))
            {
                results = results.Where(r => 
                    r.Settings.PreferredActivity.Equals(activity, StringComparison.OrdinalIgnoreCase));
            }
            
            return results.OrderByDescending(r => r.Level).ToList();
        }
        
        /// <summary>
        /// Process auto-matching for registered players
        /// Xử lý ghép tự động cho người chơi đã đăng ký
        /// </summary>
        private void ProcessAutoMatch()
        {
            // Process auto-match every few seconds to avoid performance issues
            // This is a simplified version - in production, use a timer
            
            List<string> playersToMatch = lookingForParty
                .Where(kvp => kvp.Value.Settings.AutoJoin)
                .Select(kvp => kvp.Key)
                .ToList();
            
            foreach (string playerId in playersToMatch.Take(5)) // Process 5 at a time
            {
                AutoMatchParty(playerId);
            }
        }
    }
}
