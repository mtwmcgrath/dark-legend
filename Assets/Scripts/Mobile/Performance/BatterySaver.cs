using UnityEngine;
using System.Collections;

namespace DarkLegend.Mobile.Performance
{
    /// <summary>
    /// Battery saver mode
    /// Chế độ tiết kiệm pin
    /// </summary>
    public class BatterySaver : MonoBehaviour
    {
        [Header("Battery Saver Settings")]
        public bool isEnabled = false;
        public int reducedFPS = 30;
        public float reducedBrightness = 0.7f;
        public bool disableEffects = true;
        public bool reduceAudio = true;

        [Header("Auto Enable")]
        public bool autoEnableOnLowBattery = true;
        public float lowBatteryThreshold = 0.2f;
        public float checkInterval = 30f;

        [Header("Settings Backup")]
        private int originalFPS;
        private float originalBrightness;
        private bool originalShadows;
        private float originalAudioVolume;

        private Coroutine batteryCheckCoroutine;

        private void Start()
        {
            // Backup original settings
            originalFPS = Application.targetFrameRate;
            originalBrightness = Screen.brightness;
            originalShadows = QualitySettings.shadows != ShadowQuality.Disable;
            originalAudioVolume = AudioListener.volume;

            if (autoEnableOnLowBattery)
            {
                StartBatteryMonitoring();
            }
        }

        /// <summary>
        /// Start battery monitoring
        /// Bắt đầu theo dõi pin
        /// </summary>
        private void StartBatteryMonitoring()
        {
            if (batteryCheckCoroutine == null)
            {
                batteryCheckCoroutine = StartCoroutine(MonitorBattery());
            }
        }

        /// <summary>
        /// Stop battery monitoring
        /// Dừng theo dõi pin
        /// </summary>
        private void StopBatteryMonitoring()
        {
            if (batteryCheckCoroutine != null)
            {
                StopCoroutine(batteryCheckCoroutine);
                batteryCheckCoroutine = null;
            }
        }

        /// <summary>
        /// Monitor battery level
        /// Theo dõi mức pin
        /// </summary>
        private IEnumerator MonitorBattery()
        {
            while (autoEnableOnLowBattery)
            {
                yield return new WaitForSeconds(checkInterval);

                float batteryLevel = SystemInfo.batteryLevel;

                if (batteryLevel > 0 && batteryLevel <= lowBatteryThreshold)
                {
                    if (!isEnabled)
                    {
                        Debug.Log($"[BatterySaver] Low battery detected ({batteryLevel * 100}%) - Enabling battery saver");
                        EnableBatterySaver();
                    }
                }
                else if (batteryLevel > lowBatteryThreshold + 0.1f)
                {
                    if (isEnabled)
                    {
                        Debug.Log($"[BatterySaver] Battery level restored ({batteryLevel * 100}%) - Disabling battery saver");
                        DisableBatterySaver();
                    }
                }
            }
        }

        /// <summary>
        /// Enable battery saver mode
        /// Bật chế độ tiết kiệm pin
        /// </summary>
        public void EnableBatterySaver()
        {
            if (isEnabled)
                return;

            isEnabled = true;

            // Reduce FPS
            Application.targetFrameRate = reducedFPS;

            // Reduce screen brightness
            Screen.brightness = reducedBrightness;

            // Disable effects
            if (disableEffects)
            {
                QualitySettings.shadows = ShadowQuality.Disable;
                QualitySettings.particleRaycastBudget = 16;
            }

            // Reduce audio
            if (reduceAudio)
            {
                AudioListener.volume = originalAudioVolume * 0.5f;
            }

            Debug.Log("[BatterySaver] Battery saver mode enabled");
        }

        /// <summary>
        /// Disable battery saver mode
        /// Tắt chế độ tiết kiệm pin
        /// </summary>
        public void DisableBatterySaver()
        {
            if (!isEnabled)
                return;

            isEnabled = false;

            // Restore FPS
            Application.targetFrameRate = originalFPS;

            // Restore screen brightness
            Screen.brightness = originalBrightness;

            // Restore effects
            if (disableEffects && originalShadows)
            {
                QualitySettings.shadows = ShadowQuality.All;
                QualitySettings.particleRaycastBudget = 256;
            }

            // Restore audio
            if (reduceAudio)
            {
                AudioListener.volume = originalAudioVolume;
            }

            Debug.Log("[BatterySaver] Battery saver mode disabled");
        }

        /// <summary>
        /// Toggle battery saver
        /// Bật/tắt battery saver
        /// </summary>
        public void ToggleBatterySaver()
        {
            if (isEnabled)
            {
                DisableBatterySaver();
            }
            else
            {
                EnableBatterySaver();
            }
        }

        /// <summary>
        /// Get current battery level
        /// Lấy mức pin hiện tại
        /// </summary>
        public float GetBatteryLevel()
        {
            return SystemInfo.batteryLevel;
        }

        /// <summary>
        /// Get battery status
        /// Lấy trạng thái pin
        /// </summary>
        public string GetBatteryStatus()
        {
            BatteryStatus status = SystemInfo.batteryStatus;
            return status.ToString();
        }

        private void OnDestroy()
        {
            StopBatteryMonitoring();
        }
    }
}
