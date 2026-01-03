using UnityEngine;

namespace DarkLegend.Mobile.Performance
{
    /// <summary>
    /// Mobile quality settings manager
    /// Quản lý cài đặt chất lượng cho mobile
    /// </summary>
    public class MobileQualitySettings : MonoBehaviour
    {
        [Header("Quality Presets")]
        public QualityLevel currentQuality = QualityLevel.Medium;

        public enum QualityLevel
        {
            VeryLow,
            Low,
            Medium,
            High,
            VeryHigh
        }

        [Header("Graphics Settings")]
        public bool dynamicShadows = true;
        public int shadowDistance = 50;
        public ShadowQuality shadowQuality = ShadowQuality.All;
        public int textureQuality = 0;
        public bool antiAliasing = true;
        public bool postProcessing = false;

        [Header("Performance Settings")]
        public int targetFrameRate = 60;
        public bool vSync = false;
        public int pixelLightCount = 4;
        public bool softParticles = true;

        private void Start()
        {
            ApplyQualitySettings(currentQuality);
        }

        /// <summary>
        /// Apply quality settings
        /// Áp dụng cài đặt chất lượng
        /// </summary>
        public void ApplyQualitySettings(QualityLevel quality)
        {
            currentQuality = quality;

            switch (quality)
            {
                case QualityLevel.VeryLow:
                    ApplyVeryLowSettings();
                    break;

                case QualityLevel.Low:
                    ApplyLowSettings();
                    break;

                case QualityLevel.Medium:
                    ApplyMediumSettings();
                    break;

                case QualityLevel.High:
                    ApplyHighSettings();
                    break;

                case QualityLevel.VeryHigh:
                    ApplyVeryHighSettings();
                    break;
            }

            Debug.Log($"[MobileQualitySettings] Quality set to: {quality}");
        }

        /// <summary>
        /// Very Low quality settings
        /// </summary>
        private void ApplyVeryLowSettings()
        {
            QualitySettings.SetQualityLevel(0);
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0;
            QualitySettings.masterTextureLimit = 3;
            QualitySettings.antiAliasing = 0;
            QualitySettings.pixelLightCount = 0;
            QualitySettings.softParticles = false;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }

        /// <summary>
        /// Low quality settings
        /// </summary>
        private void ApplyLowSettings()
        {
            QualitySettings.SetQualityLevel(1);
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowDistance = 20;
            QualitySettings.masterTextureLimit = 2;
            QualitySettings.antiAliasing = 0;
            QualitySettings.pixelLightCount = 1;
            QualitySettings.softParticles = false;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 30;
        }

        /// <summary>
        /// Medium quality settings
        /// </summary>
        private void ApplyMediumSettings()
        {
            QualitySettings.SetQualityLevel(2);
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowDistance = 50;
            QualitySettings.masterTextureLimit = 1;
            QualitySettings.antiAliasing = 0;
            QualitySettings.pixelLightCount = 2;
            QualitySettings.softParticles = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 45;
        }

        /// <summary>
        /// High quality settings
        /// </summary>
        private void ApplyHighSettings()
        {
            QualitySettings.SetQualityLevel(3);
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowDistance = 100;
            QualitySettings.masterTextureLimit = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.pixelLightCount = 4;
            QualitySettings.softParticles = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// Very High quality settings (flagship devices)
        /// </summary>
        private void ApplyVeryHighSettings()
        {
            QualitySettings.SetQualityLevel(4);
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowDistance = 150;
            QualitySettings.masterTextureLimit = 0;
            QualitySettings.antiAliasing = 4;
            QualitySettings.pixelLightCount = 8;
            QualitySettings.softParticles = true;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }

        /// <summary>
        /// Set custom quality
        /// Đặt chất lượng tùy chỉnh
        /// </summary>
        public void SetCustomQuality(
            bool shadows,
            int shadowDist,
            int texQuality,
            bool aa,
            int fps)
        {
            QualitySettings.shadows = shadows ? ShadowQuality.All : ShadowQuality.Disable;
            QualitySettings.shadowDistance = shadowDist;
            QualitySettings.masterTextureLimit = texQuality;
            QualitySettings.antiAliasing = aa ? 2 : 0;
            Application.targetFrameRate = fps;
        }

        /// <summary>
        /// Auto detect quality based on device
        /// Tự động phát hiện chất lượng dựa trên thiết bị
        /// </summary>
        public void AutoDetectQuality()
        {
            int systemMemory = SystemInfo.systemMemorySize;
            int processorCount = SystemInfo.processorCount;

            // Flagship device (8GB+ RAM, 8+ cores)
            if (systemMemory >= 8192 && processorCount >= 8)
            {
                ApplyQualitySettings(QualityLevel.VeryHigh);
            }
            // High-end device (6GB+ RAM, 6+ cores)
            else if (systemMemory >= 6144 && processorCount >= 6)
            {
                ApplyQualitySettings(QualityLevel.High);
            }
            // Mid-range device (4GB+ RAM, 4+ cores)
            else if (systemMemory >= 4096 && processorCount >= 4)
            {
                ApplyQualitySettings(QualityLevel.Medium);
            }
            // Low-end device (3GB+ RAM)
            else if (systemMemory >= 3072)
            {
                ApplyQualitySettings(QualityLevel.Low);
            }
            // Very low-end device
            else
            {
                ApplyQualitySettings(QualityLevel.VeryLow);
            }

            Debug.Log($"[MobileQualitySettings] Auto-detected quality: {currentQuality}");
        }
    }
}
