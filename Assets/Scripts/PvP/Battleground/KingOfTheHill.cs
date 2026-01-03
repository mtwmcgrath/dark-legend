using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// King of the Hill Mode - Chế độ giữ đồi
    /// </summary>
    public class KingOfTheHill : BattlegroundMode
    {
        [Header("KOTH Settings")]
        public int pointsToWin = 1000;
        public int pointsPerSecond = 1;           // While holding hill
        public float captureTime = 10f;           // Seconds to capture
        public float hillRotationInterval = 120f; // 2 minutes
        
        [Header("Hill Locations")]
        public Transform[] hillLocations;
        public GameObject hillMarker;
        
        private int currentHillIndex = 0;
        private float hillCaptureProgress = 0f;
        private int controllingTeam = 0;          // 0 = contested, 1 = team1, 2 = team2
        private float lastScoreTime = 0f;
        private float nextHillRotation = 0f;
        
        // Players on hill
        private List<GameObject> team1OnHill = new List<GameObject>();
        private List<GameObject> team2OnHill = new List<GameObject>();
        
        private void Awake()
        {
            modeName = "King of the Hill";
            teamSize = 6;
            timeLimit = 900; // 15 minutes
        }
        
        public override void StartMatch()
        {
            base.StartMatch();
            
            // Activate first hill
            ActivateHill(0);
            nextHillRotation = Time.time + hillRotationInterval;
        }
        
        /// <summary>
        /// Player enters hill zone
        /// Người chơi vào vùng đồi
        /// </summary>
        public void OnPlayerEnterHill(GameObject player)
        {
            if (team1.Contains(player))
            {
                if (!team1OnHill.Contains(player))
                    team1OnHill.Add(player);
            }
            else if (team2.Contains(player))
            {
                if (!team2OnHill.Contains(player))
                    team2OnHill.Add(player);
            }
            
            UpdateHillStatus();
        }
        
        /// <summary>
        /// Player exits hill zone
        /// Người chơi rời vùng đồi
        /// </summary>
        public void OnPlayerExitHill(GameObject player)
        {
            team1OnHill.Remove(player);
            team2OnHill.Remove(player);
            
            UpdateHillStatus();
        }
        
        /// <summary>
        /// Update hill control status
        /// Cập nhật trạng thái kiểm soát đồi
        /// </summary>
        private void UpdateHillStatus()
        {
            int team1Count = team1OnHill.Count;
            int team2Count = team2OnHill.Count;
            
            if (team1Count > 0 && team2Count > 0)
            {
                // Contested
                controllingTeam = 0;
                hillCaptureProgress = 0f;
            }
            else if (team1Count > 0)
            {
                // Team 1 capturing/controlling
                if (controllingTeam != 1)
                {
                    hillCaptureProgress += Time.deltaTime;
                    if (hillCaptureProgress >= captureTime)
                    {
                        controllingTeam = 1;
                        hillCaptureProgress = captureTime;
                        Debug.Log("Team 1 captured the hill!");
                    }
                }
            }
            else if (team2Count > 0)
            {
                // Team 2 capturing/controlling
                if (controllingTeam != 2)
                {
                    hillCaptureProgress += Time.deltaTime;
                    if (hillCaptureProgress >= captureTime)
                    {
                        controllingTeam = 2;
                        hillCaptureProgress = captureTime;
                        Debug.Log("Team 2 captured the hill!");
                    }
                }
            }
            else
            {
                // No one on hill
                hillCaptureProgress = Mathf.Max(0, hillCaptureProgress - Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Award points to controlling team
        /// Trao điểm cho đội kiểm soát
        /// </summary>
        private void AwardHillPoints()
        {
            if (controllingTeam == 0) return; // Contested
            if (Time.time - lastScoreTime < 1f) return; // Award once per second
            
            UpdateScore(controllingTeam, pointsPerSecond);
            lastScoreTime = Time.time;
        }
        
        /// <summary>
        /// Activate hill at index
        /// Kích hoạt đồi tại vị trí
        /// </summary>
        private void ActivateHill(int index)
        {
            if (hillLocations == null || hillLocations.Length == 0) return;
            
            currentHillIndex = index % hillLocations.Length;
            
            // Move hill marker
            if (hillMarker != null)
            {
                hillMarker.transform.position = hillLocations[currentHillIndex].position;
                hillMarker.SetActive(true);
            }
            
            // Reset hill state
            controllingTeam = 0;
            hillCaptureProgress = 0f;
            team1OnHill.Clear();
            team2OnHill.Clear();
            
            Debug.Log($"Hill activated at location {currentHillIndex}");
        }
        
        /// <summary>
        /// Rotate to next hill
        /// Chuyển sang đồi tiếp theo
        /// </summary>
        private void RotateHill()
        {
            ActivateHill(currentHillIndex + 1);
            nextHillRotation = Time.time + hillRotationInterval;
        }
        
        protected override bool CheckWinCondition()
        {
            return team1Score >= pointsToWin || team2Score >= pointsToWin;
        }
        
        protected override void Update()
        {
            base.Update();
            
            if (state == MatchState.InProgress)
            {
                UpdateHillStatus();
                AwardHillPoints();
                
                // Check for hill rotation
                if (Time.time >= nextHillRotation)
                {
                    RotateHill();
                }
            }
        }
        
        /// <summary>
        /// Get capture progress percentage
        /// Lấy % tiến trình chiếm đồi
        /// </summary>
        public float GetCaptureProgress()
        {
            return hillCaptureProgress / captureTime;
        }
    }
}
