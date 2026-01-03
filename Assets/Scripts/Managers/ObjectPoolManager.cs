using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Managers
{
    /// <summary>
    /// Object pool for performance optimization
    /// Object pool để tối ưu hóa hiệu năng
    /// </summary>
    [System.Serializable]
    public class ObjectPool
    {
        public string poolName;
        public GameObject prefab;
        public int poolSize = 20;
        public bool expandable = true;
        
        private Queue<GameObject> availableObjects = new Queue<GameObject>();
        private List<GameObject> allObjects = new List<GameObject>();
        private Transform poolParent;
        
        /// <summary>
        /// Initialize pool
        /// Khởi tạo pool
        /// </summary>
        public void Initialize(Transform parent)
        {
            poolParent = parent;
            
            // Create pool parent
            GameObject poolObj = new GameObject($"Pool_{poolName}");
            poolObj.transform.SetParent(poolParent);
            poolParent = poolObj.transform;
            
            // Pre-instantiate objects
            for (int i = 0; i < poolSize; i++)
            {
                CreateNewObject();
            }
        }
        
        /// <summary>
        /// Create new pooled object
        /// Tạo object mới trong pool
        /// </summary>
        private GameObject CreateNewObject()
        {
            GameObject obj = Object.Instantiate(prefab, poolParent);
            obj.SetActive(false);
            availableObjects.Enqueue(obj);
            allObjects.Add(obj);
            return obj;
        }
        
        /// <summary>
        /// Get object from pool
        /// Lấy object từ pool
        /// </summary>
        public GameObject Get(Vector3 position, Quaternion rotation)
        {
            GameObject obj = null;
            
            if (availableObjects.Count > 0)
            {
                obj = availableObjects.Dequeue();
            }
            else if (expandable)
            {
                obj = CreateNewObject();
            }
            else
            {
                Debug.LogWarning($"Pool {poolName} is empty and not expandable!");
                return null;
            }
            
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            
            return obj;
        }
        
        /// <summary>
        /// Return object to pool
        /// Trả object về pool
        /// </summary>
        public void Return(GameObject obj)
        {
            if (obj == null) return;
            
            obj.SetActive(false);
            obj.transform.SetParent(poolParent);
            availableObjects.Enqueue(obj);
        }
        
        /// <summary>
        /// Clear pool
        /// Xóa pool
        /// </summary>
        public void Clear()
        {
            foreach (GameObject obj in allObjects)
            {
                if (obj != null)
                {
                    Object.Destroy(obj);
                }
            }
            
            availableObjects.Clear();
            allObjects.Clear();
        }
    }
    
    /// <summary>
    /// Object pool manager
    /// Quản lý object pool
    /// </summary>
    public class ObjectPoolManager : Utils.Singleton<ObjectPoolManager>
    {
        [Header("Pools")]
        public List<ObjectPool> pools = new List<ObjectPool>();
        
        private Dictionary<string, ObjectPool> poolDictionary = new Dictionary<string, ObjectPool>();
        
        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }
        
        /// <summary>
        /// Initialize all pools
        /// Khởi tạo tất cả pools
        /// </summary>
        private void InitializePools()
        {
            poolDictionary.Clear();
            
            foreach (ObjectPool pool in pools)
            {
                pool.Initialize(transform);
                poolDictionary[pool.poolName] = pool;
            }
        }
        
        /// <summary>
        /// Create a new pool at runtime
        /// Tạo pool mới trong lúc chạy
        /// </summary>
        public void CreatePool(string poolName, GameObject prefab, int size = 20, bool expandable = true)
        {
            if (poolDictionary.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool {poolName} already exists!");
                return;
            }
            
            ObjectPool newPool = new ObjectPool
            {
                poolName = poolName,
                prefab = prefab,
                poolSize = size,
                expandable = expandable
            };
            
            newPool.Initialize(transform);
            pools.Add(newPool);
            poolDictionary[poolName] = newPool;
        }
        
        /// <summary>
        /// Spawn object from pool
        /// Spawn object từ pool
        /// </summary>
        public GameObject Spawn(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(poolName))
            {
                Debug.LogError($"Pool {poolName} does not exist!");
                return null;
            }
            
            return poolDictionary[poolName].Get(position, rotation);
        }
        
        /// <summary>
        /// Spawn object from pool with default rotation
        /// Spawn object từ pool với rotation mặc định
        /// </summary>
        public GameObject Spawn(string poolName, Vector3 position)
        {
            return Spawn(poolName, position, Quaternion.identity);
        }
        
        /// <summary>
        /// Return object to pool
        /// Trả object về pool
        /// </summary>
        public void Despawn(string poolName, GameObject obj)
        {
            if (!poolDictionary.ContainsKey(poolName))
            {
                Debug.LogWarning($"Pool {poolName} does not exist! Destroying object instead.");
                Destroy(obj);
                return;
            }
            
            poolDictionary[poolName].Return(obj);
        }
        
        /// <summary>
        /// Despawn object after delay
        /// Despawn object sau delay
        /// </summary>
        public void DespawnAfterDelay(string poolName, GameObject obj, float delay)
        {
            StartCoroutine(DespawnCoroutine(poolName, obj, delay));
        }
        
        private System.Collections.IEnumerator DespawnCoroutine(string poolName, GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Despawn(poolName, obj);
        }
        
        /// <summary>
        /// Clear specific pool
        /// Xóa pool cụ thể
        /// </summary>
        public void ClearPool(string poolName)
        {
            if (poolDictionary.ContainsKey(poolName))
            {
                poolDictionary[poolName].Clear();
                poolDictionary.Remove(poolName);
                pools.RemoveAll(p => p.poolName == poolName);
            }
        }
        
        /// <summary>
        /// Clear all pools
        /// Xóa tất cả pools
        /// </summary>
        public void ClearAllPools()
        {
            foreach (ObjectPool pool in pools)
            {
                pool.Clear();
            }
            
            pools.Clear();
            poolDictionary.Clear();
        }
    }
}
