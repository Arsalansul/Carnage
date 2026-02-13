using System;
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
    public PoolName poolName;
    public int initialSize = 10;
}

[Serializable]
public class ConsumableConfig
{
    public GameObject prefab;
    public PoolName poolName;
    public int initialSize = 10;
}