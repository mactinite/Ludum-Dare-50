using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private Transform poolParent;

    [System.Serializable]
    public struct PoolInitInfo
    {
        public int amount;
        public GameObject prefab;
    }
    [SerializeField]
    private PoolInitInfo[] poolConfig;
    private void Start()
    {
        string parentName = "Object_Pool ---- Inactive Items";
        var go = GameObject.Find(parentName);
        if (go != null)
        {
            poolParent = go.transform;
        }
        else
        {
            poolParent = new GameObject(parentName).transform;
        }
        if (poolConfig != null)
        {
            for (int i = 0; i < poolConfig.Length; i++)
            {
                for (int j = 0; j < poolConfig[i].amount; j++)
                {

                    GameObject _object = CreateNewGameObject(poolConfig[i].prefab);
                    ReturnGameObject(_object);

                }
            }
        }

    }

    public GameObject GetObject(GameObject gameObject)
    {
        if (objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objList))
        {
            if (objList.Count == 0)
            {
                return CreateNewGameObject(gameObject);
            }
            else
            {
                GameObject _object = objList.Dequeue();
                _object.SetActive(true);
                _object.transform.SetParent(null);
                return _object;
            }
        }
        else
            return CreateNewGameObject(gameObject);
    }

    private GameObject CreateNewGameObject(GameObject gameObject)
    {
        GameObject newGO = Instantiate(gameObject);
        newGO.name = gameObject.name;
        return newGO;
    }

    public void ReturnGameObject(GameObject gameObject)
    {
        if (gameObject == null) return;
        if (objectPool.TryGetValue(gameObject.name, out Queue<GameObject> objectList))
        {
            objectList.Enqueue(gameObject);
        }
        else
        {
            Queue<GameObject> newObjectQueue = new Queue<GameObject>();
            newObjectQueue.Enqueue(gameObject);
            objectPool.Add(gameObject.name, newObjectQueue);
        }
        gameObject.SetActive(false);
        gameObject.transform.SetParent(poolParent);
    }
}
