using System;
using DOTS;
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

            var enemyPrefab = GetNextEnemyEntity(ref enemiesArray);
            if (enemyPrefab == Entity.Null)
            {
                enemySpawnerEnable.ValueRW = false;
                continue;
            }

            var enemyEntity = state.EntityManager.Instantiate(enemyPrefab);
            
            var spawnPosition = RandomPosition(currentCenterPosition, config.unitsSettings.EnemySpawnDistance);
            SystemAPI.SetComponent(enemyEntity, LocalTransform.FromPosition(spawnPosition));

            var randomWalking = SystemAPI.GetComponent<RandomWalking>(enemyEntity);
            randomWalking.originPosition = spawnPosition;
            randomWalking.targetPosition = spawnPosition;
            randomWalking.random = new Random((uint)enemyEntity.Index);

            SystemAPI.SetComponent(enemyEntity, randomWalking);

            var health = SystemAPI.GetComponent<Health>(enemyEntity);
            health.max = (int) (health.max * (gameState.Wave + 0.5f));
            health.amount = health.max;
            
            SystemAPI.SetComponent(enemyEntity, health);
        }
    }

    [BurstCompile]
    private float3 RandomPosition(float3 centerPosition, float distance)
    {
        var direction = new float3(random.NextFloat(), 0, random.NextFloat());
        return centerPosition + math.normalize(direction) * distance;
    }

    [BurstCompile]
    private Entity GetNextEnemyEntity(ref WaveBlob waveBlob)
    {
        var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        var index = random.NextInt(0, waveBlob.Array.Length - 1);

        for (int i = 0; i < waveBlob.Array.Length; i++)
        {
            if (waveBlob.Array[index].count > 0)
                break;
            index = (index + 1) % waveBlob.Array.Length;
        }

        if (waveBlob.Array[index].count <= 0)
        {
            return Entity.Null;
        }

        waveBlob.Array[index].count--;
        
        switch (waveBlob.Array[index].type)
        {
            case EnemyType.Arachnid:
            default:
                return entitiesReferences.enemy_Arachnid;
            case EnemyType.Cockroach:
                return entitiesReferences.enemy_Cockroach;
        }
    }
}