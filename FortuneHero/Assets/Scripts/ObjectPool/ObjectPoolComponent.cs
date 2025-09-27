using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolComponent : MonoBehaviour
{
    //[SerializeField] GameObject objectToPool;
    //[SerializeField] int poolSize = 20;
    [Serializable]
    public class Pool
    {
        public GameObject objectToPool;
        public int poolSize = 20;
    }
    [SerializeField] List<Pool> pools;

    ObjectPool<GameObject> pool = new ObjectPool<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach (var p in pools)
            Initialize(p.objectToPool, p.poolSize);
    }

    public GameObject GetObject()
    {
        GameObject obj;
        //DANGER!!! Mieux avec un uplet (bool, GameObject)
        try
        {
            obj = pool.Take();
        }
        catch (BagEmptyException)
        {
            var clone = Instantiate(pools[0].objectToPool, transform);
            foreach (var item in clone.GetComponents<IPoolable>())
                item.Pool = this;
            clone.SetActive(false);
            obj = clone;
        }
        return obj;
    }
    public void PutObject(GameObject item)
    {
        item.SetActive(false);
        pool.Add(item);
    }
    public void Initialize(GameObject obj, int size)
    {
        for (int i = 0; i < size; ++i)
        {
            var clone = Instantiate(obj, transform);
            foreach (var item in clone.GetComponents<IPoolable>())
                item.Pool = this;
            clone.SetActive(false);
            pool.Add(clone);
        }
    }
}
