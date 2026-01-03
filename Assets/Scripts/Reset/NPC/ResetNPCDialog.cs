using UnityEngine;
using System;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset NPC Dialog - Hệ thống dialog với NPC reset
    /// Handles dialog system for reset NPC interactions
    /// </summary>
    public class ResetNPCDialog : MonoBehaviour
    {
        [Header("Dialog Settings")]
        [Tooltip("Dialog title - Tiêu đề dialog")]
        public string dialogTitle = "Reset Master";

        [Header("Messages")]
        [Tooltip("Greeting message - Lời chào")]
        [TextArea(2, 4)]
        public string greetingMessage = "Chào mừng đến với Reset Master! Bạn muốn reset nhân vật?";

        [Tooltip("Success message - Thông báo thành công")]
        [TextArea(2, 4)]
        public string successMessage = "Reset thành công! Nhân vật của bạn đã mạnh hơn!";

        [Tooltip("Fail message - Thông báo thất bại")]
        [TextArea(2, 4)]
        public string failMessage = "Bạn chưa đủ điều kiện để reset.";

        [Tooltip("Confirmation message - Thông báo xác nhận")]
        [TextArea(2, 4)]
        public string confirmationMessage = "Bạn có chắc chắn muốn reset? Hành động này không thể hoàn tác!";

        [Header("Events")]
        public event Action<string> OnMessageShown;
        public event Action OnDialogOpened;
        public event Action OnDialogClosed;

        private ResetNPC resetNPC;
        private CharacterStats currentPlayer;
        private bool isDialogOpen;

        private void Awake()
        {
            resetNPC = GetComponent<ResetNPC>();
            if (resetNPC != null)
            {
                resetNPC.OnPlayerInteract += HandlePlayerInteract;
                resetNPC.OnDialogClosed += HandleDialogClosed;
            }
        }

        private void OnDestroy()
        {
            if (resetNPC != null)
            {
                resetNPC.OnPlayerInteract -= HandlePlayerInteract;
                resetNPC.OnDialogClosed -= HandleDialogClosed;
            }
        }

        /// <summary>
        /// Handle player interaction
        /// Xử lý tương tác người chơi
        /// </summary>
        private void HandlePlayerInteract(CharacterStats player)
        {
            currentPlayer = player;
            OpenDialog();
        }

        /// <summary>
        /// Open dialog
        /// Mở dialog
        /// </summary>
        public void OpenDialog()
        {
            isDialogOpen = true;
            OnDialogOpened?.Invoke();
            ShowMessage(greetingMessage);
        }

        /// <summary>
        /// Close dialog
        /// Đóng dialog
        /// </summary>
        public void CloseDialog()
        {
            isDialogOpen = false;
            currentPlayer = null;
            OnDialogClosed?.Invoke();
        }

        /// <summary>
        /// Handle dialog closed event
        /// Xử lý sự kiện đóng dialog
        /// </summary>
        private void HandleDialogClosed()
        {
            CloseDialog();
        }

        /// <summary>
        /// Show a message
        /// Hiển thị thông báo
        /// </summary>
        public void ShowMessage(string message)
        {
            OnMessageShown?.Invoke(message);
            Debug.Log($"[{dialogTitle}] {message}");
        }

        /// <summary>
        /// Show success message
        /// Hiển thị thông báo thành công
        /// </summary>
        public void ShowSuccess(ResetType resetType)
        {
            string message = $"{successMessage}\n\nReset Type: {resetType}";
            ShowMessage(message);
        }

        /// <summary>
        /// Show failure message with reason
        /// Hiển thị thông báo thất bại kèm lý do
        /// </summary>
        public void ShowFailure(string reason)
        {
            string message = $"{failMessage}\n\nReason: {reason}";
            ShowMessage(message);
        }

        /// <summary>
        /// Show confirmation dialog
        /// Hiển thị dialog xác nhận
        /// </summary>
        public void ShowConfirmation(ResetType resetType, Action onConfirm, Action onCancel)
        {
            string message = $"{confirmationMessage}\n\nReset Type: {resetType}";
            ShowMessage(message);

            // This would trigger a UI confirmation dialog
            // For now, we'll just log it
            Debug.Log("Confirmation required. Implement UI confirmation dialog.");
        }

        /// <summary>
        /// Get dialog options based on player state
        /// Lấy các tùy chọn dialog dựa trên trạng thái người chơi
        /// </summary>
        public string[] GetDialogOptions()
        {
            if (currentPlayer == null || resetNPC == null)
                return new string[] { "Close" };

            return resetNPC.GetAvailableServices(currentPlayer);
        }

        /// <summary>
        /// Handle option selection
        /// Xử lý lựa chọn tùy chọn
        /// </summary>
        public void SelectOption(int optionIndex)
        {
            if (resetNPC != null)
            {
                resetNPC.HandleServiceSelection(optionIndex);
            }
        }

        /// <summary>
        /// Format reset info for dialog
        /// Định dạng thông tin reset cho dialog
        /// </summary>
        public string FormatResetInfo(ResetType resetType)
        {
            if (currentPlayer == null)
                return "Invalid player";

            return ResetSystem.Instance.GetResetInfo(currentPlayer, resetType);
        }

        /// <summary>
        /// Create dialog box visual
        /// Tạo hình ảnh hộp thoại
        /// </summary>
        public string CreateDialogBox(string content)
        {
            string border = "┌─────────────────────────────────────────────────────────────┐\n";
            string title = $"│  {dialogTitle.PadRight(57)}│\n";
            string divider = "├─────────────────────────────────────────────────────────────┤\n";
            string footer = "└─────────────────────────────────────────────────────────────┘";

            string[] lines = content.Split('\n');
            string body = "";
            foreach (string line in lines)
            {
                string paddedLine = line.Length > 57 ? line.Substring(0, 57) : line.PadRight(57);
                body += $"│  {paddedLine}│\n";
            }

            return border + title + divider + body + footer;
        }

        /// <summary>
        /// Is dialog currently open
        /// Dialog hiện có đang mở không
        /// </summary>
        public bool IsDialogOpen()
        {
            return isDialogOpen;
        }
    }
}
