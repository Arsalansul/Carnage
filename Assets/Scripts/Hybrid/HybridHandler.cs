using System.Collections.Generic;
using System.Linq;
using DOTS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class HybridHandler : MonoBehaviour
{
    [Inject] private NewInputActions inputActions;
    [Inject] private List<WaveSettings> waveSettingsList;
    [Inject] private List<WeaponSettings> weaponSettingsList;
    [Inject] private List<BulletSettings> bulletSettingsList;
    [Inject] private UnitsSettings unitsSettings;
    [Inject] private CameraSettings cameraSettings;
    
    public void SetInputDataField(InputDataActionType inputAction, InputAction.CallbackContext context = default)
    {
        TryGetComponentAndEntityWithAll<InputData>(out var component, out var entity, out var entityManager);

        switch (inputAction)
        {
            case InputDataActionType.Move:
                component.Movement = inputActions.GameMap.Move.ReadValue<Vector2>();
                break;
            case InputDataActionType.MousePos:
                var inputMousePosition = context.ReadValue<Vector2>();
                component.MousePos =
                    Camera.main.ScreenToWorldPoint(new Vector3(inputMousePosition.x, inputMousePosition.y, 10));
                break;
            case InputDataActionType.MouseLeftButton:
                component.MouseLeft = true;
                break;
            case InputDataActionType.MouseRightButton:
                component.MouseRight = true;
                break;
            case InputDataActionType.MouseLeftButtonCancel:
                component.MouseLeft = false;
                break;
        }

        entityManager.SetComponentData(entity, component);
    }
    
    public void RestartEcsGame()
    {
        TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
    
        component.GameOver = false;
        component.Restart = true;
        component.Score = 0;
        component.ShouldInitialize = true;
    
        entityManager.SetComponentData(entity, component);
    }

    public void InitializeEcs()
    {
        TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
    
        component.ShouldInitialize = true;
        entityManager.SetComponentData(entity, component);
    }

    public bool IsGameOver()
    {
        TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
        
        return component.GameOver;
    }
    
    public bool IsScoreChanged(out int score)
    {
        score = 0;
        
        if (!TryGetComponentAndEntityWithAll<OnScoreChanged>(out var onScoreChangedEvent, out var entity, out var entityManager)) return false;
        
        score = onScoreChangedEvent.score;
        entityManager.SetComponentEnabled<OnScoreChanged>(entity, false);
        return true;
    }
    
    public bool IsWaveChanged(out int wave)
    {
        wave = 0;
        
        if (!TryGetComponentAndEntityWithAll<OnWaveChanged>(out var onWaveChangedEvent, out var entity, out var entityManager)) return false;
        
        wave = onWaveChangedEvent.wave;
        entityManager.SetComponentEnabled<OnWaveChanged>(entity, false);
        return true;
    }
    
    public bool IsEnemiesLeftChanged(out int enemiesLeftCount)
    {
        enemiesLeftCount = 0;
        
        if (!TryGetComponentAndEntityWithAll<OnEnemiesLeftCountChanged>(out var onEnemiesLeftCountChangedEvent, out var entity, out var entityManager)) return false;
        
        enemiesLeftCount = onEnemiesLeftCountChangedEvent.enemiesLeftCount;
        entityManager.SetComponentEnabled<OnEnemiesLeftCountChanged>(entity, false);
        return true;
    }

    public bool TryGetCameraPosition(out float3 position)
    {
        position = default;
        
        if (!TryGetComponentAndEntityWithAll<CameraFollow>(out var component, out var entity, out var entityManager)) return false;
        
        var cameraFollowLocalTransform = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entity);
        position = cameraFollowLocalTransform.Position;
        return true;
    }

    public bool IsPlayClipOnDamage(out AudioClip clip)
    {
        clip = default;
        
        if (!TryGetComponentAndEntityWithAll<PlayAudioClipOnDamageData>(out var component, out var entity, out var entityManager)) return false;
        entityManager.SetComponentEnabled<PlayAudioClipOnDamageData>(entity, false);
        clip = component.AudioClip;
            
        return true;
    }

    public bool IsPlayClipOnSpawn(out AudioClip clip)
    {
        clip = default;
        
        if (!TryGetComponentAndEntityWithAll<PlayAudioClipOnSpawnData>(out var component, out var entity, out var entityManager)) return false;
        entityManager.SetComponentEnabled<PlayAudioClipOnSpawnData>(entity, false);
        clip = component.AudioClip;
            
        return true;
    }

    public void SetGameConfig()
    {
        GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);
        component.Weapons = CreateWeaponsBlobAsset(weaponSettingsList);
        component.unitsSettings = new UnitsSettings()
        {
            Layer = unitsSettings.Layer,
            EnemySpawnDistance = unitsSettings.EnemySpawnDistance
        };

        component.Bullets = CreateBulletsBlobAsset(bulletSettingsList);
        
        entityManager.SetComponentData(entity, component);
        SetCameraSettings();
    }

    public void SetWaveSettings(int waveIndex)
    {
        GetComponentAndEntityWithAll<GameConfigComponent>(out var component, out var entity, out var entityManager);
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

    private void InvokeEnemiesLeftCountChanged(int countLeft)
    {
        GetComponentAndEntityWithPresent<OnEnemiesLeftCountChanged>(out var component, out var entity, out var entityManager);
        component.enemiesLeftCount = countLeft;
        entityManager.SetComponentData(entity, component);
        entityManager.SetComponentEnabled<OnEnemiesLeftCountChanged>(entity, true);
    }

    private void SetCameraSettings()
    {
        GetComponentAndEntityWithAll<CameraFollow>(out var component, out var entity, out var entityManager);

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
            arrayBuilder[i].bulletType = settings.bulletType;
            arrayBuilder[i].TimeMax = 60 / settings.fireRate;
        }

        return builder.CreateBlobAssetReference<WeaponsBlob>(Allocator.Persistent);
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
        }

        return builder.CreateBlobAssetReference<BulletsSettingsBlob>(Allocator.Persistent);
        
    }
    
    private void GetComponentAndEntityWithAll<T>(out T component, out Entity entity, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        TryGetComponentAndEntityWithAll<T>(out component, out entity, out entityManager);
    }

    private bool TryGetComponentAndEntityWithAll<T>(out T component, out Entity entity,
        out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        component = default;
        entity = default;
        GetComponentsAndEntitiesWithAll<T>(out var components, out var entities, out entityManager);
        
        if (components.Length == 0) return false;
        component = components[0];
        entity = entities[0];
        return true;
    }
    
    private void GetComponentAndEntityWithPresent<T>(out T component, out Entity entity, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        GetComponentsAndEntitiesWithPresent<T>(out var componentArray, out var entities, out entityManager);
        component = componentArray[0];
        entity = entities[0];
    }

    private void GetComponentsAndEntitiesWithPresent<T>(out NativeArray<T> components, out NativeArray<Entity> entities, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithPresent<T>()
            .Build(entityManager);
        components = entityQuery.ToComponentDataArray<T>(Allocator.Temp);
        entities = entityQuery.ToEntityArray(Allocator.Temp);
    }

    private void GetComponentsAndEntitiesWithAll<T>(out NativeArray<T> components, out NativeArray<Entity> entities, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<T>()
            .Build(entityManager);
        components = entityQuery.ToComponentDataArray<T>(Allocator.Temp);
        entities = entityQuery.ToEntityArray(Allocator.Temp);
    }
}