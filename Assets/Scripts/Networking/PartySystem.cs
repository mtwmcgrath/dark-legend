using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Hệ thống party: tạo, invite, kick, share EXP / Party system: create, invite, kick, share EXP
    /// </summary>
    public class PartySystem : MonoBehaviourPunCallbacks
    {
        public static PartySystem Instance { get; private set; }

        [Header("Party Settings")]
        [SerializeField] private int maxPartySize = 4;
        [SerializeField] private float expShareRadius = 50f;
        [SerializeField] private float expShareBonus = 0.1f; // 10% bonus exp trong party / 10% bonus exp in party

        // Event codes
        private const byte PARTY_INVITE_EVENT = 10;
        private const byte PARTY_INVITE_RESPONSE_EVENT = 11;
        private const byte PARTY_KICK_EVENT = 12;

        // Party data
        private List<Player> partyMembers = new List<Player>();
        private Player partyLeader;
        private bool isInParty = false;

        // Delegates
        public delegate void OnPartyCreated();
        public event OnPartyCreated PartyCreated;

        public delegate void OnPartyJoined(List<Player> members);
        public event OnPartyJoined PartyJoined;

        public delegate void OnPartyLeft();
        public event OnPartyLeft PartyLeft;

        public delegate void OnPartyMemberJoined(Player player);
        public event OnPartyMemberJoined PartyMemberJoined;

        public delegate void OnPartyMemberLeft(Player player);
        public event OnPartyMemberLeft PartyMemberLeft;

        public delegate void OnPartyInviteReceived(string senderName);
        public event OnPartyInviteReceived PartyInviteReceived;

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

        #region Create & Join Party

        /// <summary>
        /// Tạo party mới / Create new party
        /// </summary>
        public void CreateParty()
        {
            if (isInParty)
            {
                Debug.LogWarning("[PartySystem] Already in a party");
                return;
            }

            partyMembers.Clear();
            partyMembers.Add(PhotonNetwork.LocalPlayer);
            partyLeader = PhotonNetwork.LocalPlayer;
            isInParty = true;

            Debug.Log("[PartySystem] Party created");
            PartyCreated?.Invoke();
        }

        /// <summary>
        /// Join party / Tham gia party
        /// </summary>
        private void JoinParty(Player leader, List<Player> members)
        {
            if (isInParty)
            {
                LeaveParty();
            }

            partyLeader = leader;
            partyMembers = new List<Player>(members);
            isInParty = true;

            Debug.Log($"[PartySystem] Joined party. Leader: {partyLeader.NickName}");
            PartyJoined?.Invoke(partyMembers);
        }

        #endregion

        #region Invite System

        /// <summary>
        /// Mời player vào party / Invite player to party
        /// </summary>
        public void InvitePlayer(string playerName)
        {
            if (!isInParty)
            {
                Debug.LogWarning("[PartySystem] Not in a party. Create one first.");
                return;
            }

            if (!IsPartyLeader())
            {
                Debug.LogWarning("[PartySystem] Only party leader can invite");
                return;
            }

            if (partyMembers.Count >= maxPartySize)
            {
                Debug.LogWarning("[PartySystem] Party is full");
                return;
            }

            // Tìm player / Find player
            Player targetPlayer = FindPlayerByName(playerName);
            if (targetPlayer != null)
            {
                // Gửi invite / Send invite
                object[] content = new object[] 
                { 
                    PhotonNetwork.NickName,
                    SerializePartyMembers()
                };
                
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { targetPlayer.ActorNumber } 
                };
                
                PhotonNetwork.RaiseEvent(PARTY_INVITE_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
                
                Debug.Log($"[PartySystem] Sent party invite to {playerName}");
            }
            else
            {
                Debug.LogWarning($"[PartySystem] Player {playerName} not found");
            }
        }

        /// <summary>
        /// Chấp nhận lời mời party / Accept party invite
        /// </summary>
        public void AcceptPartyInvite(string leaderName)
        {
            Player leader = FindPlayerByName(leaderName);
            if (leader != null)
            {
                object[] content = new object[] { PhotonNetwork.NickName, true };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { leader.ActorNumber } 
                };
                
                PhotonNetwork.RaiseEvent(PARTY_INVITE_RESPONSE_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
                
                Debug.Log($"[PartySystem] Accepted party invite from {leaderName}");
            }
        }

        /// <summary>
        /// Từ chối lời mời party / Decline party invite
        /// </summary>
        public void DeclinePartyInvite(string leaderName)
        {
            Player leader = FindPlayerByName(leaderName);
            if (leader != null)
            {
                object[] content = new object[] { PhotonNetwork.NickName, false };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { leader.ActorNumber } 
                };
                
                PhotonNetwork.RaiseEvent(PARTY_INVITE_RESPONSE_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
                
                Debug.Log($"[PartySystem] Declined party invite from {leaderName}");
            }
        }

        #endregion

        #region Leave & Kick

        /// <summary>
        /// Rời party / Leave party
        /// </summary>
        public void LeaveParty()
        {
            if (!isInParty) return;

            // Thông báo cho party members / Notify party members
            if (partyMembers.Count > 1)
            {
                int[] targetActors = GetPartyMemberActors();
                object[] content = new object[] { PhotonNetwork.NickName };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = targetActors };
                PhotonNetwork.RaiseEvent(PARTY_KICK_EVENT, content, raiseEventOptions, SendOptions.SendReliable);
            }

            partyMembers.Clear();
            partyLeader = null;
            isInParty = false;

            Debug.Log("[PartySystem] Left party");
            PartyLeft?.Invoke();
        }

        /// <summary>
        /// Kick member khỏi party / Kick member from party
        /// </summary>
        public void KickMember(string playerName)
        {
            if (!IsPartyLeader())
            {
                Debug.LogWarning("[PartySystem] Only party leader can kick members");
                return;
            }

            Player targetPlayer = partyMembers.Find(p => p.NickName == playerName);
            if (targetPlayer != null && targetPlayer != partyLeader)
            {
                partyMembers.Remove(targetPlayer);

                // Gửi kick event / Send kick event
                object[] content = new object[] { playerName };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { targetPlayer.ActorNumber } 
                };
                PhotonNetwork.RaiseEvent(PARTY_KICK_EVENT, content, raiseEventOptions, SendOptions.SendReliable);

                Debug.Log($"[PartySystem] Kicked {playerName} from party");
                PartyMemberLeft?.Invoke(targetPlayer);
            }
        }

        #endregion

        #region Event Handling

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == PARTY_INVITE_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string leaderName = (string)data[0];
                
                Debug.Log($"[PartySystem] Received party invite from {leaderName}");
                PartyInviteReceived?.Invoke(leaderName);
            }
            else if (eventCode == PARTY_INVITE_RESPONSE_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string playerName = (string)data[0];
                bool accepted = (bool)data[1];

                if (accepted)
                {
                    Player player = FindPlayerByName(playerName);
                    if (player != null && !partyMembers.Contains(player))
                    {
                        partyMembers.Add(player);
                        Debug.Log($"[PartySystem] {playerName} joined the party");
                        PartyMemberJoined?.Invoke(player);
                    }
                }
                else
                {
                    Debug.Log($"[PartySystem] {playerName} declined party invite");
                }
            }
            else if (eventCode == PARTY_KICK_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string playerName = (string)data[0];

                if (playerName == PhotonNetwork.NickName)
                {
                    // Bị kick khỏi party / Kicked from party
                    partyMembers.Clear();
                    partyLeader = null;
                    isInParty = false;
                    
                    Debug.Log("[PartySystem] You were kicked from the party");
                    PartyLeft?.Invoke();
                }
                else
                {
                    // Member khác bị kick / Another member was kicked
                    Player player = partyMembers.Find(p => p.NickName == playerName);
                    if (player != null)
                    {
                        partyMembers.Remove(player);
                        Debug.Log($"[PartySystem] {playerName} left the party");
                        PartyMemberLeft?.Invoke(player);
                    }
                }
            }
        }

        #endregion

        #region EXP Sharing

        /// <summary>
        /// Tính EXP share cho party / Calculate shared EXP for party
        /// </summary>
        public int CalculateSharedEXP(int baseEXP, Vector3 killPosition)
        {
            if (!isInParty) return baseEXP;

            // Đếm số member trong bán kính / Count members within radius
            int nearbyMembers = 0;
            foreach (Player member in partyMembers)
            {
                // TODO: Get member position from network
                // Vector3 memberPosition = GetPlayerPosition(member);
                // if (Vector3.Distance(killPosition, memberPosition) <= expShareRadius)
                // {
                //     nearbyMembers++;
                // }
                nearbyMembers++; // Temporary: count all members
            }

            if (nearbyMembers <= 1) return baseEXP;

            // Chia EXP + bonus / Divide EXP + bonus
            float sharedEXP = baseEXP / (float)nearbyMembers;
            float bonusEXP = sharedEXP * expShareBonus;
            
            return Mathf.RoundToInt(sharedEXP + bonusEXP);
        }

        /// <summary>
        /// Phân phối EXP cho party members / Distribute EXP to party members
        /// </summary>
        public void DistributeEXP(int totalEXP, Vector3 killPosition)
        {
            if (!isInParty || !IsPartyLeader()) return;

            int expPerMember = CalculateSharedEXP(totalEXP, killPosition);

            // Gửi EXP đến các members / Send EXP to members
            foreach (Player member in partyMembers)
            {
                // TODO: Implement EXP distribution RPC
                Debug.Log($"[PartySystem] Distributed {expPerMember} EXP to {member.NickName}");
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Kiểm tra có phải party leader không / Check if is party leader
        /// </summary>
        public bool IsPartyLeader()
        {
            return isInParty && partyLeader == PhotonNetwork.LocalPlayer;
        }

        /// <summary>
        /// Lấy danh sách party members / Get party members list
        /// </summary>
        public List<Player> GetPartyMembers()
        {
            return new List<Player>(partyMembers);
        }

        /// <summary>
        /// Kiểm tra có trong party không / Check if in party
        /// </summary>
        public bool IsInParty()
        {
            return isInParty;
        }

        /// <summary>
        /// Lấy số lượng members / Get member count
        /// </summary>
        public int GetPartySize()
        {
            return partyMembers.Count;
        }

        /// <summary>
        /// Lấy party leader / Get party leader
        /// </summary>
        public Player GetPartyLeader()
        {
            return partyLeader;
        }

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
        /// Lấy actor numbers của party members / Get actor numbers of party members
        /// </summary>
        private int[] GetPartyMemberActors()
        {
            int[] actors = new int[partyMembers.Count];
            for (int i = 0; i < partyMembers.Count; i++)
            {
                actors[i] = partyMembers[i].ActorNumber;
            }
            return actors;
        }

        /// <summary>
        /// Serialize party members để gửi qua network / Serialize party members to send over network
        /// </summary>
        private string SerializePartyMembers()
        {
            string[] memberNames = new string[partyMembers.Count];
            for (int i = 0; i < partyMembers.Count; i++)
            {
                memberNames[i] = partyMembers[i].NickName;
            }
            return string.Join(",", memberNames);
        }

        #endregion

        #region Photon Callbacks

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            // Xóa player khỏi party nếu họ disconnect / Remove player from party if they disconnect
            if (partyMembers.Contains(otherPlayer))
            {
                partyMembers.Remove(otherPlayer);
                Debug.Log($"[PartySystem] {otherPlayer.NickName} disconnected from party");
                PartyMemberLeft?.Invoke(otherPlayer);

                // Nếu leader disconnect, chuyển quyền / If leader disconnects, transfer leadership
                if (otherPlayer == partyLeader && partyMembers.Count > 0)
                {
                    partyLeader = partyMembers[0];
                    Debug.Log($"[PartySystem] New party leader: {partyLeader.NickName}");
                }
            }
        }

        #endregion
    }
}
