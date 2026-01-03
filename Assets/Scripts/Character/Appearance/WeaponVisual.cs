using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Weapon visual display / Hiển thị vũ khí
    /// </summary>
    public class WeaponVisual : MonoBehaviour
    {
        [Header("Weapon Attachment Points / Điểm gắn vũ khí")]
        [SerializeField] private Transform rightHandAttachment;
        [SerializeField] private Transform leftHandAttachment;
        [SerializeField] private Transform backAttachment;
        
        [Header("Current Weapons / Vũ khí hiện tại")]
        [SerializeField] private GameObject currentWeaponRight;
        [SerializeField] private GameObject currentWeaponLeft;
        
        [Header("Weapon Settings / Cài đặt vũ khí")]
        [SerializeField] private WeaponType currentWeaponType;
        [SerializeField] private int weaponTier = 0;
        
        private bool isSheathed = false;
        
        /// <summary>
        /// Equip weapon / Trang bị vũ khí
        /// </summary>
        public void EquipWeapon(WeaponType weaponType, GameObject weaponPrefab, bool isRightHand = true)
        {
            currentWeaponType = weaponType;
            
            Transform attachment = isRightHand ? rightHandAttachment : leftHandAttachment;
            if (attachment == null)
                return;
                
            // Remove old weapon / Xóa vũ khí cũ
            if (isRightHand && currentWeaponRight != null)
                Destroy(currentWeaponRight);
            else if (!isRightHand && currentWeaponLeft != null)
                Destroy(currentWeaponLeft);
                
            // Instantiate new weapon / Tạo vũ khí mới
            if (weaponPrefab != null)
            {
                var weapon = Instantiate(weaponPrefab, attachment);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                
                if (isRightHand)
                    currentWeaponRight = weapon;
                else
                    currentWeaponLeft = weapon;
            }
        }
        
        /// <summary>
        /// Unequip weapon / Tháo vũ khí
        /// </summary>
        public void UnequipWeapon(bool isRightHand = true)
        {
            if (isRightHand && currentWeaponRight != null)
            {
                Destroy(currentWeaponRight);
                currentWeaponRight = null;
            }
            else if (!isRightHand && currentWeaponLeft != null)
            {
                Destroy(currentWeaponLeft);
                currentWeaponLeft = null;
            }
        }
        
        /// <summary>
        /// Sheath weapon / Treo vũ khí sau lưng
        /// </summary>
        public void SheathWeapon()
        {
            if (isSheathed)
                return;
                
            if (currentWeaponRight != null && backAttachment != null)
            {
                currentWeaponRight.transform.SetParent(backAttachment);
                currentWeaponRight.transform.localPosition = Vector3.zero;
                currentWeaponRight.transform.localRotation = Quaternion.identity;
            }
            
            isSheathed = true;
        }
        
        /// <summary>
        /// Unsheath weapon / Rút vũ khí
        /// </summary>
        public void UnsheathWeapon()
        {
            if (!isSheathed)
                return;
                
            if (currentWeaponRight != null && rightHandAttachment != null)
            {
                currentWeaponRight.transform.SetParent(rightHandAttachment);
                currentWeaponRight.transform.localPosition = Vector3.zero;
                currentWeaponRight.transform.localRotation = Quaternion.identity;
            }
            
            isSheathed = false;
        }
        
        /// <summary>
        /// Set weapon tier / Đặt cấp độ vũ khí
        /// </summary>
        public void SetWeaponTier(int tier)
        {
            weaponTier = Mathf.Max(0, tier);
            UpdateWeaponVisuals();
        }
        
        /// <summary>
        /// Update weapon visuals / Cập nhật hình ảnh vũ khí
        /// </summary>
        private void UpdateWeaponVisuals()
        {
            // TODO: Update weapon appearance based on tier
            // Cập nhật ngoại hình vũ khí dựa trên cấp độ
            Debug.Log($"Weapon updated: Type={currentWeaponType}, Tier={weaponTier}");
        }
        
        /// <summary>
        /// Set weapon color / Đặt màu vũ khí
        /// </summary>
        public void SetWeaponColor(Color color)
        {
            SetWeaponColor(currentWeaponRight, color);
            SetWeaponColor(currentWeaponLeft, color);
        }
        
        private void SetWeaponColor(GameObject weapon, Color color)
        {
            if (weapon == null)
                return;
                
            var renderer = weapon.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
        
        /// <summary>
        /// Get current weapon type / Lấy loại vũ khí hiện tại
        /// </summary>
        public WeaponType GetWeaponType()
        {
            return currentWeaponType;
        }
        
        /// <summary>
        /// Get weapon tier / Lấy cấp độ vũ khí
        /// </summary>
        public int GetWeaponTier()
        {
            return weaponTier;
        }
        
        /// <summary>
        /// Check if weapon is sheathed / Kiểm tra vũ khí đã treo sau lưng
        /// </summary>
        public bool IsSheathed()
        {
            return isSheathed;
        }
    }
}
