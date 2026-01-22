using System;
using Unity.Entities;
using UnityEngine;

namespace DOTS
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
        public BlobAssetReference<BulletsSettingsBlob> Bullets;
        public UnitsSettings unitsSettings;
        public int enemiesCountLeft;
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

    public struct WeaponsBlob
    {
        public BlobArray<WeaponBlob> Array;
    }

    public struct WeaponBlob
    {
        public float TimeMax;
        public BulletsType bulletType;
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
}