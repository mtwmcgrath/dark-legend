using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Maps.UI
{
    /// <summary>
    /// UI hiển thị thông tin map hiện tại
    /// Current map UI display
    /// </summary>
    public class MapUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [Tooltip("Text tên map / Map name text")]
        [SerializeField] private Text mapNameText;
        
        [Tooltip("Text level range / Level range text")]
        [SerializeField] private Text levelRangeText;
        
        [Tooltip("Icon map / Map icon")]
        [SerializeField] private Image mapIcon;
        
        private Core.MapData currentMap;
        
        private void Start()
        {
            // Subscribe to map change events
            if (Core.MapManager.Instance != null)
            {
                Core.MapManager.Instance.OnMapChanged += OnMapChanged;
            }
            
            UpdateUI();
        }
        
        private void OnDestroy()
        {
            if (Core.MapManager.Instance != null)
            {
                Core.MapManager.Instance.OnMapChanged -= OnMapChanged;
            }
        }
        
        /// <summary>
        /// Khi map thay đổi / When map changes
        /// </summary>
        private void OnMapChanged(Core.MapData newMap, Core.MapData oldMap)
        {
            currentMap = newMap;
            UpdateUI();
        }
        
        /// <summary>
        /// Cập nhật UI / Update UI
        /// </summary>
        private void UpdateUI()
        {
            if (Core.MapManager.Instance != null)
            {
                currentMap = Core.MapManager.Instance.GetCurrentMap();
            }
            
            if (currentMap != null)
            {
                if (mapNameText != null)
                {
                    mapNameText.text = currentMap.mapName;
                }
                
                if (levelRangeText != null)
                {
                    levelRangeText.text = $"Level {currentMap.minLevel}-{currentMap.maxLevel}";
                }
                
                if (mapIcon != null && currentMap.minimapIcon != null)
                {
                    mapIcon.sprite = currentMap.minimapIcon;
                }
            }
        }
    }
}
