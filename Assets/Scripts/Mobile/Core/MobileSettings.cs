using UnityEngine;

namespace DarkLegend.Mobile.Core
{
    /// <summary>
    /// Mobile-specific settings
    /// Cài đặt đặc biệt cho mobile
    /// </summary>
    public class MobileSettings : MonoBehaviour
    {
        [Header("Touch Settings")]
        public float touchSensitivity = 1.0f;
        public float doubleTapTime = 0.3f;
        public float longPressTime = 0.5f;
        public float swipeThreshold = 50f;

        [Header("Camera Settings")]
        public float cameraRotationSpeed = 0.5f;
        public bool invertYAxis = false;
        public float pinchZoomSpeed = 0.5f;

        [Header("Control Settings")]
        public float joystickDeadZone = 0.1f;
        public bool dynamicJoystick = true;
        public bool hapticFeedback = true;
        public bool showTouchIndicators = true;

        [Header("UI Settings")]
        public float uiScale = 1.0f;
        public bool autoHideUI = false;
        public float uiAutoHideDelay = 3f;
        public bool showFPSCounter = false;

        [Header("Performance Settings")]
        public int targetFrameRate = 60;
        public bool adaptiveFrameRate = true;
        public bool lowPowerMode = false;

        [Header("Audio Settings")]
        public float masterVolume = 1.0f;
        public float musicVolume = 0.7f;
        public float sfxVolume = 0.8f;
        public bool enableVibration = true;

        private void Awake()
        {
            LoadSettings();
        }

        /// <summary>
        /// Load settings từ PlayerPrefs
        /// Load settings from PlayerPrefs
        /// </summary>
        public void LoadSettings()
        {
            touchSensitivity = PlayerPrefs.GetFloat("Mobile_TouchSensitivity", 1.0f);
            cameraRotationSpeed = PlayerPrefs.GetFloat("Mobile_CameraRotationSpeed", 0.5f);
            invertYAxis = PlayerPrefs.GetInt("Mobile_InvertYAxis", 0) == 1;
            hapticFeedback = PlayerPrefs.GetInt("Mobile_HapticFeedback", 1) == 1;
            targetFrameRate = PlayerPrefs.GetInt("Mobile_TargetFrameRate", 60);
            masterVolume = PlayerPrefs.GetFloat("Mobile_MasterVolume", 1.0f);
            musicVolume = PlayerPrefs.GetFloat("Mobile_MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("Mobile_SFXVolume", 0.8f);

            Debug.Log("[MobileSettings] Settings loaded from PlayerPrefs");
        }

        /// <summary>
        /// Save settings vào PlayerPrefs
        /// Save settings to PlayerPrefs
        /// </summary>
        public void SaveSettings()
        {
            PlayerPrefs.SetFloat("Mobile_TouchSensitivity", touchSensitivity);
            PlayerPrefs.SetFloat("Mobile_CameraRotationSpeed", cameraRotationSpeed);
            PlayerPrefs.SetInt("Mobile_InvertYAxis", invertYAxis ? 1 : 0);
            PlayerPrefs.SetInt("Mobile_HapticFeedback", hapticFeedback ? 1 : 0);
            PlayerPrefs.SetInt("Mobile_TargetFrameRate", targetFrameRate);
            PlayerPrefs.SetFloat("Mobile_MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("Mobile_MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("Mobile_SFXVolume", sfxVolume);
            PlayerPrefs.Save();

            Debug.Log("[MobileSettings] Settings saved to PlayerPrefs");
        }

        /// <summary>
        /// Apply mobile settings
        /// Áp dụng cài đặt mobile
        /// </summary>
        public void ApplyMobileSettings()
        {
            // Apply frame rate
            Application.targetFrameRate = targetFrameRate;
            Debug.Log($"[MobileSettings] Target frame rate set to {targetFrameRate}");

            // Apply audio settings
            AudioListener.volume = masterVolume;

            // Apply screen settings
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// Reset về default settings
        /// Reset to default settings
        /// </summary>
        public void ResetToDefaults()
        {
            touchSensitivity = 1.0f;
            cameraRotationSpeed = 0.5f;
            invertYAxis = false;
            hapticFeedback = true;
            targetFrameRate = 60;
            masterVolume = 1.0f;
            musicVolume = 0.7f;
            sfxVolume = 0.8f;

            SaveSettings();
            Debug.Log("[MobileSettings] Settings reset to defaults");
        }

        /// <summary>
        /// Set touch sensitivity
        /// Đặt độ nhạy cảm chạm
        /// </summary>
        public void SetTouchSensitivity(float value)
        {
            touchSensitivity = Mathf.Clamp(value, 0.1f, 2.0f);
            SaveSettings();
        }

        /// <summary>
        /// Set camera rotation speed
        /// Đặt tốc độ xoay camera
        /// </summary>
        public void SetCameraRotationSpeed(float value)
        {
            cameraRotationSpeed = Mathf.Clamp(value, 0.1f, 2.0f);
            SaveSettings();
        }

        /// <summary>
        /// Toggle haptic feedback
        /// Bật/tắt rung phản hồi
        /// </summary>
        public void ToggleHapticFeedback(bool enabled)
        {
            hapticFeedback = enabled;
            SaveSettings();
        }

        /// <summary>
        /// Set target frame rate
        /// Đặt FPS mục tiêu
        /// </summary>
        public void SetTargetFrameRate(int fps)
        {
            targetFrameRate = Mathf.Clamp(fps, 30, 120);
            Application.targetFrameRate = targetFrameRate;
            SaveSettings();
        }
    }
}
