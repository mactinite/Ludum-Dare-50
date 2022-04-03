using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{

    private ObjectPool objectPool;
    public GameObject prefab;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private void Awake()
    {
        if (TryGetComponent<ObjectPool>(out var pool))
        {
            objectPool = pool;
        }
        else
        {
            objectPool = gameObject.AddComponent<ObjectPool>();
        }
    }

    public void SpawnPrefab()
    {
        SpawnPrefabAt(transform.position);
    }

    //private void OnDisable()
    //{
    //    foreach (GameObject go in spawnedObjects)
    //    {
    //        objectPool.ReturnGameObject(go);
    //    }
    //}

    public void SpawnPrefabAt(Vector3 position)
    {
        var spawnedPrefab = objectPool.GetObject(prefab);
        if (!spawnedObjects.Contains(spawnedPrefab))
        {
            spawnedObjects.Add(spawnedPrefab);
        }
        spawnedPrefab.transform.position = position;

        if (spawnedPrefab.TryGetComponent<RecycleAfterTime>(out var recycleAfterTime))
        {
            recycleAfterTime.poolReference = objectPool;
        }
        
        
    }

}
