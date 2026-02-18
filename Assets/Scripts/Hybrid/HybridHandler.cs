using System.Collections.Generic;
using Core;
using DOTS.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Hybrid
{
    public class HybridHandler : MonoBehaviour
    {
        [Inject] private InputSystem_Actions inputActions;
        [Inject] private List<WaveSettings> waveSettingsList;
        [Inject] private List<WeaponSettings> weaponSettingsList;
        [Inject] private List<EnemyWeaponSettings> enemyWeaponSettingsList;
        [Inject] private List<BulletSettings> bulletSettingsList;
        [Inject] private UnitsSettings unitsSettings;
        [Inject] private CameraSettings cameraSettings;
        [Inject] private List<EnemySettings> enemySettingsList;
        [Inject] private PlayerSettings playerSettings;
        [Inject] private List<PickupSettings> pickupSettingsList;
        [Inject] private DropSettings dropSettings;
        [Inject] private BombConsumableSettings bombSettings;
        [Inject] private IInventory inventory;
        [Inject] private List<WeaponTypeToItemType> weaponTypeToItemTypeMap;
    
        private HybridUtils _hybridUtils = new ();
    
        public void SetInputDataField(InputDataActionType inputAction, InputAction.CallbackContext context = default)
        {
            _hybridUtils.TryGetComponentAndEntityWithAll<InputData>(out var component, out var entity, out var entityManager);

            switch (inputAction)
            {
                case InputDataActionType.Move:
                    component.Movement = inputActions.Player.Move.ReadValue<Vector2>();
                    break;
                case InputDataActionType.MousePos:
                    var inputMousePosition = context.ReadValue<Vector2>();
                    component.MousePos = Camera.main.ScreenToWorldPoint(new Vector3(inputMousePosition.x, inputMousePosition.y, 10));
                    break;
                case InputDataActionType.MouseLeftButton:
                    component.Fire = true;
                    break;
                case InputDataActionType.MouseLeftButtonCancel:
                    component.Fire = false;
                    break;
            }

            entityManager.SetComponentData(entity, component);
        }
    
        public void RestartEcsGame()
        {
            InitializeEcs(false, true, 0);
        }

        public void InitializeEcs(params object[] args)
        {
            _hybridUtils.TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);

            if (args != null && args.Length == 3)
            {
                component.GameOver = (bool)args[0];
                component.Restart = (bool)args[1];
                component.Score = (int)args[2];
            }
        
            component.ShouldInitialize = true;

            entityManager.SetComponentData(entity, component);
        }

        public bool IsGameOver()
        {
            _hybridUtils.TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
        
            return component.GameOver;
        }
    
        public bool IsScoreChanged(out int score)
        {
            score = 0;
        
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<OnScoreChanged>(out var onScoreChangedEvent, out var entity, out var entityManager)) return false;
        
            score = onScoreChangedEvent.score;
            entityManager.SetComponentEnabled<OnScoreChanged>(entity, false);
            return true;
        }
    
        public bool IsWaveChanged(out int wave)
        {
            wave = 0;
        
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<OnWaveChanged>(out var onWaveChangedEvent, out var entity, out var entityManager)) return false;
        
            wave = onWaveChangedEvent.wave;
            entityManager.SetComponentEnabled<OnWaveChanged>(entity, false);
            return true;
        }
    
        public bool IsEnemiesLeftChanged(out int enemiesLeftCount)
        {
            enemiesLeftCount = 0;
        
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<OnEnemiesLeftCountChanged>(out var onEnemiesLeftCountChangedEvent, out var entity, out var entityManager)) return false;
        
            enemiesLeftCount = onEnemiesLeftCountChangedEvent.enemiesLeftCount;
            entityManager.SetComponentEnabled<OnEnemiesLeftCountChanged>(entity, false);
            return true;
        }

        public bool TryGetCameraPosition(out float3 position)
        {
            position = default;
        
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<CameraFollow>(out var component, out var entity, out var entityManager)) return false;
        
            var cameraFollowLocalTransform = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entity);
            position = cameraFollowLocalTransform.Position;
            return true;
        }

        public bool IsPlayClipOnDamage(out AudioClip clip)
        {
            clip = default;
        
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<PlayAudioClipOnDamageData>(out var component, out var entity, out var entityManager)) return false;
            entityManager.SetComponentEnabled<PlayAudioClipOnDamageData>(entity, false);
            clip = component.AudioClip;
            
            return true;
        }

        public bool IsPlayClipOnSpawn(out AudioClip clip)
        {
            clip = default;
        
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<PlayAudioClipOnSpawnData>(out var component, out var entity, out var entityManager)) return false;
            entityManager.SetComponentEnabled<PlayAudioClipOnSpawnData>(entity, false);
            clip = component.AudioClip;
            
            return true;
        }

        public void SetGameConfig()
        {
            _hybridUtils.GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);
            component.Weapons = CreateWeaponsBlobAsset(weaponSettingsList);
            component.EnemyWeapons = CreateEnemyWeaponsBlobAsset(enemyWeaponSettingsList);
            component.unitsSettings = new UnitsSettings()
            {
                Layer = unitsSettings.Layer,
                EnemySpawnDistance = unitsSettings.EnemySpawnDistance
            };

            component.Bullets = CreateBulletsBlobAsset(bulletSettingsList);
            component.EnemySettings = CreateEnemySettingsBlobAsset(enemySettingsList);
            component.playerSettings = new PlayerSettings()
            {
                moveSpeed = playerSettings.moveSpeed,
                rotationSpeed = playerSettings.rotationSpeed
            };
        
            entityManager.SetComponentData(entity, component);
            SetCameraSettings();
            SetPickupSettings();
            SetDropSettings();
            SetConsumableSettings();
        }

        public void SetWaveSettings(int waveIndex)
        {
            _hybridUtils.GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);
            if (component.Wave != default)
            {
                component.Wave.Dispose();
            }
            component.Wave = CreateWaveBlobAsset(waveSettingsList[waveIndex]);
            var countLeft = 0;
            for (int i = 0; i < waveSettingsList[waveIndex].EnemiesInWave.Count; i++)
            {
                countLeft += waveSettingsList[waveIndex].EnemiesInWave[i].count;
            }

            component.enemiesCountLeft = countLeft;
            entityManager.SetComponentData(entity, component);
        
            InvokeEnemiesLeftCountChanged(countLeft);
        }

        public bool IsOnSwitchWeapon(out WeaponType weaponType)
        {
            weaponType = default;
            if (!_hybridUtils.TryGetComponentAndEntityWithAll<OnSwitchWeapon>(out var component, out var entity, out var entityManager)) return false;
            if (!entityManager.IsComponentEnabled<OnSwitchWeaponSystem>(entity) || !entityManager.IsComponentEnabled<OnSwitchWeaponAnim>(entity)) return false;

            weaponType = component.weaponType;
            return true;
        }

        public void SwitchWeaponInInventory(WeaponType weaponType)
        {
            var itemType = weaponTypeToItemTypeMap.Find(x => x.weaponType == weaponType).itemType;
            inventory.RemoveSelectedItem();
            inventory.AddItem(itemType);
            inventory.SelectItem(itemType);
            _hybridUtils.GetComponentAndEntityWithPresent<OnSwitchWeaponUi>(out var component, out var entity, out var entityManager);
            entityManager.SetComponentEnabled<OnSwitchWeaponUi>(entity, true);
        }

        public void SwitchWeapon(WeaponType weaponType)
        {
            _hybridUtils.GetComponentAndEntityWithPresent<OnSwitchWeapon>(out var component, out var entity, out var entityManager);
            component.weaponType = weaponType;
            entityManager.SetComponentData<OnSwitchWeapon>(entity, component);
            entityManager.SetComponentEnabled<OnSwitchWeapon>(entity, true);
        }

        private void InvokeEnemiesLeftCountChanged(int countLeft)
        {
            _hybridUtils.GetComponentAndEntityWithPresent<OnEnemiesLeftCountChanged>(out var component, out var entity, out var entityManager);
            component.enemiesLeftCount = countLeft;
            entityManager.SetComponentData(entity, component);
            entityManager.SetComponentEnabled<OnEnemiesLeftCountChanged>(entity, true);
        }

        private void SetCameraSettings()
        {
            _hybridUtils.GetComponentAndEntityWithAll<CameraFollow>(out var component, out var entity, out var entityManager);

            component.offset = cameraSettings.offset;
            component.moveSpeed = cameraSettings.speed;
        
            entityManager.SetComponentData(entity, component);
        }

        private BlobAssetReference<WaveBlob> CreateWaveBlobAsset(WaveSettings settings)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var waveBlob = ref builder.ConstructRoot<WaveBlob>();

            var arrayBuilder = builder.Allocate(ref waveBlob.Array, settings.EnemiesInWave.Count);

            for (var i = 0; i < settings.EnemiesInWave.Count; i++)
            {
                arrayBuilder[i].type = settings.EnemiesInWave[i].type;
                arrayBuilder[i].count = settings.EnemiesInWave[i].count;
                arrayBuilder[i].points = settings.EnemiesInWave[i].points;
            }

            return builder.CreateBlobAssetReference<WaveBlob>(Allocator.Persistent);
        }

        private BlobAssetReference<WeaponsBlob> CreateWeaponsBlobAsset(IReadOnlyList<WeaponSettings> weaponsSettings)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var weaponBlob = ref builder.ConstructRoot<WeaponsBlob>();

            var arrayBuilder = builder.Allocate(ref weaponBlob.Array, weaponsSettings.Count);

            for (var i = 0; i < weaponsSettings.Count; i++)
            {
                var settings = weaponsSettings[i];
                arrayBuilder[i].type = settings.type;
                arrayBuilder[i].bulletType = settings.bulletType;
                arrayBuilder[i].TimeMax = 60 / settings.fireRate;
                arrayBuilder[i].dropWeight = settings.dropWeight;
            }

            return builder.CreateBlobAssetReference<WeaponsBlob>(Allocator.Persistent);
        }
        
        private BlobAssetReference<EnemyWeaponsBlob> CreateEnemyWeaponsBlobAsset(IReadOnlyList<EnemyWeaponSettings> enemyWeaponsSettings)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var weaponBlob = ref builder.ConstructRoot<EnemyWeaponsBlob>();

            var arrayBuilder = builder.Allocate(ref weaponBlob.Array, enemyWeaponsSettings.Count);

            for (var i = 0; i < enemyWeaponsSettings.Count; i++)
            {
                var settings = enemyWeaponsSettings[i];
                arrayBuilder[i].type = settings.type;
                arrayBuilder[i].bulletType = settings.bulletType;
                arrayBuilder[i].TimeMax = 60 / settings.fireRate;
            }

            return builder.CreateBlobAssetReference<EnemyWeaponsBlob>(Allocator.Persistent);
        }

        private BlobAssetReference<BulletsSettingsBlob> CreateBulletsBlobAsset(IReadOnlyList<BulletSettings> settings)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var bulletsBlob = ref builder.ConstructRoot<BulletsSettingsBlob>();

            var arrayBuilder = builder.Allocate(ref bulletsBlob.Array, settings.Count);

            for (var i = 0; i < settings.Count; i++)
            {
                arrayBuilder[i].type = settings[i].type;
                arrayBuilder[i].speed = settings[i].speed;
                arrayBuilder[i].maxDistance = settings[i].maxDistance;
                arrayBuilder[i].damageOnTrigger = settings[i].damageOnTrigger;
                arrayBuilder[i].explosionRadius = settings[i].explosionRadius;
                arrayBuilder[i].explosionDamage = settings[i].explosionDamage;
            }

            return builder.CreateBlobAssetReference<BulletsSettingsBlob>(Allocator.Persistent);
        }

        private BlobAssetReference<EnemySettingsBlob> CreateEnemySettingsBlobAsset(List<EnemySettings> settingsList)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var blob = ref builder.ConstructRoot<EnemySettingsBlob>();

            var arrayBuilder = builder.Allocate(ref blob.Array, settingsList.Count);
        
            for (var i = 0; i < settingsList.Count; i++)
            {
                arrayBuilder[i].type = settingsList[i].type;
                arrayBuilder[i].moveSpeed = settingsList[i].moveSpeed;
                arrayBuilder[i].rotationSpeed = settingsList[i].rotationSpeed;
            }

            return builder.CreateBlobAssetReference<EnemySettingsBlob>(Allocator.Persistent);
        }

        private void SetDropSettings()
        {
            _hybridUtils.GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);
            component.dropSettings = dropSettings;
        
            entityManager.SetComponentData(entity, component);
        }

        private void SetPickupSettings()
        {
            _hybridUtils.GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);

            component.pickupSettings = CreatePickupSettingsBlobAsset(pickupSettingsList);
        
            entityManager.SetComponentData(entity, component);
        }

        private BlobAssetReference<PickupSettingsBlob> CreatePickupSettingsBlobAsset(List<PickupSettings> settingsList)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var blob = ref builder.ConstructRoot<PickupSettingsBlob>();

            var arrayBuilder = builder.Allocate(ref blob.Array, settingsList.Count);

            for (var i = 0; i < settingsList.Count; i++)
            {
                arrayBuilder[i].type = settingsList[i].type;
                arrayBuilder[i].dropWeight = settingsList[i].Weight;
            }

            return builder.CreateBlobAssetReference<PickupSettingsBlob>(Allocator.Persistent);
        }

        private void SetConsumableSettings()
        {
            _hybridUtils.GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);
            component.bombSettings = bombSettings;
        
            entityManager.SetComponentData(entity, component);
        }
    }
}