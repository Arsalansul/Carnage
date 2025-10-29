using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "DOTS/GameConfig")]
public class GameConfig : ScriptableObject
{
    public List<WaveSettings> WaveSettingsList;
    public List<WeaponSettings> WeaponSettings;
    public int UnitsLayer;
    public float EnemySpawnDistance;
}

[Serializable]
public class WaveSettings
{
    // public List<GameObject> enemyDotsPrefab;
    public int enemiesCount;
}

[Serializable]
public class WeaponSettings
{
    public float fireRate;
    public BulletsType bulletType;
}

public enum BulletsType
{
    small,
    explosion
}