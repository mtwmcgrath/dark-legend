using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Auto play button
    /// Nút tự động chơi
    /// </summary>
    public class AutoPlayButton : MonoBehaviour
    {
        [Header("UI Components")]
        public Button autoPlayButton;
        public Text buttonText;
        public Image buttonImage;

        [Header("Visual")]
        public Color enabledColor = Color.green;
        public Color disabledColor = Color.gray;
        public string enabledText = "Auto: ON";
        public string disabledText = "Auto: OFF";

        private bool isAutoPlayEnabled = false;

        private void Start()
        {
            if (autoPlayButton != null)
            {
                autoPlayButton.onClick.AddListener(ToggleAutoPlay);
            }

            UpdateVisual();
        }

        /// <summary>
        /// Toggle auto play
        /// Bật/tắt auto play
        /// </summary>
        public void ToggleAutoPlay()
        {
            isAutoPlayEnabled = !isAutoPlayEnabled;
            UpdateVisual();

            if (isAutoPlayEnabled)
            {
                EnableAutoPlay();
            }
            else
            {
                DisableAutoPlay();
            }

            Debug.Log($"[AutoPlayButton] Auto play: {(isAutoPlayEnabled ? "ON" : "OFF")}");
        }

        /// <summary>
        /// Enable auto play
        /// Bật auto play
        /// </summary>
        private void EnableAutoPlay()
        {
            // TODO: Enable auto play system
            // AutoPlaySystem.Instance?.EnableAutoPlay();
        }

        /// <summary>
        /// Disable auto play
        /// Tắt auto play
        /// </summary>
        private void DisableAutoPlay()
        {
            // TODO: Disable auto play system
            // AutoPlaySystem.Instance?.DisableAutoPlay();
        }

        /// <summary>
        /// Update visual
        /// Cập nhật hiển thị
        /// </summary>
        private void UpdateVisual()
        {
            if (buttonImage != null)
            {
                buttonImage.color = isAutoPlayEnabled ? enabledColor : disabledColor;
            }

            if (buttonText != null)
            {
                buttonText.text = isAutoPlayEnabled ? enabledText : disabledText;
            }
        }

        /// <summary>
        /// Set auto play
        /// Đặt auto play
        /// </summary>
        public void SetAutoPlay(bool enabled)
        {
            isAutoPlayEnabled = enabled;
            UpdateVisual();

            if (isAutoPlayEnabled)
            {
                EnableAutoPlay();
            }
            else
            {
                DisableAutoPlay();
            }
        }

        /// <summary>
        /// Is auto play enabled
        /// Auto play có được bật không
        /// </summary>
        public bool IsAutoPlayEnabled()
        {
            return isAutoPlayEnabled;
        }
    }
}
