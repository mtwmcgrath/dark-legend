using UnityEngine;

namespace DarkLegend.Maps.Core
{
    /// <summary>
    /// Xử lý chuyển map và portals
    /// Handles map transitions and portals
    /// </summary>
    public class MapTransition : MonoBehaviour
    {
        [Header("Transition Settings")]
        [Tooltip("Thời gian fade / Fade duration")]
        [SerializeField] private float transitionDuration = 1f;
        
        [Tooltip("Màu fade / Fade color")]
        [SerializeField] private Color fadeColor = Color.black;
        
        [Header("Effects")]
        [Tooltip("Hiệu ứng portal / Portal effect prefab")]
        [SerializeField] private GameObject portalEffectPrefab;
        
        [Tooltip("Âm thanh chuyển map / Transition sound")]
        [SerializeField] private AudioClip transitionSound;
        
        private bool isTransitioning = false;
        
        /// <summary>
        /// Chuyển đến map mới qua portal / Transition to new map via portal
        /// </summary>
        public void TransitionToMap(PortalData portalData, int playerLevel)
        {
            if (isTransitioning)
            {
                Debug.Log("[MapTransition] Already transitioning!");
                return;
            }
            
            if (portalData.destinationMap == null)
            {
                Debug.LogError("[MapTransition] Destination map is null!");
                return;
            }
            
            // Kiểm tra yêu cầu
            if (!CanUsePortal(portalData, playerLevel))
            {
                return;
            }
            
            // Bắt đầu chuyển map
            StartCoroutine(TransitionCoroutine(portalData));
        }
        
        /// <summary>
        /// Kiểm tra có thể dùng portal không / Check if can use portal
        /// </summary>
        private bool CanUsePortal(PortalData portalData, int playerLevel)
        {
            // Kiểm tra level
            if (playerLevel < portalData.requiredLevel)
            {
                Debug.Log($"[MapTransition] Level too low! Required: {portalData.requiredLevel}, Current: {playerLevel}");
                ShowMessage($"Cần level {portalData.requiredLevel} để sử dụng portal này!");
                return false;
            }
            
            // Kiểm tra item yêu cầu
            if (!string.IsNullOrEmpty(portalData.requiredItem))
            {
                // TODO: Kiểm tra inventory
                Debug.Log($"[MapTransition] Checking for required item: {portalData.requiredItem}");
            }
            
            // Kiểm tra Zen
            if (portalData.zenCost > 0)
            {
                // TODO: Kiểm tra Zen trong inventory
                Debug.Log($"[MapTransition] Zen cost: {portalData.zenCost}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Coroutine chuyển map / Transition coroutine
        /// </summary>
        private System.Collections.IEnumerator TransitionCoroutine(PortalData portalData)
        {
            isTransitioning = true;
            
            // Play sound
            if (transitionSound != null)
            {
                AudioSource.PlayClipAtPoint(transitionSound, transform.position);
            }
            
            // Show portal effect
            if (portalEffectPrefab != null)
            {
                GameObject effect = Instantiate(portalEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            // Fade out
            yield return FadeOut();
            
            // Chuyển map thông qua MapManager
            MapManager.Instance.TransitionToMap(
                portalData.destinationMap, 
                portalData.destinationSpawnPos
            );
            
            // Fade in
            yield return FadeIn();
            
            isTransitioning = false;
        }
        
        /// <summary>
        /// Fade out màn hình / Fade out screen
        /// </summary>
        private System.Collections.IEnumerator FadeOut()
        {
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / transitionDuration);
                // TODO: Apply fade to screen
                yield return null;
            }
        }
        
        /// <summary>
        /// Fade in màn hình / Fade in screen
        /// </summary>
        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / transitionDuration);
                // TODO: Apply fade to screen
                yield return null;
            }
        }
        
        /// <summary>
        /// Hiển thị thông báo / Show message to player
        /// </summary>
        private void ShowMessage(string message)
        {
            // TODO: Show UI message
            Debug.Log($"[MapTransition] Message: {message}");
        }
        
        /// <summary>
        /// Teleport nhanh (không fade) / Quick teleport (no fade)
        /// </summary>
        public void QuickTeleport(MapData targetMap, Vector3 position)
        {
            if (targetMap == null)
            {
                Debug.LogError("[MapTransition] Target map is null!");
                return;
            }
            
            MapManager.Instance.TransitionToMap(targetMap, position);
        }
        
        /// <summary>
        /// Town portal - về thành phố / Return to town
        /// </summary>
        public void UseTownPortal()
        {
            // TODO: Check if player has town portal item
            Debug.Log("[MapTransition] Using town portal...");
            MapManager.Instance.ReturnToTown();
        }
    }
}
