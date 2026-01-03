using UnityEngine;
using System;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP Title - Danh hiệu PvP
    /// </summary>
    [Serializable]
    public class TitleRequirement
    {
        public enum RequirementType
        {
            PvPKills,
            ArenaWins,
            DuelWins,
            ReachRank,
            WinStreak,
            GuildWars
        }
        
        public RequirementType type;
        public int value;
        public RankTier rankTier;
    }

    [Serializable]
    public class StatBonus
    {
        public float damageBonus = 0f;           // % bonus
        public float defenseBonus = 0f;
        public float allStatsBonus = 0f;
        public float pvpDamageBonus = 0f;
        public float critChanceBonus = 0f;
        
        public string GetBonusDescription()
        {
            string desc = "";
            if (damageBonus > 0) desc += $"+{damageBonus}% Damage ";
            if (defenseBonus > 0) desc += $"+{defenseBonus}% Defense ";
            if (allStatsBonus > 0) desc += $"+{allStatsBonus}% All Stats ";
            if (pvpDamageBonus > 0) desc += $"+{pvpDamageBonus}% PvP Damage ";
            if (critChanceBonus > 0) desc += $"+{critChanceBonus}% Crit Chance ";
            return desc.Trim();
        }
    }

    /// <summary>
    /// PvP Title Definition
    /// </summary>
    [CreateAssetMenu(fileName = "PvPTitle", menuName = "Dark Legend/PvP/Title")]
    public class PvPTitle : ScriptableObject
    {
        public string titleName;
        public string description;
        public TitleRequirement requirement;
        public StatBonus bonus;
        public Sprite icon;
        public Color titleColor = Color.white;
        
        /// <summary>
        /// Check if player meets requirement
        /// Kiểm tra người chơi đạt yêu cầu
        /// </summary>
        public bool MeetsRequirement(PlayerPvPStats stats)
        {
            switch (requirement.type)
            {
                case TitleRequirement.RequirementType.PvPKills:
                    return stats.totalKills >= requirement.value;
                case TitleRequirement.RequirementType.ArenaWins:
                    return stats.arenaWins >= requirement.value;
                case TitleRequirement.RequirementType.DuelWins:
                    return stats.duelWins >= requirement.value;
                case TitleRequirement.RequirementType.ReachRank:
                    return stats.highestRank >= requirement.rankTier;
                case TitleRequirement.RequirementType.WinStreak:
                    return stats.maxWinStreak >= requirement.value;
                case TitleRequirement.RequirementType.GuildWars:
                    return stats.guildWarWins >= requirement.value;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    /// Player PvP Statistics
    /// </summary>
    [Serializable]
    public class PlayerPvPStats
    {
        public int totalKills = 0;
        public int arenaWins = 0;
        public int duelWins = 0;
        public RankTier highestRank = RankTier.Bronze_III;
        public int maxWinStreak = 0;
        public int guildWarWins = 0;
    }
}
