//using System.Collections.Generic;
//using UnityEngine;

//namespace Game.Pool
//{
//    class Pool
//    {
//        public string PoolName = "";
//        public Transform Parent;
//        public GameObject Child;
//        public List<GameObject> ActiveChildLst = new List<GameObject>();
//        public List<GameObject> WaitChildLst = new List<GameObject>();
//    }

//    public static class SpacePool
//    {
//        Dictionary<string, Pool> poolDic = new Dictionary<string, Pool>();

//        public Pool CreatePool(string poolName, GameObject child)
//        {
//            if (poolDic.ContainsKey(poolName))
//            {
//                throw new System.SystemException("PoolName is Repeat!");
//            }

//            GameObject parent = new GameObject(poolName);
//            parent.transform.position = Vector3.zero;

//            Pool newPool = new Pool
//            {
//                Parent = parent.transform,
//                PoolName = poolName,
//                Child = child,
//            };

//            poolDic[poolName] = newPool;
//            Debug.LogFormat("Create New Pool:{0}", poolName);

//            return newPool;
//        }

//        // 取出
//        public GameObject Spawn(string poolName, string ifNullCreateNewChildName = "")
//        {
//            Pool pool;
//            if (poolDic.TryGetValue(poolName, out pool))
//            {
//                GameObject activeChild = pool.ActiveChildLst.Find(x => x.name == ifNullCreateNewChildName);
//                if (activeChild != null)
//                {
//                    return activeChild;
//                }

//                foreach (var obj in pool.WaitChildLst)
//                {
//                    if (!obj.activeSelf)
//                    {
//                        obj.name = ifNullCreateNewChildName;
//                        pool.ActiveChildLst.Add(obj);
//                        pool.WaitChildLst.Remove(obj);
//                        return obj;
//                    }
//                }

//                GameObject child = CreateChild(pool, ifNullCreateNewChildName);
//                pool.ActiveChildLst.Add(child);
//                return child;
//            }
//            else
//            {
//                throw new System.NullReferenceException("Pool is Null " + poolName);
//            }
//        }

//        // 回收
//        public void Recycle(string poolName, string childName)
//        {
//            Pool pool;
//            if (poolDic.TryGetValue(poolName, out pool))
//            {
//                GameObject activeChild = pool.ActiveChildLst.Find(x => x.name == childName);

//                if (activeChild != null)
//                {
//                    activeChild.SetActive(false);
//                    pool.ActiveChildLst.Remove(activeChild);
//                    pool.WaitChildLst.Add(activeChild);
//                }
//                else
//                {
//                    Debug.LogErrorFormat("Cant Recycle in {0}, childName is {1}", poolName, childName);
//                }
//            }
//            else
//            {
//                throw new System.NullReferenceException("Pool is Null" + poolName);
//            }
//        }

//        GameObject CreateChild(Pool pool, string newChildName = "")
//        {
//            GameObject child = GameObject.Instantiate(pool.Child, pool.Parent);
//            child.transform.rotation = Quaternion.identity;
//            child.transform.localScale = Vector3.one;

//            string childName = newChildName == "" ? pool.Child.name : newChildName;
//            child.name = childName;

//            return child;
//        }
//    }
//}
