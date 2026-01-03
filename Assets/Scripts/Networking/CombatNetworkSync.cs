using UnityEngine;
using Photon.Pun;
using System.Collections;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Đồng bộ combat, damage, skills / Synchronizes combat, damage, skills
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class CombatNetworkSync : MonoBehaviourPun
    {
        [Header("Combat Settings")]
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private float skillCooldown = 2f;

        private float lastAttackTime;
        private float lastSkillTime;

        // Delegates
        public delegate void OnDamageReceived(int damage, int attackerId);
        public event OnDamageReceived DamageReceivedEvent;

        public delegate void OnDeath(int killerId);
        public event OnDeath DeathEvent;

        public delegate void OnRespawn(Vector3 position);
        public event OnRespawn RespawnEvent;

        #region Attack

        /// <summary>
        /// Thực hiện tấn công / Perform attack
        /// </summary>
        public void Attack(int targetViewID, int damage)
        {
            if (!photonView.IsMine) return;

            if (Time.time - lastAttackTime < attackCooldown)
            {
                Debug.Log("[CombatNetworkSync] Attack on cooldown");
                return;
            }

            lastAttackTime = Time.time;

            // Gửi RPC để tấn công / Send RPC to attack
            photonView.RPC("RPC_Attack", RpcTarget.All, targetViewID, damage, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_Attack(int targetViewID, int damage, int attackerViewID)
        {
            PhotonView targetView = PhotonView.Find(targetViewID);
            if (targetView != null)
            {
                CombatNetworkSync targetCombat = targetView.GetComponent<CombatNetworkSync>();
                if (targetCombat != null)
                {
                    targetCombat.TakeDamage(damage, attackerViewID);
                }
            }

            Debug.Log($"[CombatNetworkSync] Attack from {attackerViewID} to {targetViewID} for {damage} damage");
        }

        #endregion

        #region Damage

        /// <summary>
        /// Nhận damage / Take damage
        /// </summary>
        public void TakeDamage(int damage, int attackerViewID)
        {
            if (!photonView.IsMine) return;

            // Xử lý damage / Handle damage
            Debug.Log($"[CombatNetworkSync] Taking {damage} damage from {attackerViewID}");
            
            DamageReceivedEvent?.Invoke(damage, attackerViewID);

            // Kiểm tra chết / Check death
            // Implement health check logic here
        }

        /// <summary>
        /// Gửi damage đến target / Deal damage to target
        /// </summary>
        public void DealDamage(int targetViewID, int damage)
        {
            if (!photonView.IsMine) return;

            photonView.RPC("RPC_DealDamage", RpcTarget.All, targetViewID, damage, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_DealDamage(int targetViewID, int damage, int attackerViewID)
        {
            if (photonView.ViewID == targetViewID)
            {
                TakeDamage(damage, attackerViewID);
            }
        }

        #endregion

        #region Skills

        /// <summary>
        /// Cast skill / Sử dụng skill
        /// </summary>
        public void CastSkill(int skillID, Vector3 targetPosition)
        {
            if (!photonView.IsMine) return;

            if (Time.time - lastSkillTime < skillCooldown)
            {
                Debug.Log("[CombatNetworkSync] Skill on cooldown");
                return;
            }

            lastSkillTime = Time.time;

            // Gửi RPC để cast skill / Send RPC to cast skill
            photonView.RPC("RPC_CastSkill", RpcTarget.All, skillID, targetPosition, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_CastSkill(int skillID, Vector3 targetPosition, int casterViewID)
        {
            Debug.Log($"[CombatNetworkSync] Player {casterViewID} cast skill {skillID} at {targetPosition}");
            
            // Trigger skill visual effects
            // Implement skill logic here
        }

        /// <summary>
        /// Cast skill với target / Cast skill with target
        /// </summary>
        public void CastSkillOnTarget(int skillID, int targetViewID)
        {
            if (!photonView.IsMine) return;

            if (Time.time - lastSkillTime < skillCooldown)
            {
                Debug.Log("[CombatNetworkSync] Skill on cooldown");
                return;
            }

            lastSkillTime = Time.time;

            photonView.RPC("RPC_CastSkillOnTarget", RpcTarget.All, skillID, targetViewID, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_CastSkillOnTarget(int skillID, int targetViewID, int casterViewID)
        {
            Debug.Log($"[CombatNetworkSync] Player {casterViewID} cast skill {skillID} on {targetViewID}");
            
            // Implement skill logic here
        }

        #endregion

        #region Buffs/Debuffs

        /// <summary>
        /// Áp dụng buff / Apply buff
        /// </summary>
        public void ApplyBuff(int buffID, float duration)
        {
            if (!photonView.IsMine) return;

            photonView.RPC("RPC_ApplyBuff", RpcTarget.All, buffID, duration, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_ApplyBuff(int buffID, float duration, int targetViewID)
        {
            Debug.Log($"[CombatNetworkSync] Buff {buffID} applied to {targetViewID} for {duration} seconds");
            
            // Implement buff logic here
            StartCoroutine(BuffCoroutine(buffID, duration));
        }

        private IEnumerator BuffCoroutine(int buffID, float duration)
        {
            // Apply buff effects
            yield return new WaitForSeconds(duration);
            // Remove buff effects
            
            Debug.Log($"[CombatNetworkSync] Buff {buffID} expired");
        }

        /// <summary>
        /// Áp dụng debuff / Apply debuff
        /// </summary>
        public void ApplyDebuff(int debuffID, float duration, int targetViewID)
        {
            if (!photonView.IsMine) return;

            photonView.RPC("RPC_ApplyDebuff", RpcTarget.All, debuffID, duration, targetViewID);
        }

        [PunRPC]
        private void RPC_ApplyDebuff(int debuffID, float duration, int targetViewID)
        {
            if (photonView.ViewID == targetViewID)
            {
                Debug.Log($"[CombatNetworkSync] Debuff {debuffID} applied for {duration} seconds");
                
                // Implement debuff logic here
                StartCoroutine(DebuffCoroutine(debuffID, duration));
            }
        }

        private IEnumerator DebuffCoroutine(int debuffID, float duration)
        {
            // Apply debuff effects
            yield return new WaitForSeconds(duration);
            // Remove debuff effects
            
            Debug.Log($"[CombatNetworkSync] Debuff {debuffID} expired");
        }

        #endregion

        #region Death & Respawn

        /// <summary>
        /// Xử lý chết / Handle death
        /// </summary>
        public void Die(int killerViewID)
        {
            if (!photonView.IsMine) return;

            photonView.RPC("RPC_Die", RpcTarget.All, killerViewID, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_Die(int killerViewID, int victimViewID)
        {
            Debug.Log($"[CombatNetworkSync] Player {victimViewID} killed by {killerViewID}");
            
            if (photonView.ViewID == victimViewID)
            {
                DeathEvent?.Invoke(killerViewID);
                
                // Tự động respawn sau 5 giây / Auto respawn after 5 seconds
                StartCoroutine(RespawnCoroutine(5f));
            }
        }

        private IEnumerator RespawnCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            
            // Tìm spawn point / Find spawn point
            Vector3 spawnPosition = Vector3.zero; // TODO: Get spawn position from spawn manager
            
            Respawn(spawnPosition);
        }

        /// <summary>
        /// Hồi sinh / Respawn
        /// </summary>
        public void Respawn(Vector3 position)
        {
            if (!photonView.IsMine) return;

            photonView.RPC("RPC_Respawn", RpcTarget.All, position, photonView.ViewID);
        }

        [PunRPC]
        private void RPC_Respawn(Vector3 position, int playerViewID)
        {
            Debug.Log($"[CombatNetworkSync] Player {playerViewID} respawned at {position}");
            
            if (photonView.ViewID == playerViewID)
            {
                transform.position = position;
                RespawnEvent?.Invoke(position);
                
                // Reset stats
                // Implement respawn logic here
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Kiểm tra cooldown / Check cooldown
        /// </summary>
        public bool IsAttackReady()
        {
            return Time.time - lastAttackTime >= attackCooldown;
        }

        public bool IsSkillReady()
        {
            return Time.time - lastSkillTime >= skillCooldown;
        }

        /// <summary>
        /// Reset cooldowns / Đặt lại cooldowns
        /// </summary>
        public void ResetCooldowns()
        {
            lastAttackTime = 0f;
            lastSkillTime = 0f;
        }

        #endregion
    }
}
