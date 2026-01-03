using UnityEngine;

namespace DarkLegend.Mobile.Input
{
    /// <summary>
    /// Interact button (NPC, items, objects)
    /// Nút tương tác (NPC, items, objects)
    /// </summary>
    public class InteractButton : TouchButton
    {
        [Header("Interact Settings")]
        public float interactRange = 2f;
        public LayerMask interactableLayer;

        private GameObject nearestInteractable;
        private bool hasInteractable = false;

        protected override void Awake()
        {
            base.Awake();
            buttonName = "Interact";
            pcEquivalent = KeyCode.E;

            // Hide button initially
            SetVisibility(false);
        }

        protected override void Update()
        {
            base.Update();

            // Check for nearby interactables
            CheckForInteractables();
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (!hasInteractable)
                return;

            base.OnPointerDown(eventData);

            PerformInteract();
        }

        /// <summary>
        /// Check for nearby interactables
        /// Kiểm tra các đối tượng có thể tương tác gần
        /// </summary>
        private void CheckForInteractables()
        {
            // TODO: Replace with actual player position
            Vector3 playerPosition = Vector3.zero;
            
            // Find nearest interactable
            Collider[] colliders = Physics.OverlapSphere(playerPosition, interactRange, interactableLayer);
            
            if (colliders.Length > 0)
            {
                // Find closest one
                float closestDistance = float.MaxValue;
                GameObject closest = null;

                foreach (Collider col in colliders)
                {
                    float distance = Vector3.Distance(playerPosition, col.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closest = col.gameObject;
                    }
                }

                nearestInteractable = closest;
                hasInteractable = true;
                SetVisibility(true);
            }
            else
            {
                nearestInteractable = null;
                hasInteractable = false;
                SetVisibility(false);
            }
        }

        /// <summary>
        /// Perform interact
        /// Thực hiện tương tác
        /// </summary>
        private void PerformInteract()
        {
            if (nearestInteractable == null)
                return;

            Debug.Log($"[InteractButton] Interacting with {nearestInteractable.name}");

            // Trigger haptic feedback
            if (useHaptic)
            {
                TriggerHaptic();
            }

            // TODO: Call interaction system
            // IInteractable interactable = nearestInteractable.GetComponent<IInteractable>();
            // if (interactable != null)
            // {
            //     interactable.OnInteract();
            // }
        }

        /// <summary>
        /// Set button visibility
        /// Đặt hiển thị nút
        /// </summary>
        private void SetVisibility(bool visible)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = visible ? 1f : 0f;
                canvasGroup.interactable = visible;
            }
            else
            {
                gameObject.SetActive(visible);
            }
        }
    }
}
