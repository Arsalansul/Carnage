using System;
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
        ref var wavesReference = ref config.Waves;
        ref var wavesArray = ref wavesReference.Value;

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
                enemySpawner.ValueRW.spawnedCount = 0;
                enemySpawnerEnable.ValueRW = true;
            }
        }

        foreach (var (localTransform, enemySpawner) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<EnemySpawner>>())
        {
            enemySpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (enemySpawner.ValueRW.timer > 0f) continue;

            enemySpawner.ValueRW.timer = enemySpawner.ValueRO.timerMax;
            enemySpawner.ValueRW.spawnedCount++;

            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            var enemyEntity = state.EntityManager.Instantiate(random.NextBool()
                ? entitiesReferences.enemyPrefab_0
                : entitiesReferences.enemyPrefab_1);
            var spawnPosition = RandomPosition(currentCenterPosition, config.EnemySpawnDistance);
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

    private float3 RandomPosition(float3 centerPosition, float distance)
    {
        var direction = new float3(random.NextFloat(), 0, random.NextFloat());
        return centerPosition + math.normalize(direction) * distance;
    }
}