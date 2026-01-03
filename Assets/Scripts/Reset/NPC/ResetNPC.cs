using UnityEngine;
using System;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset NPC - NPC thực hiện reset
    /// NPC that handles reset operations for players
    /// </summary>
    public class ResetNPC : MonoBehaviour
    {
        [Header("NPC Info")]
        [Tooltip("NPC name - Tên NPC")]
        public string npcName = "Reset Master";

        [Tooltip("NPC location - Vị trí NPC")]
        public string npcLocation = "Lorencia";

        [Tooltip("NPC description - Mô tả NPC")]
        [TextArea(3, 5)]
        public string npcDescription = "The Reset Master can help you transcend your limits by resetting your character.";

        [Header("Services")]
        [Tooltip("Offer normal reset - Cung cấp reset thường")]
        public bool offerNormalReset = true;

        [Tooltip("Offer grand reset - Cung cấp Grand Reset")]
        public bool offerGrandReset = true;

        [Tooltip("Offer master reset - Cung cấp Master Reset")]
        public bool offerMasterReset = true;

        [Tooltip("Show reset history - Hiển thị lịch sử reset")]
        public bool showResetHistory = true;

        [Tooltip("Show reset ranking - Hiển thị bảng xếp hạng reset")]
        public bool showResetRanking = true;

        [Header("Interaction Range")]
        [Tooltip("Interaction distance - Khoảng cách tương tác")]
        public float interactionRange = 3f;

        [Header("Events")]
        public event Action<CharacterStats> OnPlayerInteract;
        public event Action OnDialogClosed;

        private CharacterStats currentPlayer;

        /// <summary>
        /// Check if player is in range to interact
        /// Kiểm tra xem người chơi có trong phạm vi tương tác không
        /// </summary>
        public bool IsPlayerInRange(Transform playerTransform)
        {
            if (playerTransform == null)
                return false;

            float distance = Vector3.Distance(transform.position, playerTransform.position);
            return distance <= interactionRange;
        }

        /// <summary>
        /// Player interacts with NPC
        /// Người chơi tương tác với NPC
        /// </summary>
        public void Interact(CharacterStats player)
        {
            if (player == null)
            {
                Debug.LogWarning("Invalid player trying to interact with Reset NPC");
                return;
            }

            currentPlayer = player;
            OnPlayerInteract?.Invoke(player);

            // Open reset UI
            OpenResetUI();
        }

        /// <summary>
        /// Open reset UI for player
        /// Mở UI reset cho người chơi
        /// </summary>
        private void OpenResetUI()
        {
            // This will be implemented by UI system
            // Sẽ được implement bởi UI system
            Debug.Log($"{npcName}: Opening Reset UI for {currentPlayer.name}");

            // For now, show dialog
            ShowGreeting();
        }

        /// <summary>
        /// Show greeting message
        /// Hiển thị lời chào
        /// </summary>
        public void ShowGreeting()
        {
            string greeting = GetGreetingMessage(currentPlayer);
            Debug.Log($"{npcName}: {greeting}");
        }

        /// <summary>
        /// Get personalized greeting message
        /// Lấy lời chào cá nhân hóa
        /// </summary>
        public string GetGreetingMessage(CharacterStats player)
        {
            if (player == null)
                return "Welcome, traveler!";

            string message = $"Greetings, {player.name}!\n\n";

            // Check reset status
            if (player.hasMasterReset)
            {
                message += "I see you have achieved Master status! You are among the elite!\n\n";
            }
            else if (player.grandResetCount > 0)
            {
                message += $"You have performed {player.grandResetCount} Grand Reset(s). Impressive!\n\n";
            }
            else if (player.normalResetCount > 0)
            {
                message += $"You have reset {player.normalResetCount} time(s). Keep going!\n\n";
            }
            else
            {
                message += "I sense great potential in you. Would you like to transcend your limits?\n\n";
            }

            message += "What service can I provide today?";
            return message;
        }

        /// <summary>
        /// Get available services for player
        /// Lấy các dịch vụ khả dụng cho người chơi
        /// </summary>
        public string[] GetAvailableServices(CharacterStats player)
        {
            System.Collections.Generic.List<string> services = new System.Collections.Generic.List<string>();

            if (offerNormalReset)
            {
                if (ResetSystem.Instance.CanPerformNormalReset(player, out _))
                    services.Add("Normal Reset");
                else
                    services.Add("Normal Reset (Requirements not met)");
            }

            if (offerGrandReset)
            {
                if (ResetSystem.Instance.CanPerformGrandReset(player, out _))
                    services.Add("Grand Reset");
                else
                    services.Add("Grand Reset (Requirements not met)");
            }

            if (offerMasterReset)
            {
                if (ResetSystem.Instance.CanPerformMasterReset(player, out _))
                    services.Add("Master Reset");
                else
                    services.Add("Master Reset (Requirements not met)");
            }

            if (showResetHistory)
                services.Add("View Reset History");

            if (showResetRanking)
                services.Add("View Reset Rankings");

            services.Add("Close");

            return services.ToArray();
        }

        /// <summary>
        /// Handle service selection
        /// Xử lý lựa chọn dịch vụ
        /// </summary>
        public void HandleServiceSelection(int serviceIndex)
        {
            if (currentPlayer == null)
                return;

            string[] services = GetAvailableServices(currentPlayer);
            if (serviceIndex < 0 || serviceIndex >= services.Length)
                return;

            string selectedService = services[serviceIndex];

            if (selectedService.StartsWith("Normal Reset"))
            {
                ShowNormalResetInfo();
            }
            else if (selectedService.StartsWith("Grand Reset"))
            {
                ShowGrandResetInfo();
            }
            else if (selectedService.StartsWith("Master Reset"))
            {
                ShowMasterResetInfo();
            }
            else if (selectedService == "View Reset History")
            {
                ShowResetHistory();
            }
            else if (selectedService == "View Reset Rankings")
            {
                ShowResetRankings();
            }
            else if (selectedService == "Close")
            {
                CloseDialog();
            }
        }

        private void ShowNormalResetInfo()
        {
            string info = ResetSystem.Instance.GetResetInfo(currentPlayer, ResetType.Normal);
            Debug.Log($"{npcName}:\n{info}");
        }

        private void ShowGrandResetInfo()
        {
            string info = ResetSystem.Instance.GetResetInfo(currentPlayer, ResetType.Grand);
            Debug.Log($"{npcName}:\n{info}");
        }

        private void ShowMasterResetInfo()
        {
            string info = ResetSystem.Instance.GetResetInfo(currentPlayer, ResetType.Master);
            Debug.Log($"{npcName}:\n{info}");
        }

        private void ShowResetHistory()
        {
            if (currentPlayer.resetHistory == null || currentPlayer.resetHistory.Entries.Count == 0)
            {
                Debug.Log($"{npcName}: You have not performed any resets yet.");
                return;
            }

            Debug.Log($"{npcName}: Your Reset History:");
            var recentResets = currentPlayer.resetHistory.GetRecentResets(10);
            foreach (var entry in recentResets)
            {
                Debug.Log($"  {entry.GetFormattedString()}");
            }
        }

        private void ShowResetRankings()
        {
            Debug.Log($"{npcName}: Reset rankings are not yet implemented.");
        }

        private void CloseDialog()
        {
            currentPlayer = null;
            OnDialogClosed?.Invoke();
            Debug.Log($"{npcName}: Farewell, traveler!");
        }

        private void OnDrawGizmosSelected()
        {
            // Draw interaction range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }
    }
}
