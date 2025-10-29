using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UnitPoolConfig
{
    public UnitView prefab;
    public int initialSize = 10;
}

[Serializable]
public class AudioSourcePoolConfig
{
    public AudioSource audioSource;
    public int initialSize = 10;
}
    
[Serializable]
public class BulletConfig
{
    public GameObject prefab;
    public int initialSize = 10;
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    [SerializeField] private List<UnitPoolConfig> unitPoolConfigs;
    [SerializeField] private List<BulletConfig> bulletsConfigs;
    [SerializeField] private AudioSourcePoolConfig audioSourceConfig;

    private Dictionary<string, ObjectPool<UnitView>> unitPools;
    private Dictionary<string, ObjectPool<Transform>> bulletsPools;
    private ObjectPool<AudioSource> audioSources;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePools()
    {
        unitPools = new Dictionary<string, ObjectPool<UnitView>>();

        foreach (var config in unitPoolConfigs)
        {
            unitPools[config.prefab.name] = InitializePool(config.prefab, config.initialSize);
        }

        bulletsPools = new Dictionary<string, ObjectPool<Transform>>();

        foreach (var config in bulletsConfigs)
        {
            bulletsPools[config.prefab.name] = InitializePool(config.prefab.transform, config.initialSize);
        }
        audioSources = InitializePool(audioSourceConfig.audioSource, audioSourceConfig.initialSize);
    }

    public UnitView GetUnitFromPool(string poolName)
    {
        return unitPools.TryGetValue(poolName, out var pool) ? pool.Get() : null;
    }

    public UnitView GetUnitFromPool(string poolName, Vector3 position)
    {
        return unitPools.TryGetValue(poolName, out var pool) ? pool.Get(position) : null;
    }

    public void ReturnUnitToPool(string poolName, UnitView obj)
    {
        if (unitPools.ContainsKey(poolName)) unitPools[poolName].Return(obj);
    }

    public AudioSource GetAudioSource()
    {
        return audioSources.Get();
    }
    
    public void ReturnAudioSourceToPool(AudioSource audioSource)
    {
        audioSources.Return(audioSource);
    }

    public void ReturnAudioSourceToPool(AudioSource audioSource, float delayTime)
    {
        StartCoroutine(ReturnToPoolCoroutine(audioSources, audioSource, delayTime));
    }

    public Transform GetBulletFromPool(string poolName)
    {
        return bulletsPools.TryGetValue(poolName, out var pool) ? pool.Get() : null;
    }

    public void ReturnBulletToPool(string poolName, Transform bullet)
    {
        if (bulletsPools.ContainsKey(poolName)) bulletsPools[poolName].Return(bullet);
    }

    private ObjectPool<T> InitializePool<T>(T prefab, int initialSize) where T : Component
    {
        var parent = new GameObject(prefab.name + "Pool");
        parent.transform.SetParent(transform);
        return new ObjectPool<T>(prefab, parent.transform, initialSize);
    }
    private IEnumerator ReturnToPoolCoroutine<T>(ObjectPool<T> pool, T obj, float delay)  where T : Component
    {
        yield return new WaitForSeconds(delay);
        pool.Return(obj);
    }
}