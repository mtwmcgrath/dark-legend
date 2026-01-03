using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Hiển thị tên người chơi phía trên đầu / Display player name above head
    /// </summary>
    public class PlayerNameTag : MonoBehaviourPun
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image healthBar;
        [SerializeField] private Canvas nameTagCanvas;

        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 2.5f, 0);
        [SerializeField] private bool showHealthBar = true;
        [SerializeField] private bool alwaysFaceCamera = true;
        [SerializeField] private float maxVisibleDistance = 50f;

        [Header("Color Settings")]
        [SerializeField] private Color friendlyColor = Color.green;
        [SerializeField] private Color enemyColor = Color.red;
        [SerializeField] private Color partyColor = Color.cyan;
        [SerializeField] private Color neutralColor = Color.white;

        private Camera mainCamera;
        private Transform cameraTransform;
        private float currentHealth = 100f;
        private float maxHealth = 100f;

        private void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }

            // Setup canvas
            if (nameTagCanvas != null)
            {
                nameTagCanvas.renderMode = RenderMode.WorldSpace;
                nameTagCanvas.worldCamera = mainCamera;
            }

            // Đặt tên người chơi / Set player name
            if (photonView.IsMine)
            {
                SetPlayerName(PhotonNetwork.NickName);
                SetNameColor(friendlyColor);
            }
            else
            {
                SetPlayerName(photonView.Owner.NickName);
                UpdateNameColor();
            }

            // Ẩn health bar nếu không cần / Hide health bar if not needed
            if (healthBar != null && !showHealthBar)
            {
                healthBar.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (nameTagCanvas == null || cameraTransform == null) return;

            // Đặt vị trí name tag / Set name tag position
            nameTagCanvas.transform.position = transform.position + offset;

            // Luôn quay về phía camera / Always face camera
            if (alwaysFaceCamera)
            {
                nameTagCanvas.transform.rotation = Quaternion.LookRotation(
                    nameTagCanvas.transform.position - cameraTransform.position);
            }

            // Ẩn/hiện dựa trên khoảng cách / Hide/show based on distance
            float distance = Vector3.Distance(transform.position, cameraTransform.position);
            bool shouldShow = distance <= maxVisibleDistance;
            
            if (nameTagCanvas.enabled != shouldShow)
            {
                nameTagCanvas.enabled = shouldShow;
            }
        }

        #region Name Tag

        /// <summary>
        /// Đặt tên người chơi / Set player name
        /// </summary>
        public void SetPlayerName(string playerName)
        {
            if (nameText != null)
            {
                nameText.text = playerName;
            }
        }

        /// <summary>
        /// Đặt màu tên / Set name color
        /// </summary>
        public void SetNameColor(Color color)
        {
            if (nameText != null)
            {
                nameText.color = color;
            }
        }

        /// <summary>
        /// Cập nhật màu tên dựa trên trạng thái / Update name color based on status
        /// </summary>
        private void UpdateNameColor()
        {
            if (photonView.IsMine)
            {
                SetNameColor(friendlyColor);
                return;
            }

            // Kiểm tra party / Check party
            if (PartySystem.Instance != null && PartySystem.Instance.IsInParty())
            {
                var partyMembers = PartySystem.Instance.GetPartyMembers();
                if (partyMembers.Contains(photonView.Owner))
                {
                    SetNameColor(partyColor);
                    return;
                }
            }

            // Kiểm tra PvP / Check PvP
            if (PvPSystem.Instance != null)
            {
                if (PvPSystem.Instance.IsPvPEnabled() || PvPSystem.Instance.IsInPvPZone())
                {
                    SetNameColor(enemyColor);
                    return;
                }
            }

            SetNameColor(neutralColor);
        }

        #endregion

        #region Health Bar

        /// <summary>
        /// Cập nhật health bar / Update health bar
        /// </summary>
        public void UpdateHealthBar(float health, float maxHP)
        {
            if (healthBar == null) return;

            currentHealth = health;
            maxHealth = maxHP;

            float healthPercent = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            healthBar.fillAmount = healthPercent;

            // Đổi màu health bar / Change health bar color
            if (healthPercent > 0.5f)
                healthBar.color = Color.green;
            else if (healthPercent > 0.25f)
                healthBar.color = Color.yellow;
            else
                healthBar.color = Color.red;
        }

        /// <summary>
        /// Hiển thị/ẩn health bar / Show/hide health bar
        /// </summary>
        public void ShowHealthBar(bool show)
        {
            if (healthBar != null)
            {
                healthBar.gameObject.SetActive(show);
            }
        }

        #endregion

        #region Display Settings

        /// <summary>
        /// Đặt offset của name tag / Set name tag offset
        /// </summary>
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
        }

        /// <summary>
        /// Đặt khoảng cách hiển thị / Set visible distance
        /// </summary>
        public void SetMaxVisibleDistance(float distance)
        {
            maxVisibleDistance = distance;
        }

        /// <summary>
        /// Hiển thị/ẩn name tag / Show/hide name tag
        /// </summary>
        public void ShowNameTag(bool show)
        {
            if (nameTagCanvas != null)
            {
                nameTagCanvas.enabled = show;
            }
        }

        #endregion

        #region Level Display

        /// <summary>
        /// Hiển thị level cùng với tên / Display level with name
        /// </summary>
        public void SetPlayerNameWithLevel(string playerName, int level)
        {
            if (nameText != null)
            {
                nameText.text = $"[Lv.{level}] {playerName}";
            }
        }

        /// <summary>
        /// Hiển thị class và level / Display class and level
        /// </summary>
        public void SetPlayerInfo(string playerName, string characterClass, int level)
        {
            if (nameText != null)
            {
                nameText.text = $"[Lv.{level} {characterClass}]\n{playerName}";
            }
        }

        #endregion

        #region RPC Methods

        /// <summary>
        /// RPC để cập nhật tên / RPC to update name
        /// </summary>
        [PunRPC]
        public void RPC_UpdateName(string newName)
        {
            SetPlayerName(newName);
        }

        /// <summary>
        /// RPC để cập nhật health / RPC to update health
        /// </summary>
        [PunRPC]
        public void RPC_UpdateHealth(float health, float maxHP)
        {
            UpdateHealthBar(health, maxHP);
        }

        #endregion
    }
}
