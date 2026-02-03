using System;
using DOTS;
using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

internal partial struct EnemySpawnerSystem : ISystem
{
    private Random random;
    private float3 currentCenterPosition;

    // [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        random = new Random((uint)DateTime.Now.Ticks);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gameStateEntity = SystemAPI.GetSingletonEntity<GameState>();
        var gameState = SystemAPI.GetComponent<GameState>(gameStateEntity);
        var config = SystemAPI.GetSingleton<GameConfigComponent>();
        
        ref var waveReference = ref config.Wave;
        ref var enemiesArray = ref waveReference.Value;

        ref var enemySettingsArray = ref config.EnemySettings.Value.Array;

        foreach (var (player, localTransform) in SystemAPI.Query<RefRO<Player>, RefRO<LocalTransform>>())
        {
            currentCenterPosition = localTransform.ValueRO.Position;
            break;
        }

        if (gameState.OnWaveChanged)
        {
            foreach (var (enemySpawner, enemySpawnerEnable) in
                     SystemAPI.Query<RefRW<EnemySpawner>, EnabledRefRW<EnemySpawner>>().WithPresent<EnemySpawner>())
            {
                enemySpawnerEnable.ValueRW = true;
            }
        }

        foreach (var (enemySpawner, enemySpawnerEnable) in
                 SystemAPI.Query<RefRW<EnemySpawner>,EnabledRefRW<EnemySpawner>>())
        {
            enemySpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (enemySpawner.ValueRW.timer > 0f) continue;

            enemySpawner.ValueRW.timer = enemySpawner.ValueRO.timerMax;

            var enemyPrefab = GetNextEnemyEntity(ref enemiesArray, ref state, out var enemyInWaveConf);
            if (enemyPrefab == Entity.Null)
            {
                enemySpawnerEnable.ValueRW = false;
                continue;
            }

            var enemyEntity = state.EntityManager.Instantiate(enemyPrefab);
            
            var spawnPosition = RandomPosition(currentCenterPosition, config.unitsSettings.EnemySpawnDistance);
            SystemAPI.SetComponent(enemyEntity, LocalTransform.FromPosition(spawnPosition));

            var randomWalking = SystemAPI.GetComponentRW<RandomWalking>(enemyEntity);
            randomWalking.ValueRW.originPosition = spawnPosition;
            randomWalking.ValueRW.targetPosition = spawnPosition;
            randomWalking.ValueRW.random = new Random((uint)enemyEntity.Index);

            var health = SystemAPI.GetComponentRW<Health>(enemyEntity);
            health.ValueRW.max = (int) (health.ValueRW.max * (gameState.Wave + 0.5f));
            health.ValueRW.amount = health.ValueRW.max;
            
            var enemyComponent = SystemAPI.GetComponentRW<Enemy>(enemyEntity);
            enemyComponent.ValueRW.Points = enemyInWaveConf.points;

            var enemySettings = enemySettingsArray[0];
            for (int i = 0; i < enemySettingsArray.Length; i++)
            {
                if (enemySettingsArray[i].type == enemyInWaveConf.type)
                {
                    enemySettings = enemySettingsArray[i];
                }
            }
            
            var unitMover = SystemAPI.GetComponentRW<UnitMover>(enemyEntity);
            unitMover.ValueRW.moveSpeed = enemySettings.moveSpeed;
            unitMover.ValueRW.rotationSpeed = enemySettings.rotationSpeed;

            if (SystemAPI.HasComponent<ShootAttack>(enemyEntity))
            {
                ref var weaponsReference = ref config.Weapons;
                ref var weaponsArray = ref weaponsReference.Value;

                var enemyShootAttack = SystemAPI.GetComponentRO<EnemyShootAttack>(enemyEntity);
                var shootAttack = SystemAPI.GetComponentRW<ShootAttack>(enemyEntity);

                var weaponBlob = weaponsArray.Array[0];
        
                for (int i = 0; i < weaponsArray.Array.Length; i++)
                {
                    if (weaponsArray.Array[i].type == enemyShootAttack.ValueRO.weaponType)
                    {
                        weaponBlob = weaponsArray.Array[i];
                        break;
                    }
                }

                shootAttack.ValueRW.currentWeapon = weaponBlob;
                shootAttack.ValueRW.timerMax = weaponBlob.TimeMax;
            }
        }
    }

    [BurstCompile]
    private float3 RandomPosition(float3 centerPosition, float distance)
    {
        var direction = new float3(random.NextFloat(), 0, random.NextFloat());
        return centerPosition + math.normalize(direction) * distance;
    }

    [BurstCompile]
    private Entity GetNextEnemyEntity(ref WaveBlob waveBlob, ref SystemState state, out EnemyInWave enemyInWaveConf)
    {
        var entitiesReferencesEntity = SystemAPI.GetSingletonEntity<EntitiesReferences>();

        var index = random.NextInt(0, waveBlob.Array.Length - 1);

        for (int i = 0; i < waveBlob.Array.Length; i++)
        {
            if (waveBlob.Array[index].count > 0)
                break;
            index = (index + 1) % waveBlob.Array.Length;
        }

        enemyInWaveConf = waveBlob.Array[index];
        
        if (waveBlob.Array[index].count <= 0)
        {
            return Entity.Null;
        }

        waveBlob.Array[index].count--;

        var enemiesMap = state.EntityManager.GetBuffer<EnemiesEntityMap>(entitiesReferencesEntity);

        for (int i = 0; i < enemiesMap.Length; i++)
        {
            if (enemiesMap[i].type == waveBlob.Array[index].type)
            {
                return enemiesMap[i].entity;
            }
        }

        throw new Exception("enemy not found in map");
    }
}