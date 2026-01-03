using UnityEngine;
using System.Collections;

namespace DarkLegend.Mobile.Core
{
    /// <summary>
    /// Performance optimization cho mobile
    /// Performance optimization for mobile
    /// </summary>
    public class MobileOptimization : MonoBehaviour
    {
        [Header("Quality Presets")]
        public MobileQualityLevel qualityLevel = MobileQualityLevel.Medium;

        [Header("Auto Optimization")]
        public bool autoAdjustQuality = true;
        public int targetFPS = 45;
        public float checkInterval = 5f;

        [Header("Quality Settings")]
        public int shadowDistance = 50;
        public int textureQuality = 2;
        public bool enableAntiAliasing = true;
        public bool enablePostProcessing = false;
        public int particleLimit = 100;

        [Header("Performance Monitoring")]
        public float currentFPS = 60f;
        public float averageFPS = 60f;
        private float fpsTimer = 0f;
        private int frameCount = 0;

        public enum MobileQualityLevel
        {
            Low,        // 30 FPS, Low shadows, Simple effects
            Medium,     // 45 FPS, Medium shadows, Standard effects
            High,       // 60 FPS, High shadows, Full effects
            Ultra       // 60 FPS, Max quality (flagship phones only)
        }

        private void Start()
        {
            ApplyQualityLevel(qualityLevel);
            
            if (autoAdjustQuality)
            {
                StartCoroutine(MonitorPerformance());
            }
        }

        private void Update()
        {
            // Calculate FPS
            fpsTimer += Time.unscaledDeltaTime;
            frameCount++;

            if (fpsTimer >= 1f)
            {
                currentFPS = frameCount / fpsTimer;
                averageFPS = Mathf.Lerp(averageFPS, currentFPS, 0.1f);
                fpsTimer = 0f;
                frameCount = 0;
            }
        }

        /// <summary>
        /// Áp dụng mobile optimizations
        /// Apply mobile optimizations
        /// </summary>
        public void ApplyMobileOptimizations()
        {
            Debug.Log("[MobileOptimization] Applying mobile optimizations...");

            // Disable unnecessary features
            Physics.autoSimulation = true;
            Physics.autoSyncTransforms = false;

            // Set quality settings
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFPS;

            // Apply quality level
            ApplyQualityLevel(qualityLevel);

            Debug.Log("[MobileOptimization] Mobile optimizations applied!");
        }

        /// <summary>
        /// Áp dụng quality level
        /// Apply quality level
        /// </summary>
        public void ApplyQualityLevel(MobileQualityLevel level)
        {
            qualityLevel = level;

            switch (level)
            {
                case MobileQualityLevel.Low:
                    ApplyLowQuality();
                    break;

                case MobileQualityLevel.Medium:
                    ApplyMediumQuality();
                    break;

                case MobileQualityLevel.High:
                    ApplyHighQuality();
                    break;

                case MobileQualityLevel.Ultra:
                    ApplyUltraQuality();
                    break;
            }

            Debug.Log($"[MobileOptimization] Quality level set to: {level}");
        }

        /// <summary>
        /// Low quality settings - 30 FPS
        /// </summary>
        private void ApplyLowQuality()
        {
            QualitySettings.SetQualityLevel(0, true);
            Application.targetFrameRate = 30;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 20;
            QualitySettings.masterTextureLimit = 2;
            QualitySettings.antiAliasing = 0;
            QualitySettings.particleRaycastBudget = 64;
            QualitySettings.maximumLODLevel = 2;
        }

        /// <summary>
        /// Medium quality settings - 45 FPS
        /// </summary>
        private void ApplyMediumQuality()
        {
            QualitySettings.SetQualityLevel(1, true);
            Application.targetFrameRate = 45;
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowDistance = 50;
            QualitySettings.masterTextureLimit = 1;
            QualitySettings.antiAliasing = 0;
            QualitySettings.particleRaycastBudget = 256;
            QualitySettings.maximumLODLevel = 1;
        }

        /// <summary>
        /// High quality settings - 60 FPS
        /// </summary>
        private void ApplyHighQuality()
        {
            QualitySettings.SetQualityLevel(2, true);
            Application.targetFrameRate = 60;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowDistance = 100;
            QualitySettings.masterTextureLimit = 0;
            QualitySettings.antiAliasing = 2;
            QualitySettings.particleRaycastBudget = 512;
            QualitySettings.maximumLODLevel = 0;
        }

        /// <summary>
        /// Ultra quality settings - 60 FPS (flagship phones)
        /// </summary>
        private void ApplyUltraQuality()
        {
            QualitySettings.SetQualityLevel(3, true);
            Application.targetFrameRate = 60;
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowDistance = 150;
            QualitySettings.masterTextureLimit = 0;
            QualitySettings.antiAliasing = 4;
            QualitySettings.particleRaycastBudget = 1024;
            QualitySettings.maximumLODLevel = 0;
        }

        /// <summary>
        /// Monitor performance và auto-adjust quality
        /// Monitor performance and auto-adjust quality
        /// </summary>
        private IEnumerator MonitorPerformance()
        {
            while (autoAdjustQuality)
            {
                yield return new WaitForSeconds(checkInterval);

                // Check if FPS is below target
                if (averageFPS < targetFPS - 10)
                {
                    // Downgrade quality
                    if (qualityLevel > MobileQualityLevel.Low)
                    {
                        ApplyQualityLevel(qualityLevel - 1);
                        Debug.Log($"[MobileOptimization] Performance low - downgrading to {qualityLevel}");
                    }
                }
                // Check if FPS is consistently high
                else if (averageFPS > targetFPS + 10)
                {
                    // Upgrade quality
                    if (qualityLevel < MobileQualityLevel.Ultra)
                    {
                        ApplyQualityLevel(qualityLevel + 1);
                        Debug.Log($"[MobileOptimization] Performance good - upgrading to {qualityLevel}");
                    }
                }
            }
        }

        /// <summary>
        /// Get current FPS
        /// Lấy FPS hiện tại
        /// </summary>
        public float GetCurrentFPS()
        {
            return currentFPS;
        }

        /// <summary>
        /// Get average FPS
        /// Lấy FPS trung bình
        /// </summary>
        public float GetAverageFPS()
        {
            return averageFPS;
        }
    }
}
