using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Tournament UI - Giao diện tournament
    /// </summary>
    public class TournamentUI : MonoBehaviour
    {
        [Header("Tournament List")]
        public Transform tournamentListContent;
        public GameObject tournamentListItemPrefab;
        
        [Header("Registration")]
        public GameObject registrationPanel;
        public TextMeshProUGUI tournamentNameText;
        public TextMeshProUGUI prizePoolText;
        public TextMeshProUGUI participantsText;
        public Button registerButton;
        
        [Header("Bracket")]
        public GameObject bracketPanel;
        public Transform bracketContent;
        public GameObject matchNodePrefab;
        
        private TournamentManager tournamentManager;
        private TournamentBracket selectedTournament;
        
        private void Start()
        {
            tournamentManager = PvPManager.Instance?.GetTournamentManager();
            
            if (registerButton != null)
                registerButton.onClick.AddListener(OnRegisterClicked);
            
            RefreshTournamentList();
        }
        
        /// <summary>
        /// Refresh tournament list
        /// Làm mới danh sách tournament
        /// </summary>
        private void RefreshTournamentList()
        {
            if (tournamentManager == null || tournamentListContent == null) return;
            
            // Clear existing items
            foreach (Transform child in tournamentListContent)
            {
                Destroy(child.gameObject);
            }
            
            // Get active tournaments
            var tournaments = tournamentManager.GetActiveTournaments();
            
            // Create list items
            foreach (var tournament in tournaments)
            {
                CreateTournamentListItem(tournament);
            }
        }
        
        /// <summary>
        /// Create tournament list item
        /// Tạo mục danh sách tournament
        /// </summary>
        private void CreateTournamentListItem(TournamentBracket tournament)
        {
            if (tournamentListItemPrefab == null || tournamentListContent == null) return;
            
            GameObject item = Instantiate(tournamentListItemPrefab, tournamentListContent);
            
            // Set item data
            var nameText = item.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            var button = item.GetComponent<Button>();
            
            if (nameText != null)
                nameText.text = tournament.tournamentType.ToString();
            
            if (button != null)
                button.onClick.AddListener(() => SelectTournament(tournament));
        }
        
        /// <summary>
        /// Select tournament
        /// Chọn tournament
        /// </summary>
        private void SelectTournament(TournamentBracket tournament)
        {
            selectedTournament = tournament;
            ShowRegistrationPanel();
        }
        
        /// <summary>
        /// Show registration panel
        /// Hiện panel đăng ký
        /// </summary>
        private void ShowRegistrationPanel()
        {
            if (registrationPanel == null || selectedTournament == null) return;
            
            registrationPanel.SetActive(true);
            
            if (tournamentNameText != null)
                tournamentNameText.text = selectedTournament.tournamentType.ToString();
            if (prizePoolText != null)
                prizePoolText.text = $"Prize Pool: {selectedTournament.prizePool:N0} Zen";
            // TODO: Get actual participant count
            if (participantsText != null)
                participantsText.text = $"Participants: 0 / {selectedTournament.maxParticipants}";
        }
        
        /// <summary>
        /// Register button clicked
        /// Nhấn nút đăng ký
        /// </summary>
        private void OnRegisterClicked()
        {
            if (tournamentManager == null || selectedTournament == null) return;
            
            // TODO: Get local player
            GameObject player = null; // Replace with actual player
            if (player != null)
            {
                bool success = tournamentManager.RegisterPlayer(selectedTournament, player);
                if (success)
                {
                    Debug.Log("Successfully registered for tournament");
                }
            }
        }
        
        /// <summary>
        /// Show bracket view
        /// Hiện bảng đấu
        /// </summary>
        public void ShowBracket(TournamentBracket tournament)
        {
            if (bracketPanel == null || bracketContent == null) return;
            
            bracketPanel.SetActive(true);
            
            // Clear existing nodes
            foreach (Transform child in bracketContent)
            {
                Destroy(child.gameObject);
            }
            
            // Create bracket visualization
            var matches = tournament.GetCurrentRoundMatches();
            foreach (var match in matches)
            {
                CreateMatchNode(match);
            }
        }
        
        /// <summary>
        /// Create match node in bracket
        /// Tạo nút trận đấu trong bảng đấu
        /// </summary>
        private void CreateMatchNode(TournamentMatch match)
        {
            if (matchNodePrefab == null || bracketContent == null) return;
            
            GameObject node = Instantiate(matchNodePrefab, bracketContent);
            
            // Set match data
            var team1Text = node.transform.Find("Team1Text")?.GetComponent<TextMeshProUGUI>();
            var team2Text = node.transform.Find("Team2Text")?.GetComponent<TextMeshProUGUI>();
            
            if (team1Text != null && match.team1.Count > 0)
                team1Text.text = match.team1[0].name;
            if (team2Text != null && match.team2.Count > 0)
                team2Text.text = match.team2[0].name;
        }
    }
}
