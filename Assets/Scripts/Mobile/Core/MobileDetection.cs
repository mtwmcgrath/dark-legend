using UnityEngine;

namespace DarkLegend.Mobile.Core
{
    /// <summary>
    /// Detect platform (PC/Mobile)
    /// Phát hiện platform (PC/Mobile)
    /// </summary>
    public class MobileDetection : MonoBehaviour
    {
        [Header("Platform Info")]
        public RuntimePlatform currentPlatform;
        public bool isMobile = false;
        public bool isAndroid = false;
        public bool isIOS = false;
        public bool isTablet = false;

        [Header("Device Info")]
        public string deviceModel;
        public string deviceName;
        public int processorCount;
        public int systemMemorySize;

        private void Awake()
        {
            DetectPlatform();
            GetDeviceInfo();
        }

        /// <summary>
        /// Phát hiện platform hiện tại
        /// Detect current platform
        /// </summary>
        private void DetectPlatform()
        {
            currentPlatform = Application.platform;

            switch (currentPlatform)
            {
                case RuntimePlatform.Android:
                    isMobile = true;
                    isAndroid = true;
                    Debug.Log("[MobileDetection] Platform: Android");
                    break;

                case RuntimePlatform.IPhonePlayer:
                    isMobile = true;
                    isIOS = true;
                    Debug.Log("[MobileDetection] Platform: iOS");
                    break;

                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                    isMobile = false;
                    Debug.Log("[MobileDetection] Platform: PC");
                    break;

                default:
                    isMobile = false;
                    Debug.Log($"[MobileDetection] Platform: {currentPlatform}");
                    break;
            }

            // Check if device is tablet
            CheckIfTablet();
        }

        /// <summary>
        /// Lấy thông tin thiết bị
        /// Get device information
        /// </summary>
        private void GetDeviceInfo()
        {
            deviceModel = SystemInfo.deviceModel;
            deviceName = SystemInfo.deviceName;
            processorCount = SystemInfo.processorCount;
            systemMemorySize = SystemInfo.systemMemorySize;

            Debug.Log($"[MobileDetection] Device: {deviceName} ({deviceModel})");
            Debug.Log($"[MobileDetection] CPU Cores: {processorCount}, RAM: {systemMemorySize}MB");
        }

        /// <summary>
        /// Kiểm tra xem có phải tablet không
        /// Check if device is tablet
        /// </summary>
        private void CheckIfTablet()
        {
            if (!isMobile)
            {
                isTablet = false;
                return;
            }

            // Check screen size and DPI
            float screenSize = Mathf.Sqrt(
                (Screen.width / Screen.dpi) * (Screen.width / Screen.dpi) +
                (Screen.height / Screen.dpi) * (Screen.height / Screen.dpi)
            );

            // Tablets usually have screen size > 6.5 inches
            isTablet = screenSize > 6.5f;

            Debug.Log($"[MobileDetection] Screen Size: {screenSize:F2} inches - {(isTablet ? "Tablet" : "Phone")}");
        }

        /// <summary>
        /// Kiểm tra có phải mobile platform không
        /// Check if mobile platform
        /// </summary>
        public bool IsMobilePlatform()
        {
            return isMobile;
        }

        /// <summary>
        /// Kiểm tra có phải Android không
        /// Check if Android platform
        /// </summary>
        public bool IsAndroid()
        {
            return isAndroid;
        }

        /// <summary>
        /// Kiểm tra có phải iOS không
        /// Check if iOS platform
        /// </summary>
        public bool IsIOS()
        {
            return isIOS;
        }

        /// <summary>
        /// Kiểm tra có phải tablet không
        /// Check if tablet device
        /// </summary>
        public bool IsTablet()
        {
            return isTablet;
        }

        /// <summary>
        /// Lấy thông tin platform
        /// Get platform information
        /// </summary>
        public string GetPlatformInfo()
        {
            return $"Platform: {currentPlatform}\n" +
                   $"Device: {deviceName} ({deviceModel})\n" +
                   $"Type: {(isTablet ? "Tablet" : "Phone")}\n" +
                   $"CPU: {processorCount} cores\n" +
                   $"RAM: {systemMemorySize}MB";
        }
    }
}
