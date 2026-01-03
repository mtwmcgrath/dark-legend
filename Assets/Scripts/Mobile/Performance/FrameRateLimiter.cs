using UnityEngine;

namespace DarkLegend.Mobile.Performance
{
    /// <summary>
    /// Frame rate limiter
    /// Giới hạn FPS
    /// </summary>
    public class FrameRateLimiter : MonoBehaviour
    {
        [Header("Frame Rate Settings")]
        public int targetFrameRate = 60;
        public bool limitFrameRate = true;
        public bool adaptiveFrameRate = false;

        [Header("Adaptive Settings")]
        public int minFrameRate = 30;
        public int maxFrameRate = 60;
        public float targetGPUTime = 16.6f; // ~60 FPS

        [Header("Monitoring")]
        public float currentFPS = 0f;
        public float averageFPS = 0f;
        public float currentGPUTime = 0f;

        private float fpsTimer = 0f;
        private int frameCount = 0;
        private float adjustTimer = 0f;
        private float adjustInterval = 2f;

        private void Start()
        {
            SetFrameRate(targetFrameRate);
        }

        private void Update()
        {
            CalculateFPS();

            if (adaptiveFrameRate)
            {
                adjustTimer += Time.unscaledDeltaTime;
                if (adjustTimer >= adjustInterval)
                {
                    AdjustFrameRate();
                    adjustTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Calculate FPS
        /// Tính toán FPS
        /// </summary>
        private void CalculateFPS()
        {
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
        /// Adjust frame rate adaptively
        /// Điều chỉnh FPS tự động
        /// </summary>
        private void AdjustFrameRate()
        {
            // Get GPU frame time
            currentGPUTime = Time.deltaTime * 1000f;

            if (currentGPUTime > targetGPUTime + 5f)
            {
                // GPU is struggling, reduce frame rate
                targetFrameRate = Mathf.Max(targetFrameRate - 5, minFrameRate);
                SetFrameRate(targetFrameRate);
                Debug.Log($"[FrameRateLimiter] Reducing frame rate to {targetFrameRate} (GPU time: {currentGPUTime:F1}ms)");
            }
            else if (currentGPUTime < targetGPUTime - 5f && targetFrameRate < maxFrameRate)
            {
                // GPU has headroom, increase frame rate
                targetFrameRate = Mathf.Min(targetFrameRate + 5, maxFrameRate);
                SetFrameRate(targetFrameRate);
                Debug.Log($"[FrameRateLimiter] Increasing frame rate to {targetFrameRate} (GPU time: {currentGPUTime:F1}ms)");
            }
        }

        /// <summary>
        /// Set frame rate
        /// Đặt FPS
        /// </summary>
        public void SetFrameRate(int fps)
        {
            if (limitFrameRate)
            {
                targetFrameRate = Mathf.Clamp(fps, minFrameRate, maxFrameRate);
                Application.targetFrameRate = targetFrameRate;
                QualitySettings.vSyncCount = 0;
                Debug.Log($"[FrameRateLimiter] Frame rate set to {targetFrameRate}");
            }
            else
            {
                Application.targetFrameRate = -1;
            }
        }

        /// <summary>
        /// Enable frame rate limit
        /// Bật giới hạn FPS
        /// </summary>
        public void EnableFrameRateLimit(bool enable)
        {
            limitFrameRate = enable;
            SetFrameRate(targetFrameRate);
        }

        /// <summary>
        /// Enable adaptive frame rate
        /// Bật FPS tự động
        /// </summary>
        public void EnableAdaptiveFrameRate(bool enable)
        {
            adaptiveFrameRate = enable;
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

        /// <summary>
        /// Enable VSync
        /// Bật VSync
        /// </summary>
        public void EnableVSync(bool enable)
        {
            QualitySettings.vSyncCount = enable ? 1 : 0;
            
            if (enable)
            {
                Application.targetFrameRate = -1;
            }
            else
            {
                SetFrameRate(targetFrameRate);
            }
        }
    }
}
