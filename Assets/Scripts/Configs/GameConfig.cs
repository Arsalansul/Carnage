using System;
using System.Collections.Generic;
using DOTS;
using DOTS.Authoring;
using Ui.Models;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObjectInstaller<GameConfig>
{
    public List<WaveSettings> WaveSettingsList;
    public List<WeaponSettings> WeaponSettings;
    public List<BulletSettings> BulletSettingsList;
    public UnitsSettings UnitsSettings;
    public CameraSettings CameraSettings;
    public PlayerSettings PlayerSettings;
    public List<EnemySettings> EnemySettingsList;
    public List<WeaponTypeToItemType> WeaponTypeToItemTypeMap;
    public List<PickupSettings> PickupSettings;
    public DropSettings DropSettings;
    
    public override void InstallBindings()
    {
        Container.BindInstance(WaveSettingsList);
        Container.BindInstance(WeaponSettings);
        Container.BindInstance(BulletSettingsList);
        Container.BindInstance(UnitsSettings);
        Container.BindInstance(CameraSettings);
        Container.BindInstance(EnemySettingsList);
        Container.BindInstance(PlayerSettings);
        Container.BindInstance(WeaponTypeToItemTypeMap);
        Container.BindInstance(PickupSettings);
        Container.BindInstance(DropSettings);
    }
}

[Serializable]
public class WaveSettings
{
    public List<EnemyInWave> EnemiesInWave;
}

[Serializable]
public struct WeaponSettings
{
    public WeaponType type;
    public float fireRate;
    public BulletsType bulletType;
    public Sprite sprite;
}

[Serializable]
public struct CameraSettings
{
    public float speed;
    public float3 offset;
}

[Serializable]
public struct WeaponTypeToItemType
{
    public WeaponType weaponType;
    public ItemType itemType;
}