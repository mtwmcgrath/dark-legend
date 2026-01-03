using UnityEngine;

namespace DarkLegend.Utils
{
    /// <summary>
    /// Utility extension methods
    /// Các phương thức mở rộng tiện ích
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Get or add component to GameObject
        /// Lấy hoặc thêm component vào GameObject
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Reset transform to default values
        /// Đặt lại transform về giá trị mặc định
        /// </summary>
        public static void ResetTransform(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Check if layer is in layermask
        /// Kiểm tra layer có trong layermask không
        /// </summary>
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        /// <summary>
        /// Remap value from one range to another
        /// Chuyển đổi giá trị từ phạm vi này sang phạm vi khác
        /// </summary>
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        /// <summary>
        /// Get distance to target (ignore Y axis)
        /// Lấy khoảng cách đến mục tiêu (bỏ qua trục Y)
        /// </summary>
        public static float DistanceXZ(this Vector3 from, Vector3 to)
        {
            Vector3 flatFrom = new Vector3(from.x, 0, from.z);
            Vector3 flatTo = new Vector3(to.x, 0, to.z);
            return Vector3.Distance(flatFrom, flatTo);
        }

        /// <summary>
        /// Clamp angle between min and max
        /// Giới hạn góc giữa min và max
        /// </summary>
        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
    }
}
