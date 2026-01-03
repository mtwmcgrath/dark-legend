using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// ScriptableObject for PvP configuration
    /// Cấu hình dữ liệu PvP
    /// </summary>
    [CreateAssetMenu(fileName = "PvPData", menuName = "Dark Legend/PvP/PvP Data")]
    public class PvPData : ScriptableObject
    {
        [Header("General Settings")]
        public bool pvpEnabled = true;
        public float globalPvPDamageMultiplier = 1.0f;
        
        [Header("Duel Settings")]
        public int duelTimeLimit = 300; // 5 minutes
        public bool duelAllowPotions = true;
        public bool duelAllowSkills = true;
        public float duelRespawnTime = 3f;
        
        [Header("Arena Settings")]
        public int arenaELORange = 200;
        public int arenaMaxWaitTime = 120; // 2 minutes
        public int arenaDefaultRating = 1200;
        
        [Header("Battleground Settings")]
        public int teamDeathmatchSize = 10;
        public int teamDeathmatchKillsToWin = 100;
        public int teamDeathmatchTimeLimit = 900; // 15 minutes
        
        [Header("PK System Settings")]
        public float pkDecayRate = 1f; // Per hour
        public int pkCountForMurderer = 1;
        public int pkCountForOutlaw = 10;
        public int autoBountyPerPK = 10000; // Zen
        
        [Header("Ranking Settings")]
        public int seasonDurationDays = 90; // 3 months
        public int challengerMinPlayers = 100; // Top 100
    }
}
