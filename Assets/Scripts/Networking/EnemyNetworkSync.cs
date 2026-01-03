using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Đồng bộ monsters/enemies giữa các clients / Synchronizes monsters/enemies between clients
    /// Master Client quản lý AI, clients khác chỉ nhận thông tin / Master Client manages AI, other clients receive info
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class EnemyNetworkSync : MonoBehaviourPun, IPunObservable
    {
        [Header("Sync Settings")]
        [SerializeField] private bool syncPosition = true;
        [SerializeField] private bool syncRotation = true;
        [SerializeField] private bool syncHealth = true;
        [SerializeField] private bool syncState = true;

        [Header("Interpolation")]
        [SerializeField] private float positionLerpSpeed = 5f;
        [SerializeField] private float rotationLerpSpeed = 5f;

        // Network data
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private float networkHealth;
        private int networkState; // 0 = Idle, 1 = Patrol, 2 = Chase, 3 = Attack

        // Enemy data
        private float currentHealth;
        private float maxHealth = 100f;
        private int currentState = 0;

        private void Awake()
        {
            networkPosition = transform.position;
            networkRotation = transform.rotation;
            currentHealth = maxHealth;
            networkHealth = currentHealth;
        }

        private void Update()
        {
            // Chỉ Master Client mới điều khiển AI
            // Only Master Client controls AI
            if (!PhotonNetwork.IsMasterClient)
            {
                // Clients khác chỉ interpolate position và rotation
                // Other clients only interpolate position and rotation
                if (syncPosition)
                {
                    transform.position = Vector3.Lerp(transform.position, networkPosition, 
                        Time.deltaTime * positionLerpSpeed);
                }

                if (syncRotation)
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, 
                        Time.deltaTime * rotationLerpSpeed);
                }

                if (syncHealth)
                {
                    currentHealth = networkHealth;
                }

                if (syncState)
                {
                    currentState = networkState;
                }
            }
        }

        #region IPunObservable Implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Master Client gửi dữ liệu / Master Client sends data
                if (syncPosition)
                {
                    stream.SendNext(transform.position);
                }

                if (syncRotation)
                {
                    stream.SendNext(transform.rotation);
                }

                if (syncHealth)
                {
                    stream.SendNext(currentHealth);
                }

                if (syncState)
                {
                    stream.SendNext(currentState);
                }
            }
            else
            {
                // Clients nhận dữ liệu / Clients receive data
                if (syncPosition)
                {
                    networkPosition = (Vector3)stream.ReceiveNext();
                }

                if (syncRotation)
                {
                    networkRotation = (Quaternion)stream.ReceiveNext();
                }

                if (syncHealth)
                {
                    networkHealth = (float)stream.ReceiveNext();
                }

                if (syncState)
                {
                    networkState = (int)stream.ReceiveNext();
                }
            }
        }

        #endregion

        #region Combat

        /// <summary>
        /// Nhận damage / Take damage
        /// </summary>
        public void TakeDamage(int damage, int attackerViewID)
        {
            // Chỉ Master Client xử lý damage / Only Master Client handles damage
            if (!PhotonNetwork.IsMasterClient) return;

            currentHealth -= damage;
            Debug.Log($"[EnemyNetworkSync] Enemy took {damage} damage. Health: {currentHealth}/{maxHealth}");

            // Gửi RPC để hiển thị damage effect trên tất cả clients
            // Send RPC to show damage effect on all clients
            photonView.RPC("RPC_ShowDamage", RpcTarget.All, damage);

            if (currentHealth <= 0)
            {
                Die(attackerViewID);
            }
        }

        [PunRPC]
        private void RPC_ShowDamage(int damage)
        {
            // Hiển thị damage text / Show damage text
            Debug.Log($"[EnemyNetworkSync] Show damage: {damage}");
            // TODO: Implement damage text display
        }

        /// <summary>
        /// Tấn công player / Attack player
        /// </summary>
        public void AttackPlayer(int targetViewID, int damage)
        {
            // Chỉ Master Client có thể tấn công / Only Master Client can attack
            if (!PhotonNetwork.IsMasterClient) return;

            photonView.RPC("RPC_AttackPlayer", RpcTarget.All, targetViewID, damage);
        }

        [PunRPC]
        private void RPC_AttackPlayer(int targetViewID, int damage)
        {
            PhotonView targetView = PhotonView.Find(targetViewID);
            if (targetView != null)
            {
                CombatNetworkSync targetCombat = targetView.GetComponent<CombatNetworkSync>();
                if (targetCombat != null)
                {
                    targetCombat.TakeDamage(damage, photonView.ViewID);
                }
            }

            Debug.Log($"[EnemyNetworkSync] Enemy attacked player {targetViewID} for {damage} damage");
        }

        #endregion

        #region State Management

        /// <summary>
        /// Đổi state / Change state
        /// </summary>
        public void ChangeState(int newState)
        {
            // Chỉ Master Client có thể đổi state / Only Master Client can change state
            if (!PhotonNetwork.IsMasterClient) return;

            currentState = newState;
            photonView.RPC("RPC_ChangeState", RpcTarget.Others, newState);
        }

        [PunRPC]
        private void RPC_ChangeState(int newState)
        {
            currentState = newState;
            Debug.Log($"[EnemyNetworkSync] Enemy state changed to: {newState}");
        }

        #endregion

        #region Death & Respawn

        /// <summary>
        /// Xử lý chết / Handle death
        /// </summary>
        private void Die(int killerViewID)
        {
            // Chỉ Master Client xử lý chết / Only Master Client handles death
            if (!PhotonNetwork.IsMasterClient) return;

            Debug.Log($"[EnemyNetworkSync] Enemy killed by player {killerViewID}");

            photonView.RPC("RPC_Die", RpcTarget.All, killerViewID);

            // Drop loot / Rơi đồ
            DropLoot(killerViewID);

            // Respawn sau một khoảng thời gian / Respawn after some time
            StartCoroutine(RespawnCoroutine(30f));
        }

        [PunRPC]
        private void RPC_Die(int killerViewID)
        {
            Debug.Log($"[EnemyNetworkSync] Enemy died. Killer: {killerViewID}");
            
            // Play death animation / Chơi animation chết
            // TODO: Implement death animation
            
            // Disable enemy / Vô hiệu hóa enemy
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Drop loot cho killer / Drop loot for killer
        /// </summary>
        private void DropLoot(int killerViewID)
        {
            // Chỉ Master Client drop loot / Only Master Client drops loot
            if (!PhotonNetwork.IsMasterClient) return;

            Debug.Log($"[EnemyNetworkSync] Dropping loot for player {killerViewID}");
            
            // TODO: Implement loot drop system
            photonView.RPC("RPC_DropLoot", RpcTarget.All, killerViewID, transform.position);
        }

        [PunRPC]
        private void RPC_DropLoot(int killerViewID, Vector3 position)
        {
            Debug.Log($"[EnemyNetworkSync] Loot dropped at {position} for player {killerViewID}");
            // TODO: Spawn loot items
        }

        /// <summary>
        /// Hồi sinh / Respawn
        /// </summary>
        private IEnumerator RespawnCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            // Chỉ Master Client respawn / Only Master Client respawns
            if (!PhotonNetwork.IsMasterClient) yield break;

            currentHealth = maxHealth;
            currentState = 0; // Idle state

            photonView.RPC("RPC_Respawn", RpcTarget.All, transform.position);
        }

        [PunRPC]
        private void RPC_Respawn(Vector3 position)
        {
            Debug.Log($"[EnemyNetworkSync] Enemy respawned at {position}");
            
            transform.position = position;
            currentHealth = maxHealth;
            currentState = 0;
            
            gameObject.SetActive(true);
        }

        #endregion

        #region Spawn Management

        /// <summary>
        /// Spawn enemy (chỉ Master Client) / Spawn enemy (Master Client only)
        /// </summary>
        public static GameObject SpawnEnemy(string prefabName, Vector3 position, Quaternion rotation)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("[EnemyNetworkSync] Only Master Client can spawn enemies");
                return null;
            }

            GameObject enemy = PhotonNetwork.Instantiate(prefabName, position, rotation);
            Debug.Log($"[EnemyNetworkSync] Enemy spawned: {prefabName} at {position}");
            
            return enemy;
        }

        /// <summary>
        /// Destroy enemy (chỉ Master Client) / Destroy enemy (Master Client only)
        /// </summary>
        public void DestroyEnemy()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("[EnemyNetworkSync] Only Master Client can destroy enemies");
                return;
            }

            PhotonNetwork.Destroy(gameObject);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Lấy health hiện tại / Get current health
        /// </summary>
        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        /// <summary>
        /// Lấy max health / Get max health
        /// </summary>
        public float GetMaxHealth()
        {
            return maxHealth;
        }

        /// <summary>
        /// Lấy state hiện tại / Get current state
        /// </summary>
        public int GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Đặt max health / Set max health
        /// </summary>
        public void SetMaxHealth(float health)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                maxHealth = health;
                currentHealth = health;
            }
        }

        #endregion
    }
}
