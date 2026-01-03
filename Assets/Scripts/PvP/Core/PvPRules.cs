using UnityEngine;
using System;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP rules configuration - Luật chơi PvP
    /// </summary>
    [Serializable]
    public class PvPRules
    {
        [Header("Combat Rules")]
        public bool allowPotions = true;
        public bool allowSkills = true;
        public bool allowPets = false;
        public bool friendlyFire = false;
        
        [Header("Death Penalties")]
        public bool loseExpOnDeath = false;
        public float expLossPercentage = 0.0f;
        public bool dropItemsOnDeath = false;
        public float itemDropChance = 0.0f;
        
        [Header("Zone Rules")]
        public bool isPvPZone = false;
        public bool isSafeZone = false;
        public bool autoFlag = false; // Auto enable PvP in this zone
        
        [Header("Time Limits")]
        public int timeLimit = 0; // 0 = no limit
        public float respawnTime = 5f;
        
        [Header("Score Settings")]
        public int pointsPerKill = 1;
        public int pointsPerAssist = 0;
        public int pointsPerObjective = 5;
    }
}
