using System.Collections.Generic;
using UnityEngine;

public static class SpacePoolExtensions
{
	public static PrefabPool CreatePool<T>(this T prefab) where T : Component
	{
        return SpacePool.CreatePool(prefab, 0);
	}

	public static PrefabPool CreatePool<T>(this T prefab, int initialPoolSize) where T : Component
	{
        return SpacePool.CreatePool(prefab, initialPoolSize);
	}

	public static PrefabPool CreatePool(this GameObject prefab)
	{
		return SpacePool.CreatePool(prefab, 0);
	}

	public static PrefabPool CreatePool(this GameObject prefab, int initialPoolSize)
	{
        return SpacePool.CreatePool(prefab, initialPoolSize);
	}

    //public static bool IsSpawned(this PrefabPool pool, string childName)
    //{
    //    return SpacePool.IsSpawned(childName);
    //}

    public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, Quaternion rotation, string name = "") where T : Component
	{
		return SpacePool.Spawn(prefab, parent, position, rotation, name);
	}

	public static T Spawn<T>(this T prefab, Vector3 position, Quaternion rotation, string name = "") where T : Component
	{
		return SpacePool.Spawn(prefab, null, position, rotation, name);
	}

	public static T Spawn<T>(this T prefab, Transform parent, Vector3 position, string name = "") where T : Component
	{
		return SpacePool.Spawn(prefab, parent, position, Quaternion.identity, name);
	}

	public static T Spawn<T>(this T prefab, Vector3 position, string name = "") where T : Component
	{
		return SpacePool.Spawn(prefab, null, position, Quaternion.identity, name);
	}

	public static T Spawn<T>(this T prefab, Transform parent, string name = "") where T : Component
	{
		return SpacePool.Spawn(prefab, parent, Vector3.zero, Quaternion.identity, name);
	}
	public static T Spawn<T>(this T prefab, string name = "") where T : Component
	{
		return SpacePool.Spawn(prefab, null, Vector3.zero, Quaternion.identity, name);
	}

	public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, Quaternion rotation, string name = "")
	{
		return SpacePool.FinalSpawn(prefab, parent, position, rotation, name);
	}

	public static GameObject Spawn(this GameObject prefab, Vector3 position, Quaternion rotation, string name = "")
	{
		return SpacePool.FinalSpawn(prefab, null, position, rotation, name);
	}
	public static GameObject Spawn(this GameObject prefab, Transform parent, Vector3 position, string name = "")
	{
		return SpacePool.FinalSpawn(prefab, parent, position, Quaternion.identity, name);
	}
	public static GameObject Spawn(this GameObject prefab, Vector3 position, string name = "")
	{
		return SpacePool.FinalSpawn(prefab, null, position, Quaternion.identity, name);
	}

	public static GameObject Spawn(this GameObject prefab, Transform parent, string name = "")
	{
		return SpacePool.FinalSpawn(prefab, parent, Vector3.zero, Quaternion.identity, name);
	}

	public static GameObject Spawn(this GameObject prefab, string name = "")
	{
		return SpacePool.FinalSpawn(prefab, null, Vector3.zero, Quaternion.identity, name);
	}
	
	public static void Recycle<T>(this T obj) where T : Component
	{
		SpacePool.Recycle(obj);
	}

    public static void Recycle(this PrefabPool pool, string name)
    {
        SpacePool.Recycle(pool, name);
    }

    public static void Recycle(this GameObject obj)
	{
		SpacePool.Recycle(obj);
	}

    public static void Recycle(this GameObject obj, string name)
    {
        SpacePool.Recycle( name);
    }

    public static void RecycleAll<T>(this T prefab) where T : Component
	{
		SpacePool.RecycleAll(prefab);
	}

	public static void RecycleAll(this GameObject prefab)
	{
		SpacePool.RecycleAll(prefab);
	}

	public static int CountPooled<T>(this T prefab) where T : Component
	{
		return SpacePool.CountPooled(prefab);
	}
	public static int CountPooled(this GameObject prefab)
	{
		return SpacePool.CountPooled(prefab);
	}

	public static int CountSpawned<T>(this T prefab) where T : Component
	{
		return SpacePool.CountSpawned(prefab);
	}
	public static int CountSpawned(this GameObject prefab)
	{
		return SpacePool.CountSpawned(prefab);
	}

	public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list, bool appendList)
	{
		return SpacePool.GetSpawned(prefab, list, appendList);
	}

	public static List<GameObject> GetSpawned(this GameObject prefab, List<GameObject> list)
	{
		return SpacePool.GetSpawned(prefab, list, false);
	}

	public static List<GameObject> GetSpawned(this GameObject prefab)
	{
		return SpacePool.GetSpawned(prefab, null, false);
	}
	public static List<T> GetSpawned<T>(this T prefab, List<T> list, bool appendList) where T : Component
	{
		return SpacePool.GetSpawned(prefab, list, appendList);
	}

	public static List<T> GetSpawned<T>(this T prefab, List<T> list) where T : Component
	{
		return SpacePool.GetSpawned(prefab, list, false);
	}
	public static List<T> GetSpawned<T>(this T prefab) where T : Component
	{
		return SpacePool.GetSpawned(prefab, null, false);
	}

	public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list, bool appendList)
	{
		return SpacePool.GetPooled(prefab, list, appendList);
	}
	public static List<GameObject> GetPooled(this GameObject prefab, List<GameObject> list)
	{
		return SpacePool.GetPooled(prefab, list, false);
	}
	public static List<GameObject> GetPooled(this GameObject prefab)
	{
		return SpacePool.GetPooled(prefab, null, false);
	}
	public static List<T> GetPooled<T>(this T prefab, List<T> list, bool appendList) where T : Component
	{
		return SpacePool.GetPooled(prefab, list, appendList);
	}
	public static List<T> GetPooled<T>(this T prefab, List<T> list) where T : Component
	{
		return SpacePool.GetPooled(prefab, list, false);
	}
	public static List<T> GetPooled<T>(this T prefab) where T : Component
	{
		return SpacePool.GetPooled(prefab, null, false);
	}

	public static void DestroyPooled(this GameObject prefab)
	{
		SpacePool.DestroyPooled(prefab);
	}
	public static void DestroyPooled<T>(this T prefab) where T : Component
	{
		SpacePool.DestroyPooled(prefab.gameObject);
	}

	public static void DestroyAll(this GameObject prefab)
	{
		SpacePool.DestroyAll(prefab);
	}

	public static void DestroyAll<T>(this T prefab) where T : Component
	{
		SpacePool.DestroyAll(prefab.gameObject);
	}
}
