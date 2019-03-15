using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("SpacePool/SpacePool")]
public sealed class SpacePool : MonoBehaviour
{
    public enum StartupPoolMode { Awake, Start, Manually };

    static SpacePool _instance;

    public static SpacePool Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = Object.FindObjectOfType<SpacePool>();
            if (_instance != null)
                return _instance;

            var obj = new GameObject("SpacePool");
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            _instance = obj.AddComponent<SpacePool>();
            return _instance;
        }
    }

    #region Inspector Parameters

    [SerializeField]
    private int SpawnCount = 0;

    public List<PrefabPool> PreRrefabPoolOptions = new List<PrefabPool>();

    #endregion Inspector Parameters
    public StartupPoolMode startupPoolMode;
    bool startupPoolsCreated;

    Dictionary<object, PrefabPool> poolDic = new Dictionary<object, PrefabPool>();
    Dictionary<GameObject, GameObject> spawnedObjects = new Dictionary<GameObject, GameObject>();

    static List<GameObject> tempList = new List<GameObject>();

    void Awake()
    {
        _instance = this;
        if (startupPoolMode == StartupPoolMode.Awake)
            CreateStartupPools();
    }

    void Start()
    {
        if (startupPoolMode == StartupPoolMode.Start)
            CreateStartupPools();
    }

    public static void CreateStartupPools()
    {
        if (!Instance.startupPoolsCreated)
        {
            Instance.startupPoolsCreated = true;
            var pools = Instance.PreRrefabPoolOptions;

            if (pools != null && pools.Count > 0)
            {
                for (int i = 0; i < pools.Count; ++i)
                {
                    PrefabPool pool = pools[i];
                    CreatePool(pool.prefab, pool.preloadAmount);
                    // pool.count = pool.preloadAmount;
                }
            }
        }
    }

    public static PrefabPool CreatePool(GameObject prefab, int initialPoolSize)
    {
        if (prefab != null && !Instance.poolDic.ContainsKey(prefab))
        {
            PrefabPool pool = new PrefabPool
            {
                QueueCache = new Queue(),
            };

            Instance.poolDic.Add(prefab, pool);

            if (initialPoolSize > 0)
            {
                bool active = prefab.activeSelf;
                prefab.SetActive(false);
                Transform parent = Instance.transform;
                while (pool.QueueCache.Count < initialPoolSize)
                {
                    var obj = (GameObject)Object.Instantiate(prefab);
                    obj.transform.parent = parent;
                    pool.QueueCache.Enqueue(obj);
                    SpacePool.Instance.SpawnCount++;
                }
                prefab.SetActive(active);
            }

            return pool;
        }

        return null;
    }

    public static PrefabPool CreatePool<T>(T prefab, int initialPoolSize) where T : Component
    {
        return CreatePool(prefab.gameObject, initialPoolSize);
    }

    public static GameObject FinalSpawn(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, string childName)
    {
        // 是否生成过池
        if (Instance.poolDic.TryGetValue(prefab, out PrefabPool pool))
        {
            GameObject childObj = null;

            // TODO 先查找是否激活中 处理重复名字问题
            //if (!string.IsNullOrEmpty(childName))
            //{
            //    foreach (var spawnObj in Instance.spawnedObjects)
            //    {
            //        if (spawnObj.Key.name == childName)
            //        {
            //            return spawnObj.Key;
            //        }
            //    }
            //}

            if (pool.QueueCache.Count > 0)
            {
                while (childObj == null && pool.QueueCache.Count > 0)
                {
                    childObj = pool.QueueCache.Dequeue() as GameObject;
                    SpacePool.Instance.SpawnCount--;
                }
                if (childObj != null)
                {
                    SetChild(childObj.transform, parent, position, rotation, childName);
                    childObj.SetActive(true);
                    Instance.spawnedObjects.Add(childObj, prefab);
                    return childObj;
                }
            }
            childObj = (GameObject)Object.Instantiate(prefab);
            SetChild(childObj.transform, parent, position, rotation, childName);
            Instance.spawnedObjects.Add(childObj, prefab);
            return childObj;
        }
        else
        {
            Debug.LogFormat("Pool:[{0}] is not include this prefab so New...", prefab.name);

            // Pool is not include this prefab so New...
            CreatePool(prefab, 1);
            return FinalSpawn(prefab, parent, position, rotation, childName);

            //obj = (GameObject)Object.Instantiate(prefab);
            //SetChild(obj.transform, parent, position, rotation, name);
            //return obj;
        }
    }

    static Transform SetChild(Transform trans, Transform parent, Vector3 position, Quaternion rotation, string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            trans.gameObject.name = name;
        }
        trans.parent = parent;
        trans.localPosition = position;
        trans.localRotation = rotation;
        trans.localScale = Vector3.one;
        return trans;
    }

    public static void Recycle<T>(T obj) where T : Component
    {
        Recycle(obj.gameObject);
    }

    public static void Recycle(PrefabPool pool, string name)
    {
        foreach (var obj in Instance.spawnedObjects)
        {
            if (obj.Key.name == name)
            {
                Recycle(obj.Key, obj.Value);
                return;
            }
        }

        Debug.LogErrorFormat("Can't Recycle:{0}", name);
    }

    public static void Recycle(string name)
    {
        Recycle(null, name);
    }

    public static void Recycle(GameObject obj)
    {
        if (Instance.spawnedObjects.TryGetValue(obj, out GameObject prefab))
            Recycle(obj, prefab);
        else
            Object.Destroy(obj);
    }

    static void Recycle(GameObject obj, GameObject prefab)
    {
        Instance.poolDic[prefab].QueueCache.Enqueue(obj);
        SpacePool.Instance.SpawnCount++;

        Instance.spawnedObjects.Remove(obj);
        obj.transform.parent = Instance.transform;
        obj.SetActive(false);
    }

    public static void RecycleAll<T>(T prefab) where T : Component
    {
        RecycleAll(prefab.gameObject);
    }

    public static void RecycleAll(GameObject prefab)
    {
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefab)
                tempList.Add(item.Key);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }

    public static void RecycleAll()
    {
        tempList.AddRange(Instance.spawnedObjects.Keys);
        for (int i = 0; i < tempList.Count; ++i)
            Recycle(tempList[i]);
        tempList.Clear();
    }

    public static bool IsSpawned(GameObject obj)
    {
        return Instance.spawnedObjects.ContainsKey(obj);
    }

    // TODO 测试
    public static bool IsSpawned(string name)
    {
        foreach (var obj in Instance.spawnedObjects)
        {
            if (obj.Key.name == name)
            {
                return true;
            }
        }
        return false;
    }

    public static int CountPooled<T>(T prefab) where T : Component
    {
        return CountPooled(prefab.gameObject);
    }

    public static int CountPooled(GameObject prefab)
    {
        PrefabPool pool;
        if (Instance.poolDic.TryGetValue(prefab, out pool))
            return pool.QueueCache.Count;
        return 0;
    }

    public static int CountSpawned<T>(T prefab) where T : Component
    {
        return CountSpawned(prefab.gameObject);
    }

    public static int CountSpawned(GameObject prefab)
    {
        int count = 0;
        foreach (var instancePrefab in Instance.spawnedObjects.Values)
            if (prefab == instancePrefab)
                ++count;
        return count;
    }

    public static int CountAllPooled()
    {
        int count = 0;
        foreach (var pool in Instance.poolDic.Values)
            count += pool.QueueCache.Count;
        return count;
    }

    public static List<GameObject> GetPooled(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        PrefabPool pool;
        if (Instance.poolDic.TryGetValue(prefab, out pool))
        {
            //todo
            list.AddRange(pool.QueueCache as IEnumerable<GameObject>);
        }
        return list;
    }

    public static List<T> GetPooled<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        PrefabPool pool;
        if (Instance.poolDic.TryGetValue(prefab.gameObject, out pool))
            foreach (GameObject q in pool.QueueCache)
            {
                list.Add(q.GetComponent<T>());
            }

        return list;
    }

    public static List<GameObject> GetSpawned(GameObject prefab, List<GameObject> list, bool appendList)
    {
        if (list == null)
            list = new List<GameObject>();
        if (!appendList)
            list.Clear();
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefab)
                list.Add(item.Key);
        return list;
    }

    public static List<T> GetSpawned<T>(T prefab, List<T> list, bool appendList) where T : Component
    {
        if (list == null)
            list = new List<T>();
        if (!appendList)
            list.Clear();
        var prefabObj = prefab.gameObject;
        foreach (var item in Instance.spawnedObjects)
            if (item.Value == prefabObj)
                list.Add(item.Key.GetComponent<T>());
        return list;
    }

    public static void DestroyPooled(GameObject prefab)
    {
        PrefabPool pool;
        if (Instance.poolDic.TryGetValue(prefab, out pool))
        {
            while (pool.QueueCache.Count > 0)
            {
                GameObject.Destroy(pool.QueueCache.Dequeue() as GameObject);
            }
            pool.QueueCache.Clear();
        }
    }

    public static void DestroyPooled<T>(T prefab) where T : Component
    {
        DestroyPooled(prefab.gameObject);
    }

    public static void DestroyAll(GameObject prefab)
    {
        RecycleAll(prefab);
        DestroyPooled(prefab);
    }

    public static void DestroyAll<T>(T prefab) where T : Component
    {
        DestroyAll(prefab.gameObject);
    }

    #region Spawn
    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, Quaternion rotation, string name) where T : Component
    {
        return FinalSpawn(prefab.gameObject, parent, position, rotation, name).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, string name) where T : Component
    {
        return FinalSpawn(prefab.gameObject, null, position, rotation, name).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Transform parent, Vector3 position, string name) where T : Component
    {
        return FinalSpawn(prefab.gameObject, parent, position, Quaternion.identity, name).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Vector3 position, string name) where T : Component
    {
        return FinalSpawn(prefab.gameObject, null, position, Quaternion.identity, name).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, Transform parent, string name) where T : Component
    {
        return FinalSpawn(prefab.gameObject, parent, Vector3.zero, Quaternion.identity, name).GetComponent<T>();
    }

    public static T Spawn<T>(T prefab, string name) where T : Component
    {
        return FinalSpawn(prefab.gameObject, null, Vector3.zero, Quaternion.identity, name).GetComponent<T>();
    }

    public static GameObject Spawn(GameObject prefab, Transform parent, Vector3 position, string name)
    {
        return FinalSpawn(prefab, parent, position, Quaternion.identity, name);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, string name)
    {
        return FinalSpawn(prefab, null, position, rotation, name);
    }
    public static GameObject Spawn(GameObject prefab, Transform parent, string name)
    {
        return FinalSpawn(prefab, parent, Vector3.zero, Quaternion.identity, name);
    }
    public static GameObject Spawn(GameObject prefab, Vector3 position, string name)
    {
        return FinalSpawn(prefab, null, position, Quaternion.identity, name);
    }
    public static GameObject Spawn(GameObject prefab, string name)
    {
        return FinalSpawn(prefab, null, Vector3.zero, Quaternion.identity, name);
    }
    #endregion
}

[System.Serializable]
public class PrefabPool
{
    public GameObject prefab;
    public int preloadAmount = 1;
    public Queue QueueCache;
}

