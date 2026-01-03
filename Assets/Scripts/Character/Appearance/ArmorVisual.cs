using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Armor visual display / Hiển thị giáp
    /// </summary>
    public class ArmorVisual : MonoBehaviour
    {
        [Header("Armor Parts / Phần giáp")]
        [SerializeField] private GameObject helmetMesh;
        [SerializeField] private GameObject chestMesh;
        [SerializeField] private GameObject pantsMesh;
        [SerializeField] private GameObject glovesMesh;
        [SerializeField] private GameObject bootsMesh;
        
        [Header("Current Armor / Giáp hiện tại")]
        [SerializeField] private ArmorType currentArmorType;
        [SerializeField] private int armorTier = 0;
        
        /// <summary>
        /// Set armor type / Đặt loại giáp
        /// </summary>
        public void SetArmorType(ArmorType armorType)
        {
            currentArmorType = armorType;
            UpdateArmorVisuals();
        }
        
        /// <summary>
        /// Set armor tier / Đặt cấp độ giáp
        /// </summary>
        public void SetArmorTier(int tier)
        {
            armorTier = Mathf.Max(0, tier);
            UpdateArmorVisuals();
        }
        
        /// <summary>
        /// Update armor visuals / Cập nhật hình ảnh giáp
        /// </summary>
        private void UpdateArmorVisuals()
        {
            // TODO: Load appropriate armor meshes based on type and tier
            // Tải mesh giáp phù hợp dựa trên loại và cấp độ
            Debug.Log($"Armor updated: Type={currentArmorType}, Tier={armorTier}");
        }
        
        /// <summary>
        /// Show armor part / Hiển thị phần giáp
        /// </summary>
        public void ShowArmorPart(string partName, bool show)
        {
            GameObject part = GetArmorPart(partName);
            if (part != null)
                part.SetActive(show);
        }
        
        /// <summary>
        /// Get armor part / Lấy phần giáp
        /// </summary>
        private GameObject GetArmorPart(string partName)
        {
            return partName.ToLower() switch
            {
                "helmet" => helmetMesh,
                "chest" => chestMesh,
                "pants" => pantsMesh,
                "gloves" => glovesMesh,
                "boots" => bootsMesh,
                _ => null
            };
        }
        
        /// <summary>
        /// Set armor color / Đặt màu giáp
        /// </summary>
        public void SetArmorColor(Color color)
        {
            SetPartColor(helmetMesh, color);
            SetPartColor(chestMesh, color);
            SetPartColor(pantsMesh, color);
            SetPartColor(glovesMesh, color);
            SetPartColor(bootsMesh, color);
        }
        
        /// <summary>
        /// Set part color / Đặt màu phần
        /// </summary>
        private void SetPartColor(GameObject part, Color color)
        {
            if (part == null)
                return;
                
            var renderer = part.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
        
        /// <summary>
        /// Get current armor type / Lấy loại giáp hiện tại
        /// </summary>
        public ArmorType GetArmorType()
        {
            return currentArmorType;
        }
        
        /// <summary>
        /// Get current armor tier / Lấy cấp độ giáp hiện tại
        /// </summary>
        public int GetArmorTier()
        {
            return armorTier;
        }
    }
}
