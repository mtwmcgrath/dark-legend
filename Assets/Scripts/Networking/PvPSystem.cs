using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Hệ thống PvP: duel, PvP zones, PK system / PvP system: duel, PvP zones, PK system
    /// </summary>
    public class PvPSystem : MonoBehaviourPunCallbacks
    {
        public static PvPSystem Instance { get; private set; }

        [Header("PvP Settings")]
        [SerializeField] private bool pvpEnabled = false;
        [SerializeField] private int pkPenaltyTime = 300; // 5 phút / 5 minutes
        [SerializeField] private int pkKillPenalty = 10; // % exp loss

        // Event codes
        private const byte DUEL_REQUEST_EVENT = 20;
        private const byte DUEL_RESPONSE_EVENT = 21;
        private const byte DUEL_END_EVENT = 22;

        // PvP state
        private bool isInDuel = false;
        private Player duelOpponent;
        private int pkCount = 0;
        private float pkPenaltyEndTime = 0f;

        // PvP zones
        private List<string> pvpZones = new List<string> { "Arena", "Battlefield", "DuelZone" };
        private string currentZone = "";

        // Kill tracking
        private Dictionary<int, int> playerKills = new Dictionary<int, int>();

        // Delegates
        public delegate void OnPvPModeChanged(bool enabled);
        public event OnPvPModeChanged PvPModeChanged;

        public delegate void OnDuelRequested(string requesterName);
        public event OnDuelRequested DuelRequested;

        public delegate void OnDuelStarted(Player opponent);
        public event OnDuelStarted DuelStarted;

        public delegate void OnDuelEnded(Player winner, Player loser);
        public event OnDuelEnded DuelEnded;

        public delegate void OnPlayerKilled(Player killer, Player victim);
        public event OnPlayerKilled PlayerKilled;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        private void Update()
        {
            // Kiểm tra PK penalty timeout / Check PK penalty timeout
            if (pkPenaltyEndTime > 0 && Time.time >= pkPenaltyEndTime)
            {
                pkPenaltyEndTime = 0f;
                Debug.Log("[PvPSystem] PK penalty expired");
            }
        }

        #region PvP Mode

        /// <summary>
        /// Toggle PvP mode / Bật/tắt chế độ PvP
        /// </summary>
        public void TogglePvPMode()
        {
            pvpEnabled = !pvpEnabled;
            Debug.Log($"[PvPSystem] PvP mode: {(pvpEnabled ? "ON" : "OFF")}");
            PvPModeChanged?.Invoke(pvpEnabled);
        }

        /// <summary>
        /// Đặt PvP mode / Set PvP mode
        /// </summary>
        public void SetPvPMode(bool enabled)
        {
            if (pvpEnabled != enabled)
            {
                pvpEnabled = enabled;
                Debug.Log($"[PvPSystem] PvP mode: {(pvpEnabled ? "ON" : "OFF")}");
                PvPModeChanged?.Invoke(pvpEnabled);
            }
        }

        /// <summary>
        /// Kiểm tra PvP có bật không / Check if PvP is enabled
        /// </summary>
        public bool IsPvPEnabled()
        {
            return pvpEnabled;
        }

        /// <summary>
        /// Kiểm tra có thể tấn công player không / Check if can attack player
        /// </summary>
        public bool CanAttackPlayer(Player target)
        {
            // Không thể tấn công chính mình / Can't attack self
            if (target == PhotonNetwork.LocalPlayer) return false;

            // Trong duel, chỉ có thể tấn công opponent / In duel, can only attack opponent
            if (isInDuel)
            {
                return target == duelOpponent;
            }

            // Kiểm tra PvP zone / Check PvP zone
            if (IsInPvPZone())
            {
                return true;
            }

            // Kiểm tra room PvP settings / Check room PvP settings
            if (RoomManager.Instance != null && RoomManager.Instance.IsPvPEnabled())
            {
                return pvpEnabled;
            }

            return false;
        }

        #endregion

        #region Duel System

        /// <summary>
        /// Gửi lời mời duel / Send duel request
        /// </summary>
        public void RequestDuel(string targetPlayerName)
        {
            if (isInDuel)
            {
                Debug.LogWarning("[PvPSystem] Already in a duel");
                return;
            }

            Player targetPlayer = FindPlayerByName(targetPlayerName);
            if (targetPlayer != null)
            {
                object[] content = new object[] { PhotonNetwork.NickName };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { targetPlayer.ActorNumber } 
                };
                
                PhotonNetwork.RaiseEvent(DUEL_REQUEST_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
                
                Debug.Log($"[PvPSystem] Sent duel request to {targetPlayerName}");
            }
            else
            {
                Debug.LogWarning($"[PvPSystem] Player {targetPlayerName} not found");
            }
        }

        /// <summary>
        /// Chấp nhận duel / Accept duel
        /// </summary>
        public void AcceptDuel(string requesterName)
        {
            Player requester = FindPlayerByName(requesterName);
            if (requester != null)
            {
                object[] content = new object[] { PhotonNetwork.NickName, true };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { requester.ActorNumber } 
                };
                
                PhotonNetwork.RaiseEvent(DUEL_RESPONSE_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
                
                // Bắt đầu duel / Start duel
                StartDuel(requester);
                
                Debug.Log($"[PvPSystem] Accepted duel with {requesterName}");
            }
        }

        /// <summary>
        /// Từ chối duel / Decline duel
        /// </summary>
        public void DeclineDuel(string requesterName)
        {
            Player requester = FindPlayerByName(requesterName);
            if (requester != null)
            {
                object[] content = new object[] { PhotonNetwork.NickName, false };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { requester.ActorNumber } 
                };
                
                PhotonNetwork.RaiseEvent(DUEL_RESPONSE_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
                
                Debug.Log($"[PvPSystem] Declined duel with {requesterName}");
            }
        }

        /// <summary>
        /// Bắt đầu duel / Start duel
        /// </summary>
        private void StartDuel(Player opponent)
        {
            isInDuel = true;
            duelOpponent = opponent;
            
            Debug.Log($"[PvPSystem] Duel started with {opponent.NickName}");
            DuelStarted?.Invoke(opponent);
        }

        /// <summary>
        /// Kết thúc duel / End duel
        /// </summary>
        public void EndDuel(Player winner, Player loser)
        {
            if (!isInDuel) return;

            // Gửi event kết thúc duel / Send duel end event
            int[] targetActors = new int[] { winner.ActorNumber, loser.ActorNumber };
            object[] content = new object[] { winner.NickName, loser.NickName };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = targetActors };
            PhotonNetwork.RaiseEvent(DUEL_END_EVENT, content, raiseEventOptions, SendOptions.SendReliable);

            isInDuel = false;
            duelOpponent = null;

            Debug.Log($"[PvPSystem] Duel ended. Winner: {winner.NickName}, Loser: {loser.NickName}");
            DuelEnded?.Invoke(winner, loser);
        }

        #endregion

        #region PK System

        /// <summary>
        /// Xử lý khi kill player / Handle player kill
        /// </summary>
        public void OnPlayerKill(Player killer, Player victim)
        {
            // Không tính PK trong duel / Don't count PK in duel
            if (isInDuel) return;

            // Không tính PK trong PvP zones / Don't count PK in PvP zones
            if (IsInPvPZone()) return;

            // Tăng PK count / Increase PK count
            if (killer == PhotonNetwork.LocalPlayer)
            {
                pkCount++;
                pkPenaltyEndTime = Time.time + pkPenaltyTime;
                
                Debug.LogWarning($"[PvPSystem] PK count increased: {pkCount}. Penalty time: {pkPenaltyTime}s");
            }

            // Track kill / Theo dõi kill
            if (!playerKills.ContainsKey(killer.ActorNumber))
            {
                playerKills[killer.ActorNumber] = 0;
            }
            playerKills[killer.ActorNumber]++;

            Debug.Log($"[PvPSystem] Player {killer.NickName} killed {victim.NickName}");
            PlayerKilled?.Invoke(killer, victim);
        }

        /// <summary>
        /// Lấy PK count / Get PK count
        /// </summary>
        public int GetPKCount()
        {
            return pkCount;
        }

        /// <summary>
        /// Kiểm tra có đang bị PK penalty không / Check if under PK penalty
        /// </summary>
        public bool IsUnderPKPenalty()
        {
            return pkPenaltyEndTime > 0 && Time.time < pkPenaltyEndTime;
        }

        /// <summary>
        /// Lấy thời gian PK penalty còn lại / Get remaining PK penalty time
        /// </summary>
        public float GetRemainingPKPenaltyTime()
        {
            if (!IsUnderPKPenalty()) return 0f;
            return pkPenaltyEndTime - Time.time;
        }

        /// <summary>
        /// Reset PK count / Đặt lại PK count
        /// </summary>
        public void ResetPKCount()
        {
            pkCount = 0;
            pkPenaltyEndTime = 0f;
        }

        #endregion

        #region PvP Zones

        /// <summary>
        /// Đăng ký PvP zone / Register PvP zone
        /// </summary>
        public void RegisterPvPZone(string zoneName)
        {
            if (!pvpZones.Contains(zoneName))
            {
                pvpZones.Add(zoneName);
                Debug.Log($"[PvPSystem] Registered PvP zone: {zoneName}");
            }
        }

        /// <summary>
        /// Vào PvP zone / Enter PvP zone
        /// </summary>
        public void EnterPvPZone(string zoneName)
        {
            currentZone = zoneName;
            Debug.Log($"[PvPSystem] Entered PvP zone: {zoneName}");
        }

        /// <summary>
        /// Rời PvP zone / Leave PvP zone
        /// </summary>
        public void LeavePvPZone()
        {
            Debug.Log($"[PvPSystem] Left PvP zone: {currentZone}");
            currentZone = "";
        }

        /// <summary>
        /// Kiểm tra có trong PvP zone không / Check if in PvP zone
        /// </summary>
        public bool IsInPvPZone()
        {
            return !string.IsNullOrEmpty(currentZone) && pvpZones.Contains(currentZone);
        }

        /// <summary>
        /// Lấy tên zone hiện tại / Get current zone name
        /// </summary>
        public string GetCurrentZone()
        {
            return currentZone;
        }

        #endregion

        #region Kill Tracking

        /// <summary>
        /// Lấy số lượng kills của player / Get player kill count
        /// </summary>
        public int GetPlayerKills(Player player)
        {
            if (playerKills.ContainsKey(player.ActorNumber))
            {
                return playerKills[player.ActorNumber];
            }
            return 0;
        }

        /// <summary>
        /// Lấy leaderboard kills / Get kills leaderboard
        /// </summary>
        public Dictionary<int, int> GetKillsLeaderboard()
        {
            return new Dictionary<int, int>(playerKills);
        }

        /// <summary>
        /// Reset kills / Đặt lại kills
        /// </summary>
        public void ResetKills()
        {
            playerKills.Clear();
        }

        #endregion

        #region Event Handling

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == DUEL_REQUEST_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string requesterName = (string)data[0];
                
                Debug.Log($"[PvPSystem] Received duel request from {requesterName}");
                DuelRequested?.Invoke(requesterName);
            }
            else if (eventCode == DUEL_RESPONSE_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string playerName = (string)data[0];
                bool accepted = (bool)data[1];

                if (accepted)
                {
                    Player player = FindPlayerByName(playerName);
                    if (player != null)
                    {
                        StartDuel(player);
                    }
                }
                else
                {
                    Debug.Log($"[PvPSystem] {playerName} declined duel request");
                }
            }
            else if (eventCode == DUEL_END_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string winnerName = (string)data[0];
                string loserName = (string)data[1];

                isInDuel = false;
                duelOpponent = null;

                Debug.Log($"[PvPSystem] Duel ended. Winner: {winnerName}, Loser: {loserName}");
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Tìm player theo tên / Find player by name
        /// </summary>
        private Player FindPlayerByName(string playerName)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == playerName)
                {
                    return player;
                }
            }
            return null;
        }

        /// <summary>
        /// Kiểm tra có đang trong duel không / Check if in duel
        /// </summary>
        public bool IsInDuel()
        {
            return isInDuel;
        }

        /// <summary>
        /// Lấy duel opponent / Get duel opponent
        /// </summary>
        public Player GetDuelOpponent()
        {
            return duelOpponent;
        }

        #endregion
    }
}
