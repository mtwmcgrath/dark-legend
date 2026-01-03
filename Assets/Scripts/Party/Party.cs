using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Party
{
    /// <summary>
    /// Enhanced party system with loot distribution and EXP sharing
    /// Hệ thống nhóm nâng cao với phân phối chiến lợi phẩm và chia EXP
    /// </summary>
    public class Party
    {
        public string PartyId;
        public string PartyName;
        public string LeaderId;
        public List<PartyMember> Members;
        public int MaxMembers = 5;
        public PartySettings Settings;
        public DateTime CreationTime;
        
        // Loot and EXP settings / Cài đặt chiến lợi phẩm và EXP
        public LootMode LootMode;
        public bool ShareEXP = true;
        public float ExpBonus = 1.1f; // 10% bonus when in party
        
        public Party(string partyId, string leaderId, string partyName = "")
        {
            PartyId = partyId;
            LeaderId = leaderId;
            PartyName = string.IsNullOrEmpty(partyName) ? $"Party_{partyId.Substring(0, 8)}" : partyName;
            Members = new List<PartyMember>();
            Settings = new PartySettings();
            CreationTime = DateTime.Now;
            LootMode = LootMode.FreeForAll;
        }
        
        /// <summary>
        /// Add member to party
        /// Thêm thành viên vào nhóm
        /// </summary>
        public bool AddMember(PartyMember member)
        {
            if (Members.Count >= MaxMembers)
            {
                return false;
            }
            
            if (Members.Any(m => m.PlayerId == member.PlayerId))
            {
                return false;
            }
            
            // Check level restrictions
            if (Settings.MinLevel > 0 && member.Level < Settings.MinLevel)
            {
                return false;
            }
            
            if (Settings.MaxLevel > 0 && member.Level > Settings.MaxLevel)
            {
                return false;
            }
            
            Members.Add(member);
            member.JoinTime = DateTime.Now;
            
            return true;
        }
        
        /// <summary>
        /// Remove member from party
        /// Xóa thành viên khỏi nhóm
        /// </summary>
        public bool RemoveMember(string playerId)
        {
            PartyMember member = Members.FirstOrDefault(m => m.PlayerId == playerId);
            if (member == null)
            {
                return false;
            }
            
            Members.Remove(member);
            
            // Transfer leadership if leader left
            if (playerId == LeaderId && Members.Count > 0)
            {
                LeaderId = Members[0].PlayerId;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get party leader
        /// Lấy trưởng nhóm
        /// </summary>
        public PartyMember GetLeader()
        {
            return Members.FirstOrDefault(m => m.PlayerId == LeaderId);
        }
        
        /// <summary>
        /// Check if party is full
        /// Kiểm tra nhóm đã đầy chưa
        /// </summary>
        public bool IsFull => Members.Count >= MaxMembers;
        
        /// <summary>
        /// Get average party level
        /// Lấy cấp độ trung bình của nhóm
        /// </summary>
        public int GetAverageLevel()
        {
            if (Members.Count == 0)
                return 0;
            
            return Mathf.RoundToInt(Members.Average(m => m.Level));
        }
        
        /// <summary>
        /// Calculate EXP share for member
        /// Tính chia EXP cho thành viên
        /// </summary>
        public int CalculateExpShare(int totalExp, PartyMember member)
        {
            if (!ShareEXP || Members.Count == 0)
            {
                return totalExp;
            }
            
            // Base share with bonus
            int baseShare = Mathf.RoundToInt(totalExp * ExpBonus / Members.Count);
            
            // Level penalty/bonus (within 10 levels gets full share)
            int avgLevel = GetAverageLevel();
            int levelDiff = Mathf.Abs(member.Level - avgLevel);
            
            if (levelDiff > 10)
            {
                float penalty = 1f - (levelDiff - 10) * 0.05f; // 5% penalty per level over 10
                penalty = Mathf.Max(0.5f, penalty); // Minimum 50% share
                baseShare = Mathf.RoundToInt(baseShare * penalty);
            }
            
            return baseShare;
        }
    }
    
    /// <summary>
    /// Party member information
    /// Thông tin thành viên nhóm
    /// </summary>
    [Serializable]
    public class PartyMember
    {
        public string PlayerId;
        public string PlayerName;
        public int Level;
        public string CharacterClass;
        public bool IsOnline;
        public DateTime JoinTime;
        
        // Position for distance calculations
        public Vector3 Position;
        
        public PartyMember(string playerId, string playerName, int level, string characterClass)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Level = level;
            CharacterClass = characterClass;
            IsOnline = true;
            JoinTime = DateTime.Now;
        }
    }
    
    /// <summary>
    /// Party settings
    /// Cài đặt nhóm
    /// </summary>
    [Serializable]
    public class PartySettings
    {
        public int MinLevel;           // 0 = no minimum
        public int MaxLevel;           // 0 = no maximum
        public bool AutoAccept;        // Auto accept join requests
        public bool AllowPvP;          // Allow PvP between members
        public bool IsPublic = true;   // Visible in party finder
        public string PreferredActivity; // "Dungeon", "Boss", "Leveling", "PvP"
    }
    
    /// <summary>
    /// Loot distribution modes
    /// Chế độ phân phối chiến lợi phẩm
    /// </summary>
    public enum LootMode
    {
        FreeForAll,    // Ai nhặt được là của người đó
        RoundRobin,    // Luân phiên theo thứ tự
        Leader,        // Trưởng nhóm quyết định
        Random,        // Ngẫu nhiên cho 1 người
        NeedBeforeGreed // Cần thiết trước ham muốn
    }
}
