using System;
using System.Collections.Concurrent;
using System.Linq;
using UnityEngine;


public class BagEmptyException : ApplicationException { }
public interface IPoolable
{
    ObjectPoolComponent Pool { get; set; }
}
public class ObjectPool<T>
{
    ConcurrentDictionary<int, T> pool = new ConcurrentDictionary<int, T>();
    ConcurrentQueue<int> indexsDisponibles = new ConcurrentQueue<int>();
    int counter = 0;

    public T Take()
    {
        if (pool.IsEmpty)
            throw new BagEmptyException();
        //Récupère juste les clés dans le dictionnaire
        int[] keys = pool.Keys.ToArray();
        int rng = keys[UnityEngine.Random.Range(0, keys.Length)];
        if (!pool.TryRemove(rng, out var item))
            throw new BagEmptyException();
        indexsDisponibles.Enqueue(rng);
        return item;
    }
    public void Add(T item)
    {
        if (!indexsDisponibles.TryDequeue(out int index))
            index = counter++;
        pool[index] = item;
    }
}
