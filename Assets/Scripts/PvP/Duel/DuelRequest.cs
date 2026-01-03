using UnityEngine;
using System;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Duel settings - Cài đặt đấu tay đôi
    /// </summary>
    [Serializable]
    public class DuelSettings
    {
        public int timeLimit = 300;           // 5 minutes
        public bool allowPotions = true;
        public bool allowSkills = true;
        public int betAmount = 0;             // Zen cược
        public DuelType type = DuelType.Normal;
    }

    /// <summary>
    /// Active duel instance - Trận đấu đang diễn ra
    /// </summary>
    public class ActiveDuel
    {
        public GameObject challenger;
        public GameObject target;
        public DuelSettings settings;
        public Vector3 challengerOriginalPosition;
        public Vector3 targetOriginalPosition;
        public float startTime;
        public float endTime;
        
        public bool IsExpired => Time.time > endTime;
        public float TimeRemaining => Mathf.Max(0, endTime - Time.time);
    }

    /// <summary>
    /// Duel request - Yêu cầu đấu tay đôi
    /// </summary>
    public class DuelRequest
    {
        public GameObject challenger;
        public GameObject target;
        public DuelSettings settings;
        public float requestTime;
        public float expiryTime;
        
        public bool IsExpired => Time.time > expiryTime;
        
        public DuelRequest(GameObject challenger, GameObject target, DuelSettings settings)
        {
            this.challenger = challenger;
            this.target = target;
            this.settings = settings;
            this.requestTime = Time.time;
            this.expiryTime = Time.time + 30f; // 30 seconds to accept
        }
    }
}
