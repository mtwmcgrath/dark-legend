using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DarkLegend.Networking.UI
{
    /// <summary>
    /// UI cho party / UI for party
    /// </summary>
    public class PartyUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject partyPanel;
        [SerializeField] private Transform partyMemberListContent;
        [SerializeField] private GameObject partyMemberItemPrefab;
        [SerializeField] private Button createPartyButton;
        [SerializeField] private Button leavePartyButton;
        [SerializeField] private TMP_InputField invitePlayerInput;
        [SerializeField] private Button inviteButton;
        [SerializeField] private TextMeshProUGUI partyLeaderText;

        [Header("Invite Panel")]
        [SerializeField] private GameObject invitePanel;
        [SerializeField] private TextMeshProUGUI inviteMessageText;
        [SerializeField] private Button acceptInviteButton;
        [SerializeField] private Button declineInviteButton;

        private PartySystem partySystem;
        private List<GameObject> partyMemberItems = new List<GameObject>();
        private string pendingInviteFrom = "";

        private void Start()
        {
            // Lấy PartySystem / Get PartySystem
            partySystem = PartySystem.Instance;
            if (partySystem == null)
            {
                GameObject partyObj = new GameObject("PartySystem");
                partySystem = partyObj.AddComponent<PartySystem>();
            }

            // Setup UI / Thiết lập UI
            SetupUI();

            // Subscribe to events / Đăng ký events
            if (partySystem != null)
            {
                partySystem.PartyCreated += OnPartyCreated;
                partySystem.PartyJoined += OnPartyJoined;
                partySystem.PartyLeft += OnPartyLeft;
                partySystem.PartyMemberJoined += OnPartyMemberJoined;
                partySystem.PartyMemberLeft += OnPartyMemberLeft;
                partySystem.PartyInviteReceived += OnPartyInviteReceived;
            }

            // Ẩn party panel và invite panel / Hide party panel and invite panel
            if (partyPanel != null)
                partyPanel.SetActive(false);

            if (invitePanel != null)
                invitePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            // Unsubscribe events / Hủy đăng ký events
            if (partySystem != null)
            {
                partySystem.PartyCreated -= OnPartyCreated;
                partySystem.PartyJoined -= OnPartyJoined;
                partySystem.PartyLeft -= OnPartyLeft;
                partySystem.PartyMemberJoined -= OnPartyMemberJoined;
                partySystem.PartyMemberLeft -= OnPartyMemberLeft;
                partySystem.PartyInviteReceived -= OnPartyInviteReceived;
            }
        }

        private void SetupUI()
        {
            // Setup buttons / Thiết lập buttons
            if (createPartyButton != null)
                createPartyButton.onClick.AddListener(OnCreatePartyButtonClicked);

            if (leavePartyButton != null)
                leavePartyButton.onClick.AddListener(OnLeavePartyButtonClicked);

            if (inviteButton != null)
                inviteButton.onClick.AddListener(OnInviteButtonClicked);

            if (acceptInviteButton != null)
                acceptInviteButton.onClick.AddListener(OnAcceptInviteButtonClicked);

            if (declineInviteButton != null)
                declineInviteButton.onClick.AddListener(OnDeclineInviteButtonClicked);
        }

        #region Button Handlers

        private void OnCreatePartyButtonClicked()
        {
            if (partySystem != null)
            {
                partySystem.CreateParty();
            }
        }

        private void OnLeavePartyButtonClicked()
        {
            if (partySystem != null)
            {
                partySystem.LeaveParty();
            }
        }

        private void OnInviteButtonClicked()
        {
            if (invitePlayerInput == null || partySystem == null) return;

            string playerName = invitePlayerInput.text;
            if (!string.IsNullOrEmpty(playerName))
            {
                partySystem.InvitePlayer(playerName);
                invitePlayerInput.text = "";
            }
        }

        private void OnAcceptInviteButtonClicked()
        {
            if (partySystem != null && !string.IsNullOrEmpty(pendingInviteFrom))
            {
                partySystem.AcceptPartyInvite(pendingInviteFrom);
                
                if (invitePanel != null)
                    invitePanel.SetActive(false);
                
                pendingInviteFrom = "";
            }
        }

        private void OnDeclineInviteButtonClicked()
        {
            if (partySystem != null && !string.IsNullOrEmpty(pendingInviteFrom))
            {
                partySystem.DeclinePartyInvite(pendingInviteFrom);
                
                if (invitePanel != null)
                    invitePanel.SetActive(false);
                
                pendingInviteFrom = "";
            }
        }

        #endregion

        #region Party Callbacks

        private void OnPartyCreated()
        {
            Debug.Log("[PartyUI] Party created");
            
            if (partyPanel != null)
                partyPanel.SetActive(true);
            
            UpdatePartyUI();
        }

        private void OnPartyJoined(List<Photon.Realtime.Player> members)
        {
            Debug.Log("[PartyUI] Joined party");
            
            if (partyPanel != null)
                partyPanel.SetActive(true);
            
            UpdatePartyUI();
        }

        private void OnPartyLeft()
        {
            Debug.Log("[PartyUI] Left party");
            
            if (partyPanel != null)
                partyPanel.SetActive(false);
            
            ClearPartyMemberList();
        }

        private void OnPartyMemberJoined(Photon.Realtime.Player player)
        {
            Debug.Log($"[PartyUI] {player.NickName} joined party");
            UpdatePartyUI();
        }

        private void OnPartyMemberLeft(Photon.Realtime.Player player)
        {
            Debug.Log($"[PartyUI] {player.NickName} left party");
            UpdatePartyUI();
        }

        private void OnPartyInviteReceived(string senderName)
        {
            Debug.Log($"[PartyUI] Received party invite from {senderName}");
            
            pendingInviteFrom = senderName;
            
            if (invitePanel != null)
            {
                invitePanel.SetActive(true);
            }
            
            if (inviteMessageText != null)
            {
                inviteMessageText.text = $"{senderName} invited you to join their party!";
            }
        }

        #endregion

        #region Party UI Update

        private void UpdatePartyUI()
        {
            if (partySystem == null) return;

            // Cập nhật party leader text / Update party leader text
            if (partyLeaderText != null)
            {
                var leader = partySystem.GetPartyLeader();
                if (leader != null)
                {
                    partyLeaderText.text = $"Leader: {leader.NickName}";
                }
            }

            // Cập nhật party member list / Update party member list
            UpdatePartyMemberList();

            // Cập nhật buttons / Update buttons
            if (createPartyButton != null)
                createPartyButton.gameObject.SetActive(!partySystem.IsInParty());

            if (leavePartyButton != null)
                leavePartyButton.gameObject.SetActive(partySystem.IsInParty());

            if (inviteButton != null)
                inviteButton.interactable = partySystem.IsPartyLeader();
        }

        private void UpdatePartyMemberList()
        {
            // Xóa list cũ / Clear old list
            ClearPartyMemberList();

            if (partySystem == null || !partySystem.IsInParty()) return;

            // Tạo member items / Create member items
            List<Photon.Realtime.Player> members = partySystem.GetPartyMembers();
            foreach (var member in members)
            {
                CreatePartyMemberItem(member);
            }
        }

        private void CreatePartyMemberItem(Photon.Realtime.Player player)
        {
            if (partyMemberItemPrefab == null || partyMemberListContent == null) return;

            GameObject item = Instantiate(partyMemberItemPrefab, partyMemberListContent);
            partyMemberItems.Add(item);

            // Setup item data / Thiết lập dữ liệu item
            TextMeshProUGUI playerNameText = item.transform.Find("PlayerName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerLevelText = item.transform.Find("PlayerLevel")?.GetComponent<TextMeshProUGUI>();
            Image healthBar = item.transform.Find("HealthBar")?.GetComponent<Image>();
            Image leaderIcon = item.transform.Find("LeaderIcon")?.GetComponent<Image>();
            Button kickButton = item.transform.Find("KickButton")?.GetComponent<Button>();

            if (playerNameText != null)
                playerNameText.text = player.NickName;

            if (playerLevelText != null)
            {
                int level = RoomManager.GetPlayerLevel(player);
                playerLevelText.text = $"Lv.{level}";
            }

            if (leaderIcon != null)
            {
                bool isLeader = partySystem != null && partySystem.GetPartyLeader() == player;
                leaderIcon.gameObject.SetActive(isLeader);
            }

            if (kickButton != null)
            {
                bool canKick = partySystem != null && partySystem.IsPartyLeader() && 
                               player != partySystem.GetPartyLeader();
                kickButton.gameObject.SetActive(canKick);
                
                if (canKick)
                {
                    kickButton.onClick.AddListener(() => OnKickMemberClicked(player.NickName));
                }
            }
        }

        private void ClearPartyMemberList()
        {
            foreach (GameObject item in partyMemberItems)
            {
                Destroy(item);
            }
            partyMemberItems.Clear();
        }

        private void OnKickMemberClicked(string playerName)
        {
            if (partySystem != null)
            {
                partySystem.KickMember(playerName);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Toggle party panel visibility / Hiện/ẩn party panel
        /// </summary>
        public void TogglePartyPanel()
        {
            if (partyPanel != null)
            {
                partyPanel.SetActive(!partyPanel.activeSelf);
            }
        }

        #endregion
    }
}
