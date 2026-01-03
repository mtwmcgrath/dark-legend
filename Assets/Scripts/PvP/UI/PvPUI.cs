using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Main PvP UI - Giao diện chính PvP
    /// </summary>
    public class PvPUI : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject mainPanel;
        public GameObject duelPanel;
        public GameObject arenaPanel;
        public GameObject battlegroundPanel;
        public GameObject rankingPanel;
        public GameObject tournamentPanel;
        
        [Header("Buttons")]
        public Button duelButton;
        public Button arenaButton;
        public Button battlegroundButton;
        public Button rankingButton;
        public Button tournamentButton;
        public Button closeButton;
        
        private void Start()
        {
            // Setup button listeners
            if (duelButton != null)
                duelButton.onClick.AddListener(() => ShowPanel("duel"));
            if (arenaButton != null)
                arenaButton.onClick.AddListener(() => ShowPanel("arena"));
            if (battlegroundButton != null)
                battlegroundButton.onClick.AddListener(() => ShowPanel("battleground"));
            if (rankingButton != null)
                rankingButton.onClick.AddListener(() => ShowPanel("ranking"));
            if (tournamentButton != null)
                tournamentButton.onClick.AddListener(() => ShowPanel("tournament"));
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
            
            HideAllPanels();
        }
        
        /// <summary>
        /// Show PvP menu
        /// Hiện menu PvP
        /// </summary>
        public void Show()
        {
            if (mainPanel != null)
                mainPanel.SetActive(true);
        }
        
        /// <summary>
        /// Hide PvP menu
        /// Ẩn menu PvP
        /// </summary>
        public void Hide()
        {
            HideAllPanels();
            if (mainPanel != null)
                mainPanel.SetActive(false);
        }
        
        /// <summary>
        /// Show specific panel
        /// Hiện panel cụ thể
        /// </summary>
        private void ShowPanel(string panelName)
        {
            HideAllPanels();
            
            switch (panelName.ToLower())
            {
                case "duel":
                    if (duelPanel != null) duelPanel.SetActive(true);
                    break;
                case "arena":
                    if (arenaPanel != null) arenaPanel.SetActive(true);
                    break;
                case "battleground":
                    if (battlegroundPanel != null) battlegroundPanel.SetActive(true);
                    break;
                case "ranking":
                    if (rankingPanel != null) rankingPanel.SetActive(true);
                    break;
                case "tournament":
                    if (tournamentPanel != null) tournamentPanel.SetActive(true);
                    break;
            }
        }
        
        /// <summary>
        /// Hide all panels
        /// Ẩn tất cả panels
        /// </summary>
        private void HideAllPanels()
        {
            if (duelPanel != null) duelPanel.SetActive(false);
            if (arenaPanel != null) arenaPanel.SetActive(false);
            if (battlegroundPanel != null) battlegroundPanel.SetActive(false);
            if (rankingPanel != null) rankingPanel.SetActive(false);
            if (tournamentPanel != null) tournamentPanel.SetActive(false);
        }
    }
}
