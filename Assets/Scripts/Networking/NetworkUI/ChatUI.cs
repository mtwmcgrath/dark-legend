using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DarkLegend.Networking.UI
{
    /// <summary>
    /// UI cho chat box / UI for chat box
    /// </summary>
    public class ChatUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform chatContent;
        [SerializeField] private GameObject chatMessagePrefab;
        [SerializeField] private TMP_InputField messageInput;
        [SerializeField] private Button sendButton;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Chat Tabs")]
        [SerializeField] private Toggle globalTab;
        [SerializeField] private Toggle roomTab;
        [SerializeField] private Toggle partyTab;

        [Header("Settings")]
        [SerializeField] private int maxMessages = 50;
        [SerializeField] private Color systemMessageColor = Color.yellow;
        [SerializeField] private Color globalMessageColor = Color.white;
        [SerializeField] private Color roomMessageColor = Color.cyan;
        [SerializeField] private Color partyMessageColor = Color.green;
        [SerializeField] private Color whisperMessageColor = Color.magenta;

        private ChatSystem chatSystem;
        private List<GameObject> chatMessages = new List<GameObject>();
        private ChatSystem.ChatChannel currentChannel = ChatSystem.ChatChannel.Room;

        private void Start()
        {
            // Lấy ChatSystem / Get ChatSystem
            chatSystem = ChatSystem.Instance;
            if (chatSystem == null)
            {
                GameObject chatObj = new GameObject("ChatSystem");
                chatSystem = chatObj.AddComponent<ChatSystem>();
            }

            // Setup UI / Thiết lập UI
            SetupUI();

            // Subscribe to events / Đăng ký events
            if (chatSystem != null)
            {
                chatSystem.MessageReceived += OnMessageReceived;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe events / Hủy đăng ký events
            if (chatSystem != null)
            {
                chatSystem.MessageReceived -= OnMessageReceived;
            }
        }

        private void SetupUI()
        {
            // Setup send button / Thiết lập nút gửi
            if (sendButton != null)
                sendButton.onClick.AddListener(OnSendButtonClicked);

            // Setup input field / Thiết lập ô nhập
            if (messageInput != null)
            {
                messageInput.onSubmit.AddListener(OnMessageSubmit);
            }

            // Setup tabs / Thiết lập tabs
            if (globalTab != null)
                globalTab.onValueChanged.AddListener((isOn) => { if (isOn) OnTabChanged(ChatSystem.ChatChannel.Global); });

            if (roomTab != null)
                roomTab.onValueChanged.AddListener((isOn) => { if (isOn) OnTabChanged(ChatSystem.ChatChannel.Room); });

            if (partyTab != null)
                partyTab.onValueChanged.AddListener((isOn) => { if (isOn) OnTabChanged(ChatSystem.ChatChannel.Party); });
        }

        #region Input Handlers

        private void OnSendButtonClicked()
        {
            SendMessage();
        }

        private void OnMessageSubmit(string message)
        {
            SendMessage();
        }

        private void SendMessage()
        {
            if (messageInput == null || chatSystem == null) return;

            string message = messageInput.text;
            if (string.IsNullOrEmpty(message)) return;

            // Xử lý chat commands hoặc gửi message / Process chat commands or send message
            chatSystem.ProcessChatCommand(message);

            // Xóa input / Clear input
            messageInput.text = "";
            messageInput.ActivateInputField();
        }

        #endregion

        #region Tab Management

        private void OnTabChanged(ChatSystem.ChatChannel channel)
        {
            currentChannel = channel;
            RefreshChatDisplay();
        }

        private void RefreshChatDisplay()
        {
            // Xóa messages hiện tại / Clear current messages
            ClearMessages();

            // Hiển thị messages theo channel / Display messages by channel
            if (chatSystem != null)
            {
                List<ChatSystem.ChatMessage> messages = chatSystem.GetChatHistoryByChannel(currentChannel);
                foreach (var msg in messages)
                {
                    DisplayMessage(msg);
                }
            }
        }

        #endregion

        #region Message Display

        private void OnMessageReceived(ChatSystem.ChatMessage message)
        {
            // Chỉ hiển thị nếu đang ở channel này / Only display if on this channel
            if (message.channel == currentChannel || message.channel == ChatSystem.ChatChannel.Whisper)
            {
                DisplayMessage(message);
            }
        }

        private void DisplayMessage(ChatSystem.ChatMessage message)
        {
            if (chatMessagePrefab == null || chatContent == null) return;

            // Tạo message item / Create message item
            GameObject messageObj = Instantiate(chatMessagePrefab, chatContent);
            chatMessages.Add(messageObj);

            // Setup message text / Thiết lập text
            TextMeshProUGUI messageText = messageObj.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                string channelPrefix = GetChannelPrefix(message.channel);
                messageText.text = $"{channelPrefix}[{message.senderName}]: {message.message}";
                messageText.color = GetChannelColor(message.channel);
            }

            // Giới hạn số lượng messages / Limit number of messages
            if (chatMessages.Count > maxMessages)
            {
                GameObject oldMessage = chatMessages[0];
                chatMessages.RemoveAt(0);
                Destroy(oldMessage);
            }

            // Scroll to bottom / Cuộn xuống cuối
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        private void DisplaySystemMessage(string message)
        {
            if (chatMessagePrefab == null || chatContent == null) return;

            GameObject messageObj = Instantiate(chatMessagePrefab, chatContent);
            chatMessages.Add(messageObj);

            TextMeshProUGUI messageText = messageObj.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.text = $"[System]: {message}";
                messageText.color = systemMessageColor;
            }
        }

        private void ClearMessages()
        {
            foreach (GameObject msg in chatMessages)
            {
                Destroy(msg);
            }
            chatMessages.Clear();
        }

        #endregion

        #region Helper Methods

        private string GetChannelPrefix(ChatSystem.ChatChannel channel)
        {
            switch (channel)
            {
                case ChatSystem.ChatChannel.Global:
                    return "[Global] ";
                case ChatSystem.ChatChannel.Room:
                    return "[Room] ";
                case ChatSystem.ChatChannel.Party:
                    return "[Party] ";
                case ChatSystem.ChatChannel.Whisper:
                    return "[Whisper] ";
                default:
                    return "";
            }
        }

        private Color GetChannelColor(ChatSystem.ChatChannel channel)
        {
            switch (channel)
            {
                case ChatSystem.ChatChannel.Global:
                    return globalMessageColor;
                case ChatSystem.ChatChannel.Room:
                    return roomMessageColor;
                case ChatSystem.ChatChannel.Party:
                    return partyMessageColor;
                case ChatSystem.ChatChannel.Whisper:
                    return whisperMessageColor;
                default:
                    return Color.white;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Thêm system message / Add system message
        /// </summary>
        public void AddSystemMessage(string message)
        {
            DisplaySystemMessage(message);
        }

        /// <summary>
        /// Focus input field / Tập trung vào input field
        /// </summary>
        public void FocusInput()
        {
            if (messageInput != null)
            {
                messageInput.ActivateInputField();
            }
        }

        #endregion
    }
}
