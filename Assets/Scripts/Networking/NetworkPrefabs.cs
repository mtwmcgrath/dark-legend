using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Networking
{
    /// <summary>
    /// Quản lý network prefabs cho spawning / Manages network prefabs for spawning
    /// </summary>
    [CreateAssetMenu(fileName = "NetworkPrefabs", menuName = "DarkLegend/Networking/Network Prefabs")]
    public class NetworkPrefabs : ScriptableObject
    {
        [System.Serializable]
        public class PrefabEntry
        {
            public string prefabName;
            public GameObject prefab;
            public string description;
        }

        [Header("Player Prefabs")]
        [SerializeField] private List<PrefabEntry> playerPrefabs = new List<PrefabEntry>();

        [Header("Enemy Prefabs")]
        [SerializeField] private List<PrefabEntry> enemyPrefabs = new List<PrefabEntry>();

        [Header("Item Prefabs")]
        [SerializeField] private List<PrefabEntry> itemPrefabs = new List<PrefabEntry>();

        [Header("Effect Prefabs")]
        [SerializeField] private List<PrefabEntry> effectPrefabs = new List<PrefabEntry>();

        #region Player Prefabs

        /// <summary>
        /// Lấy player prefab theo tên / Get player prefab by name
        /// </summary>
        public GameObject GetPlayerPrefab(string prefabName)
        {
            return GetPrefabFromList(playerPrefabs, prefabName);
        }

        /// <summary>
        /// Lấy tất cả player prefabs / Get all player prefabs
        /// </summary>
        public List<PrefabEntry> GetAllPlayerPrefabs()
        {
            return new List<PrefabEntry>(playerPrefabs);
        }

        /// <summary>
        /// Thêm player prefab / Add player prefab
        /// </summary>
        public void AddPlayerPrefab(string name, GameObject prefab, string description = "")
        {
            AddPrefabToList(playerPrefabs, name, prefab, description);
        }

        #endregion

        #region Enemy Prefabs

        /// <summary>
        /// Lấy enemy prefab theo tên / Get enemy prefab by name
        /// </summary>
        public GameObject GetEnemyPrefab(string prefabName)
        {
            return GetPrefabFromList(enemyPrefabs, prefabName);
        }

        /// <summary>
        /// Lấy tất cả enemy prefabs / Get all enemy prefabs
        /// </summary>
        public List<PrefabEntry> GetAllEnemyPrefabs()
        {
            return new List<PrefabEntry>(enemyPrefabs);
        }

        /// <summary>
        /// Thêm enemy prefab / Add enemy prefab
        /// </summary>
        public void AddEnemyPrefab(string name, GameObject prefab, string description = "")
        {
            AddPrefabToList(enemyPrefabs, name, prefab, description);
        }

        #endregion

        #region Item Prefabs

        /// <summary>
        /// Lấy item prefab theo tên / Get item prefab by name
        /// </summary>
        public GameObject GetItemPrefab(string prefabName)
        {
            return GetPrefabFromList(itemPrefabs, prefabName);
        }

        /// <summary>
        /// Lấy tất cả item prefabs / Get all item prefabs
        /// </summary>
        public List<PrefabEntry> GetAllItemPrefabs()
        {
            return new List<PrefabEntry>(itemPrefabs);
        }

        /// <summary>
        /// Thêm item prefab / Add item prefab
        /// </summary>
        public void AddItemPrefab(string name, GameObject prefab, string description = "")
        {
            AddPrefabToList(itemPrefabs, name, prefab, description);
        }

        #endregion

        #region Effect Prefabs

        /// <summary>
        /// Lấy effect prefab theo tên / Get effect prefab by name
        /// </summary>
        public GameObject GetEffectPrefab(string prefabName)
        {
            return GetPrefabFromList(effectPrefabs, prefabName);
        }

        /// <summary>
        /// Lấy tất cả effect prefabs / Get all effect prefabs
        /// </summary>
        public List<PrefabEntry> GetAllEffectPrefabs()
        {
            return new List<PrefabEntry>(effectPrefabs);
        }

        /// <summary>
        /// Thêm effect prefab / Add effect prefab
        /// </summary>
        public void AddEffectPrefab(string name, GameObject prefab, string description = "")
        {
            AddPrefabToList(effectPrefabs, name, prefab, description);
        }

        #endregion

        #region Helper Methods

        private GameObject GetPrefabFromList(List<PrefabEntry> list, string prefabName)
        {
            foreach (PrefabEntry entry in list)
            {
                if (entry.prefabName == prefabName)
                {
                    return entry.prefab;
                }
            }

            Debug.LogWarning($"[NetworkPrefabs] Prefab not found: {prefabName}");
            return null;
        }

        private void AddPrefabToList(List<PrefabEntry> list, string name, GameObject prefab, string description)
        {
            // Kiểm tra xem prefab đã tồn tại chưa / Check if prefab already exists
            foreach (PrefabEntry entry in list)
            {
                if (entry.prefabName == name)
                {
                    Debug.LogWarning($"[NetworkPrefabs] Prefab already exists: {name}");
                    return;
                }
            }

            PrefabEntry newEntry = new PrefabEntry
            {
                prefabName = name,
                prefab = prefab,
                description = description
            };

            list.Add(newEntry);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Kiểm tra prefabs hợp lệ / Validate prefabs
        /// </summary>
        public void ValidatePrefabs()
        {
            Debug.Log("[NetworkPrefabs] Validating prefabs...");

            ValidatePrefabList(playerPrefabs, "Player");
            ValidatePrefabList(enemyPrefabs, "Enemy");
            ValidatePrefabList(itemPrefabs, "Item");
            ValidatePrefabList(effectPrefabs, "Effect");

            Debug.Log("[NetworkPrefabs] Validation complete");
        }

        private void ValidatePrefabList(List<PrefabEntry> list, string category)
        {
            int validCount = 0;
            int invalidCount = 0;

            foreach (PrefabEntry entry in list)
            {
                if (entry.prefab == null)
                {
                    Debug.LogWarning($"[NetworkPrefabs] {category} prefab is null: {entry.prefabName}");
                    invalidCount++;
                }
                else if (!entry.prefab.GetComponent<Photon.Pun.PhotonView>())
                {
                    Debug.LogWarning($"[NetworkPrefabs] {category} prefab missing PhotonView: {entry.prefabName}");
                    invalidCount++;
                }
                else
                {
                    validCount++;
                }
            }

            Debug.Log($"[NetworkPrefabs] {category} prefabs - Valid: {validCount}, Invalid: {invalidCount}");
        }

        #endregion
    }
}
