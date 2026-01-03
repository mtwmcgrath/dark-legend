using UnityEngine;
using Photon.Pun;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Setup PhotonView cho player / Setup PhotonView for player
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PlayerPhotonView : MonoBehaviourPun
    {
        [Header("Components")]
        [SerializeField] private PlayerNetworkSync networkSync;
        [SerializeField] private CombatNetworkSync combatSync;
        [SerializeField] private PlayerNameTag nameTag;

        [Header("Camera Settings")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private AudioListener audioListener;

        private void Awake()
        {
            // Lấy components / Get components
            if (networkSync == null)
                networkSync = GetComponent<PlayerNetworkSync>();

            if (combatSync == null)
                combatSync = GetComponent<CombatNetworkSync>();

            if (nameTag == null)
                nameTag = GetComponentInChildren<PlayerNameTag>();
        }

        private void Start()
        {
            // Chỉ bật camera và audio listener cho local player
            // Only enable camera and audio listener for local player
            if (photonView.IsMine)
            {
                SetupLocalPlayer();
            }
            else
            {
                SetupRemotePlayer();
            }

            // Setup player properties / Thiết lập player properties
            SetupPlayerProperties();
        }

        private void SetupLocalPlayer()
        {
            // Bật camera / Enable camera
            if (playerCamera != null)
            {
                playerCamera.enabled = true;
                playerCamera.gameObject.SetActive(true);
            }

            // Bật audio listener / Enable audio listener
            if (audioListener != null)
            {
                audioListener.enabled = true;
            }

            Debug.Log("[PlayerPhotonView] Local player setup complete");
        }

        private void SetupRemotePlayer()
        {
            // Tắt camera / Disable camera
            if (playerCamera != null)
            {
                playerCamera.enabled = false;
                playerCamera.gameObject.SetActive(false);
            }

            // Tắt audio listener / Disable audio listener
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }

            Debug.Log($"[PlayerPhotonView] Remote player setup complete: {photonView.Owner.NickName}");
        }

        private void SetupPlayerProperties()
        {
            if (photonView.IsMine)
            {
                // Đặt player properties / Set player properties
                // TODO: Load from player data
                string characterClass = "DarkKnight";
                int level = 1;
                string characterName = PhotonNetwork.NickName;

                if (RoomManager.Instance != null)
                {
                    RoomManager.Instance.SetPlayerProperties(characterClass, level, characterName);
                }
            }
        }

        #region Public Methods

        /// <summary>
        /// Lấy PhotonView / Get PhotonView
        /// </summary>
        public PhotonView GetPhotonView()
        {
            return photonView;
        }

        /// <summary>
        /// Lấy NetworkSync / Get NetworkSync
        /// </summary>
        public PlayerNetworkSync GetNetworkSync()
        {
            return networkSync;
        }

        /// <summary>
        /// Lấy CombatSync / Get CombatSync
        /// </summary>
        public CombatNetworkSync GetCombatSync()
        {
            return combatSync;
        }

        /// <summary>
        /// Lấy NameTag / Get NameTag
        /// </summary>
        public PlayerNameTag GetNameTag()
        {
            return nameTag;
        }

        /// <summary>
        /// Kiểm tra có phải local player không / Check if is local player
        /// </summary>
        public bool IsLocalPlayer()
        {
            return photonView.IsMine;
        }

        #endregion
    }
}
