using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private readonly Transform parent;
    private readonly Queue<T> pool = new();
    private readonly T prefab;

    public ObjectPool(T prefab, Transform parent = null, int initialSize = 10)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (var i = 0; i < initialSize; i++) CreateNewInstance();
    }

    private void CreateNewInstance()
    {
        var instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(false);
        pool.Enqueue(instance);
    }

    public T Get()
    {
        if (pool.Count == 0) CreateNewInstance();

        var instance = pool.Dequeue();
        instance.gameObject.SetActive(true);
        return instance;
    }

    public T Get(Vector3 position)
    {
        if (pool.Count == 0) CreateNewInstance();

        var instance = pool.Dequeue();
        instance.transform.position = position;
        instance.gameObject.SetActive(true);
        return instance;
    }

    public void Return(T instance)
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }

    public void Clear()
    {
        while (pool.Count > 0)
        {
            var instance = pool.Dequeue();
            if (instance != null) Object.Destroy(instance.gameObject);
        }
    }
}