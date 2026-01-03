using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Hệ thống chat: global, room, party, whisper / Chat system: global, room, party, whisper
    /// </summary>
    public class ChatSystem : MonoBehaviourPunCallbacks
    {
        public static ChatSystem Instance { get; private set; }

        // Event codes cho custom events / Event codes for custom events
        private const byte GLOBAL_CHAT_EVENT = 1;
        private const byte WHISPER_CHAT_EVENT = 2;
        private const byte PARTY_CHAT_EVENT = 3;

        // Chat channels
        public enum ChatChannel
        {
            Global,     // Tất cả người chơi / All players
            Room,       // Trong room / In room
            Party,      // Trong party / In party
            Whisper     // Chat riêng / Private chat
        }

        // Chat message data
        public class ChatMessage
        {
            public string senderName;
            public string message;
            public ChatChannel channel;
            public long timestamp;

            public ChatMessage(string sender, string msg, ChatChannel ch)
            {
                senderName = sender;
                message = msg;
                channel = ch;
                timestamp = System.DateTime.Now.Ticks;
            }
        }

        // Chat history
        private List<ChatMessage> chatHistory = new List<ChatMessage>();
        private const int maxChatHistory = 100;

        // Delegates
        public delegate void OnMessageReceived(ChatMessage message);
        public event OnMessageReceived MessageReceived;

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
            // Đăng ký nhận custom events / Register to receive custom events
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        private void OnDisable()
        {
            // Hủy đăng ký / Unregister
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
        }

        #region Send Messages

        /// <summary>
        /// Gửi message đến global chat / Send message to global chat
        /// </summary>
        public void SendGlobalMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            object[] content = new object[] { PhotonNetwork.NickName, message };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(GLOBAL_CHAT_EVENT, content, raiseEventOptions, SendOptions.SendReliable);

            Debug.Log($"[ChatSystem] Global: {PhotonNetwork.NickName}: {message}");
        }

        /// <summary>
        /// Gửi message đến room chat / Send message to room chat
        /// </summary>
        public void SendRoomMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            if (!PhotonNetwork.InRoom) return;

            photonView.RPC("RPC_RoomMessage", RpcTarget.All, PhotonNetwork.NickName, message);
        }

        /// <summary>
        /// Gửi message đến party chat / Send message to party chat
        /// </summary>
        public void SendPartyMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            // Lấy danh sách party members từ PartySystem
            // Get party members list from PartySystem
            if (PartySystem.Instance != null)
            {
                List<Player> partyMembers = PartySystem.Instance.GetPartyMembers();
                if (partyMembers.Count > 0)
                {
                    int[] targetActors = new int[partyMembers.Count];
                    for (int i = 0; i < partyMembers.Count; i++)
                    {
                        targetActors[i] = partyMembers[i].ActorNumber;
                    }

                    object[] content = new object[] { PhotonNetwork.NickName, message };
                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { TargetActors = targetActors };
                    PhotonNetwork.RaiseEvent(PARTY_CHAT_EVENT, content, raiseEventOptions, SendOptions.SendReliable);

                    Debug.Log($"[ChatSystem] Party: {PhotonNetwork.NickName}: {message}");
                }
            }
        }

        /// <summary>
        /// Gửi whisper (chat riêng) / Send whisper (private chat)
        /// </summary>
        public void SendWhisper(string targetPlayerName, string message)
        {
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(targetPlayerName)) return;

            // Tìm player theo tên / Find player by name
            Player targetPlayer = null;
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == targetPlayerName)
                {
                    targetPlayer = player;
                    break;
                }
            }

            if (targetPlayer != null)
            {
                object[] content = new object[] { PhotonNetwork.NickName, message };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions 
                { 
                    TargetActors = new int[] { targetPlayer.ActorNumber } 
                };
                PhotonNetwork.RaiseEvent(WHISPER_CHAT_EVENT, content, raiseEventOptions, SendOptions.SendReliable);

                Debug.Log($"[ChatSystem] Whisper to {targetPlayerName}: {message}");

                // Thêm vào chat history cho người gửi / Add to chat history for sender
                AddMessageToHistory(PhotonNetwork.NickName, $"[To {targetPlayerName}] {message}", ChatChannel.Whisper);
            }
            else
            {
                Debug.LogWarning($"[ChatSystem] Player {targetPlayerName} not found");
            }
        }

        #endregion

        #region Receive Messages

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == GLOBAL_CHAT_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string senderName = (string)data[0];
                string message = (string)data[1];
                
                ReceiveGlobalMessage(senderName, message);
            }
            else if (eventCode == WHISPER_CHAT_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string senderName = (string)data[0];
                string message = (string)data[1];
                
                ReceiveWhisper(senderName, message);
            }
            else if (eventCode == PARTY_CHAT_EVENT)
            {
                object[] data = (object[])photonEvent.CustomData;
                string senderName = (string)data[0];
                string message = (string)data[1];
                
                ReceivePartyMessage(senderName, message);
            }
        }

        private void ReceiveGlobalMessage(string senderName, string message)
        {
            Debug.Log($"[ChatSystem] Global: {senderName}: {message}");
            AddMessageToHistory(senderName, message, ChatChannel.Global);
        }

        [PunRPC]
        private void RPC_RoomMessage(string senderName, string message)
        {
            Debug.Log($"[ChatSystem] Room: {senderName}: {message}");
            AddMessageToHistory(senderName, message, ChatChannel.Room);
        }

        private void ReceivePartyMessage(string senderName, string message)
        {
            Debug.Log($"[ChatSystem] Party: {senderName}: {message}");
            AddMessageToHistory(senderName, message, ChatChannel.Party);
        }

        private void ReceiveWhisper(string senderName, string message)
        {
            Debug.Log($"[ChatSystem] Whisper from {senderName}: {message}");
            AddMessageToHistory(senderName, $"[From {senderName}] {message}", ChatChannel.Whisper);
        }

        #endregion

        #region Chat Commands

        /// <summary>
        /// Xử lý chat commands / Process chat commands
        /// </summary>
        public void ProcessChatCommand(string input)
        {
            if (string.IsNullOrEmpty(input)) return;

            if (!input.StartsWith("/"))
            {
                // Không phải command, gửi như room chat / Not a command, send as room chat
                SendRoomMessage(input);
                return;
            }

            string[] parts = input.Split(' ');
            string command = parts[0].ToLower();

            switch (command)
            {
                case "/global":
                case "/g":
                    if (parts.Length > 1)
                    {
                        string message = string.Join(" ", parts, 1, parts.Length - 1);
                        SendGlobalMessage(message);
                    }
                    break;

                case "/party":
                case "/p":
                    if (parts.Length > 1)
                    {
                        string message = string.Join(" ", parts, 1, parts.Length - 1);
                        SendPartyMessage(message);
                    }
                    break;

                case "/whisper":
                case "/w":
                    if (parts.Length > 2)
                    {
                        string targetPlayer = parts[1];
                        string message = string.Join(" ", parts, 2, parts.Length - 2);
                        SendWhisper(targetPlayer, message);
                    }
                    break;

                case "/room":
                case "/r":
                    if (parts.Length > 1)
                    {
                        string message = string.Join(" ", parts, 1, parts.Length - 1);
                        SendRoomMessage(message);
                    }
                    break;

                case "/help":
                    ShowChatHelp();
                    break;

                default:
                    Debug.LogWarning($"[ChatSystem] Unknown command: {command}");
                    break;
            }
        }

        private void ShowChatHelp()
        {
            Debug.Log("[ChatSystem] Chat Commands:");
            Debug.Log("/g or /global <message> - Send to global chat");
            Debug.Log("/r or /room <message> - Send to room chat");
            Debug.Log("/p or /party <message> - Send to party chat");
            Debug.Log("/w or /whisper <player> <message> - Send private message");
            Debug.Log("/help - Show this help");
        }

        #endregion

        #region Chat History

        /// <summary>
        /// Thêm message vào history / Add message to history
        /// </summary>
        private void AddMessageToHistory(string senderName, string message, ChatChannel channel)
        {
            ChatMessage chatMessage = new ChatMessage(senderName, message, channel);
            chatHistory.Add(chatMessage);

            // Giới hạn số lượng messages / Limit number of messages
            if (chatHistory.Count > maxChatHistory)
            {
                chatHistory.RemoveAt(0);
            }

            // Trigger event / Kích hoạt event
            MessageReceived?.Invoke(chatMessage);
        }

        /// <summary>
        /// Lấy chat history / Get chat history
        /// </summary>
        public List<ChatMessage> GetChatHistory()
        {
            return new List<ChatMessage>(chatHistory);
        }

        /// <summary>
        /// Lấy chat history theo channel / Get chat history by channel
        /// </summary>
        public List<ChatMessage> GetChatHistoryByChannel(ChatChannel channel)
        {
            List<ChatMessage> filteredHistory = new List<ChatMessage>();
            foreach (ChatMessage msg in chatHistory)
            {
                if (msg.channel == channel)
                {
                    filteredHistory.Add(msg);
                }
            }
            return filteredHistory;
        }

        /// <summary>
        /// Xóa chat history / Clear chat history
        /// </summary>
        public void ClearChatHistory()
        {
            chatHistory.Clear();
        }

        #endregion
    }
}
