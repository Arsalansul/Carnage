using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ConfigAuthoring : MonoBehaviour
{
    public GameConfig gameConfig;

    //todo waves config
    public class Baker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            if (authoring.gameConfig == null) return;

            var entity = GetEntity(TransformUsageFlags.None);
            var wavesBlobAsset = CreateWavesBlobAsset(authoring.gameConfig.WaveSettingsList);
            AddBlobAsset(ref wavesBlobAsset, out _);

            var weaponsSettings = CreateWeaponsBlobAsset(authoring.gameConfig.WeaponSettings);
            AddBlobAsset(ref weaponsSettings, out _);

            AddComponent(entity, new GameConfigComponent
            {
                Waves = wavesBlobAsset,
                Weapons = weaponsSettings,
                UnitsLayer = authoring.gameConfig.UnitsLayer,
                EnemySpawnDistance = authoring.gameConfig.EnemySpawnDistance
            });
        }

        private BlobAssetReference<WavesBlob> CreateWavesBlobAsset(IReadOnlyList<WaveSettings> waveSettingsList)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var wavesBlob = ref builder.ConstructRoot<WavesBlob>();

            var arrayBuilder = builder.Allocate(ref wavesBlob.Array, waveSettingsList.Count);

            for (var i = 0; i < waveSettingsList.Count; i++)
            {
                var waveSettings = waveSettingsList[i];
                arrayBuilder[i].EnemiesCount = waveSettings.enemiesCount;
            }

            return builder.CreateBlobAssetReference<WavesBlob>(Allocator.Persistent);
        }

        private BlobAssetReference<WeaponsBlob> CreateWeaponsBlobAsset(IReadOnlyList<WeaponSettings> weaponsSettings)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var weaponBlob = ref builder.ConstructRoot<WeaponsBlob>();

            var arrayBuilder = builder.Allocate(ref weaponBlob.Array, weaponsSettings.Count);

            for (var i = 0; i < weaponsSettings.Count; i++)
            {
                var weaponSettings = weaponsSettings[i];
                arrayBuilder[i].BulletIndex = (int)weaponSettings.bulletType;
                arrayBuilder[i].TimeMax = 60 / weaponSettings.fireRate;
            }

            return builder.CreateBlobAssetReference<WeaponsBlob>(Allocator.Persistent);
        }
    }
}

public struct GameConfigComponent : IComponentData
{
    public BlobAssetReference<WavesBlob> Waves;
    public BlobAssetReference<WeaponsBlob> Weapons;
    public int UnitsLayer;
    public float EnemySpawnDistance;
}

public struct WavesBlob
{
    public BlobArray<EnemyWaveBlob> Array;
}

public struct EnemyWaveBlob
{
    public int EnemiesCount;
}

public struct WeaponsBlob
{
    public BlobArray<WeaponBlob> Array;
}

public struct WeaponBlob
{
    public float TimeMax;
    public int BulletIndex;
}