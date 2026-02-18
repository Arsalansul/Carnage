using System;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public class Baker : Baker<ConfigAuthoring>
        {
            public override void Bake(ConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new GameConfigComponent());
            }
        }
    }

    public struct GameConfigComponent : IComponentData
    {
        public BlobAssetReference<WaveBlob> Wave;
        public BlobAssetReference<WeaponsBlob> Weapons;
        public BlobAssetReference<EnemyWeaponsBlob> EnemyWeapons;
        public BlobAssetReference<EnemySettingsBlob> EnemySettings;
        public BlobAssetReference<BulletsSettingsBlob> Bullets;
        public UnitsSettings unitsSettings;
        public int enemiesCountLeft;
        public PlayerSettings playerSettings;
        public BlobAssetReference<PickupSettingsBlob> pickupSettings;
        public DropSettings dropSettings;
        public BombConsumableSettings bombSettings;
    }

    public struct WaveBlob
    {
        public BlobArray<EnemyInWave> Array;
    }

    [Serializable]
    public struct EnemyInWave
    {
        public int count;
        public int points;
        public EnemyType type;
    }

    public struct EnemySettingsBlob
    {
        public BlobArray<EnemySettings> Array;
    }
    
    [Serializable]
    public struct EnemySettings
    {
        public EnemyType type;
        public float moveSpeed;
        public float rotationSpeed;
    }

    public struct WeaponsBlob
    {
        public BlobArray<WeaponBlob> Array;
    }

    public struct EnemyWeaponsBlob
    {
        public BlobArray<WeaponBlob> Array;
    }

    public struct WeaponBlob : IDropWeight
    {
        public WeaponType type;
        public float TimeMax;
        public BulletsType bulletType;
        public int dropWeight;
        public int Weight => dropWeight;
    }

    [Serializable]
    public struct UnitsSettings
    {
        public int Layer;
        public float EnemySpawnDistance;
    }

    public struct BulletsSettingsBlob
    {
        public BlobArray<BulletSettings> Array;
    }

    [Serializable]
    public struct BulletSettings
    {
        public BulletsType type;
        public float speed;
        public float maxDistance;
        public int damageOnTrigger;
        public float explosionRadius;
        public int explosionDamage;
    }

    [Serializable]
    public struct PlayerSettings
    {
        public float moveSpeed;
        public float rotationSpeed;
    }

    [Serializable]
    public struct PickupSettings : IDropWeight
    {
        public PickupType type;
        public int dropWeight;
        public int Weight => dropWeight;
    }

    public struct PickupSettingsBlob
    {
        public BlobArray<PickupSettings> Array;
    }

    [Serializable]
    public struct DropSettings
    {
        [Range(0, 1)]
        public float chance;
    }

    [Serializable]
    public struct BombConsumableSettings
    {
        public int damage;
        public float explosionRange;
    }

    public interface IDropWeight
    {
        public int Weight { get; }
    }
}