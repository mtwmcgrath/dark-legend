using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena Season - Mùa giải đấu trường
    /// </summary>
    [Serializable]
    public class ArenaSeason
    {
        public int seasonNumber;
        public DateTime startDate;
        public DateTime endDate;
        public Dictionary<string, SeasonReward> rewards = new Dictionary<string, SeasonReward>();
        
        public ArenaSeason(int seasonNumber, int durationDays = 90)
        {
            this.seasonNumber = seasonNumber;
            this.startDate = DateTime.Now;
            this.endDate = startDate.AddDays(durationDays);
        }
        
        /// <summary>
        /// Check if season is active
        /// Kiểm tra mùa giải có đang hoạt động không
        /// </summary>
        public bool IsActive()
        {
            DateTime now = DateTime.Now;
            return now >= startDate && now <= endDate;
        }
        
        /// <summary>
        /// Get days remaining in season
        /// Lấy số ngày còn lại trong mùa
        /// </summary>
        public int GetDaysRemaining()
        {
            if (!IsActive()) return 0;
            return (int)(endDate - DateTime.Now).TotalDays;
        }
    }
}
