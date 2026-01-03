using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Đồng bộ Transform, Animation và Stats của người chơi / Synchronizes player Transform, Animation and Stats
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PlayerNetworkSync : MonoBehaviourPun, IPunObservable
    {
        [Header("Sync Settings")]
        [SerializeField] private bool syncPosition = true;
        [SerializeField] private bool syncRotation = true;
        [SerializeField] private bool syncAnimation = true;
        [SerializeField] private bool syncStats = true;

        [Header("Interpolation Settings")]
        [SerializeField] private float positionLerpSpeed = 10f;
        [SerializeField] private float rotationLerpSpeed = 10f;

        [Header("Lag Compensation")]
        [SerializeField] private bool enableLagCompensation = true;
        [SerializeField] private float maxExtrapolationTime = 0.5f;

        // Network position and rotation
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        
        // Animation sync
        private Animator animator;
        private int currentAnimationState;
        
        // Stats sync
        private float currentHP;
        private float currentMP;
        private int currentLevel;

        // Lag compensation
        private float lastReceiveTime;
        private Vector3 velocity;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }

        private void Update()
        {
            if (!photonView.IsMine)
            {
                // Interpolate position và rotation cho người chơi khác
                // Interpolate position and rotation for other players
                if (syncPosition)
                {
                    if (enableLagCompensation)
                    {
                        // Extrapolation để dự đoán vị trí / Extrapolation to predict position
                        float extrapolationTime = Time.time - lastReceiveTime;
                        if (extrapolationTime < maxExtrapolationTime)
                        {
                            Vector3 extrapolatedPosition = networkPosition + velocity * extrapolationTime;
                            transform.position = Vector3.Lerp(transform.position, extrapolatedPosition, 
                                Time.deltaTime * positionLerpSpeed);
                        }
                        else
                        {
                            transform.position = Vector3.Lerp(transform.position, networkPosition, 
                                Time.deltaTime * positionLerpSpeed);
                        }
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(transform.position, networkPosition, 
                            Time.deltaTime * positionLerpSpeed);
                    }
                }

                if (syncRotation)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, 
                        Time.deltaTime * rotationLerpSpeed);
                }
            }
        }

        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Gửi dữ liệu từ local player / Send data from local player
                if (syncPosition)
                {
                    stream.SendNext(transform.position);
                    if (enableLagCompensation)
                    {
                        // Tính velocity / Calculate velocity
                        Vector3 currentVelocity = (transform.position - networkPosition) / Time.deltaTime;
                        stream.SendNext(currentVelocity);
                    }
                }

                if (syncRotation)
                {
                    stream.SendNext(transform.rotation);
                }

                if (syncAnimation && animator != null)
                {
                    stream.SendNext(currentAnimationState);
                }

                if (syncStats)
                {
                    stream.SendNext(currentHP);
                    stream.SendNext(currentMP);
                    stream.SendNext(currentLevel);
                }
            }
            else
            {
                // Nhận dữ liệu từ người chơi khác / Receive data from other players
                if (syncPosition)
                {
                    networkPosition = (Vector3)stream.ReceiveNext();
                    if (enableLagCompensation)
                    {
                        velocity = (Vector3)stream.ReceiveNext();
                    }
                    lastReceiveTime = Time.time;
                }

                if (syncRotation)
                {
                    networkRotation = (Quaternion)stream.ReceiveNext();
                }

                if (syncAnimation && animator != null)
                {
                    int animState = (int)stream.ReceiveNext();
                    if (animState != currentAnimationState)
                    {
                        currentAnimationState = animState;
                        // Play animation state
                        // animator.Play(currentAnimationState);
                    }
                }

                if (syncStats)
                {
                    currentHP = (float)stream.ReceiveNext();
                    currentMP = (float)stream.ReceiveNext();
                    currentLevel = (int)stream.ReceiveNext();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cập nhật stats để đồng bộ / Update stats to sync
        /// </summary>
        public void UpdateStats(float hp, float mp, int level)
        {
            if (photonView.IsMine)
            {
                currentHP = hp;
                currentMP = mp;
                currentLevel = level;
            }
        }

        /// <summary>
        /// Cập nhật animation state để đồng bộ / Update animation state to sync
        /// </summary>
        public void UpdateAnimationState(int stateHash)
        {
            if (photonView.IsMine)
            {
                currentAnimationState = stateHash;
            }
        }

        /// <summary>
        /// Lấy HP hiện tại / Get current HP
        /// </summary>
        public float GetCurrentHP()
        {
            return currentHP;
        }

        /// <summary>
        /// Lấy MP hiện tại / Get current MP
        /// </summary>
        public float GetCurrentMP()
        {
            return currentMP;
        }

        /// <summary>
        /// Lấy level hiện tại / Get current level
        /// </summary>
        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        /// <summary>
        /// Teleport người chơi đến vị trí mới / Teleport player to new position
        /// </summary>
        public void Teleport(Vector3 position)
        {
            if (photonView.IsMine)
            {
                transform.position = position;
                networkPosition = position;
                
                // Gửi RPC để cập nhật vị trí cho tất cả clients
                // Send RPC to update position for all clients
                photonView.RPC("RPC_Teleport", RpcTarget.Others, position);
            }
        }

        [PunRPC]
        private void RPC_Teleport(Vector3 position)
        {
            transform.position = position;
            networkPosition = position;
        }

        /// <summary>
        /// Đồng bộ vị trí ngay lập tức / Sync position immediately
        /// </summary>
        public void SyncPositionImmediate(Vector3 position)
        {
            if (photonView.IsMine)
            {
                transform.position = position;
                networkPosition = position;
            }
        }

        #endregion

        #region Owner Transfer

        /// <summary>
        /// Request quyền sở hữu object / Request object ownership
        /// </summary>
        public void RequestOwnership()
        {
            if (!photonView.IsMine)
            {
                photonView.RequestOwnership();
            }
        }

        /// <summary>
        /// Chuyển quyền sở hữu cho player khác / Transfer ownership to another player
        /// </summary>
        public void TransferOwnership(Photon.Realtime.Player newOwner)
        {
            if (photonView.IsMine)
            {
                photonView.TransferOwnership(newOwner);
            }
        }

        #endregion
    }
}
