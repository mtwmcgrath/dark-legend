using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Duel UI - Giao diện đấu tay đôi
    /// </summary>
    public class DuelUI : MonoBehaviour
    {
        [Header("Request UI")]
        public GameObject requestPanel;
        public TextMeshProUGUI requestText;
        public Button acceptButton;
        public Button declineButton;
        
        [Header("Duel Info UI")]
        public GameObject duelInfoPanel;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI player1NameText;
        public TextMeshProUGUI player2NameText;
        public Slider player1HealthBar;
        public Slider player2HealthBar;
        
        [Header("Settings UI")]
        public GameObject settingsPanel;
        public TMP_Dropdown duelTypeDropdown;
        public Toggle allowPotionsToggle;
        public Toggle allowSkillsToggle;
        public TMP_InputField betAmountInput;
        public Button sendRequestButton;
        
        private DuelSystem duelSystem;
        private DuelRequest currentRequest;
        
        private void Start()
        {
            duelSystem = PvPManager.Instance?.GetDuelSystem();
            
            if (duelSystem != null)
            {
                duelSystem.OnDuelRequestSent += OnDuelRequestReceived;
                duelSystem.OnDuelAccepted += OnDuelStarted;
                duelSystem.OnDuelEnd += OnDuelEnded;
            }
            
            // Setup button listeners
            if (acceptButton != null)
                acceptButton.onClick.AddListener(OnAcceptClicked);
            if (declineButton != null)
                declineButton.onClick.AddListener(OnDeclineClicked);
            if (sendRequestButton != null)
                sendRequestButton.onClick.AddListener(OnSendRequestClicked);
            
            HideAllPanels();
        }
        
        private void OnDestroy()
        {
            if (duelSystem != null)
            {
                duelSystem.OnDuelRequestSent -= OnDuelRequestReceived;
                duelSystem.OnDuelAccepted -= OnDuelStarted;
                duelSystem.OnDuelEnd -= OnDuelEnded;
            }
        }
        
        private void Update()
        {
            // Update duel timer if in duel
            if (duelInfoPanel != null && duelInfoPanel.activeSelf)
            {
                UpdateDuelInfo();
            }
        }
        
        /// <summary>
        /// Show duel request popup
        /// Hiện popup yêu cầu đấu
        /// </summary>
        private void OnDuelRequestReceived(GameObject challenger, GameObject target)
        {
            // TODO: Check if this is for local player
            currentRequest = new DuelRequest(challenger, target, new DuelSettings());
            
            if (requestPanel != null)
            {
                requestPanel.SetActive(true);
                if (requestText != null)
                {
                    requestText.text = $"{challenger.name} challenges you to a duel!";
                }
            }
        }
        
        private void OnAcceptClicked()
        {
            if (currentRequest != null && duelSystem != null)
            {
                duelSystem.AcceptDuel(currentRequest.target, currentRequest.challenger);
            }
            HideRequestPanel();
        }
        
        private void OnDeclineClicked()
        {
            if (currentRequest != null && duelSystem != null)
            {
                duelSystem.DeclineDuel(currentRequest.target, currentRequest.challenger);
            }
            HideRequestPanel();
        }
        
        private void OnSendRequestClicked()
        {
            // TODO: Get target player and send request with settings
            DuelSettings settings = GetSettingsFromUI();
            // duelSystem.SendDuelRequest(localPlayer, targetPlayer, settings);
        }
        
        private void OnDuelStarted(GameObject player1, GameObject player2)
        {
            if (duelInfoPanel != null)
            {
                duelInfoPanel.SetActive(true);
                if (player1NameText != null)
                    player1NameText.text = player1.name;
                if (player2NameText != null)
                    player2NameText.text = player2.name;
            }
        }
        
        private void OnDuelEnded(GameObject winner, GameObject loser, GameObject draw)
        {
            HideDuelInfoPanel();
            
            // TODO: Show result popup
            string resultText = winner != null ? $"{winner.name} wins!" : "Draw!";
            Debug.Log(resultText);
        }
        
        private void UpdateDuelInfo()
        {
            // TODO: Update health bars and timer
            // This requires integration with player health system
        }
        
        private DuelSettings GetSettingsFromUI()
        {
            DuelSettings settings = new DuelSettings();
            
            if (duelTypeDropdown != null)
            {
                settings.type = (DuelType)duelTypeDropdown.value;
            }
            if (allowPotionsToggle != null)
            {
                settings.allowPotions = allowPotionsToggle.isOn;
            }
            if (allowSkillsToggle != null)
            {
                settings.allowSkills = allowSkillsToggle.isOn;
            }
            if (betAmountInput != null && int.TryParse(betAmountInput.text, out int bet))
            {
                settings.betAmount = bet;
            }
            
            return settings;
        }
        
        private void HideRequestPanel()
        {
            if (requestPanel != null)
                requestPanel.SetActive(false);
        }
        
        private void HideDuelInfoPanel()
        {
            if (duelInfoPanel != null)
                duelInfoPanel.SetActive(false);
        }
        
        private void HideAllPanels()
        {
            HideRequestPanel();
            HideDuelInfoPanel();
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
        
        public void ShowSettingsPanel()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(true);
        }
        
        public void HideSettingsPanel()
        {
            if (settingsPanel != null)
                settingsPanel.SetActive(false);
        }
    }
}
