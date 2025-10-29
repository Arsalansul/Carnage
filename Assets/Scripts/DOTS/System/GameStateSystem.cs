using Unity.Burst;
using Unity.Entities;

namespace System
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventsSystem))]
    [UpdateAfter(typeof(HealthSystem))]
    public partial struct GameStateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<GameConfigComponent>();
            ref var wavesReference = ref config.Waves;
            ref var wavesArray = ref wavesReference.Value;

            var gameOverInThisFrame = false;

            foreach (var (health, player, entity) in
                     SystemAPI.Query<RefRO<Health>, RefRO<Player>>().WithEntityAccess())
            {
                if (!health.ValueRO.onHealthChanged) continue;

                if (health.ValueRO.amount <= 0) gameOverInThisFrame = true;
            }

            var gameState = SystemAPI.GetSingleton<GameState>();

            gameState.OnWaveChanged = false;

            if (gameOverInThisFrame)
            {
                gameState.GameOver = true;
                SystemAPI.SetSingleton(gameState);
                return;
            }

            foreach (var (health, enemy, entity) in SystemAPI.Query<RefRO<Health>, RefRO<Enemy>>().WithEntityAccess())
            {
                if (!health.ValueRO.onHealthChanged) continue;

                if (health.ValueRO.amount <= 0)
                {
                    gameState.Score += enemy.ValueRO.Points;
                    gameState.EnemiesLeftCount --;
                }
            }

            var spawnedEnemyCount = 0;
            foreach (var enemySpawner in SystemAPI.Query<RefRO<EnemySpawner>>().WithPresent<EnemySpawner>())
                spawnedEnemyCount += enemySpawner.ValueRO.spawnedCount;
            gameState.SpawnedEnemiesCount = spawnedEnemyCount;

            if (gameState.SpawnedEnemiesCount >= wavesArray.Array[gameState.Wave].EnemiesCount)
            {
                var enemiesCount = 0;
                foreach (var (health, enemy) in SystemAPI.Query<RefRO<Health>, RefRO<Enemy>>())
                {
                    enemiesCount++;
                    break;
                }

                if (enemiesCount == 0)
                {
                    gameState.SpawnedEnemiesCount = 0;
                    gameState.Wave++;
                    gameState.OnWaveChanged = true;
                }
                else
                {
                    foreach (var enemySpawner in SystemAPI.Query<EnabledRefRW<EnemySpawner>>())
                    {
                        enemySpawner.ValueRW = false;
                    }
                }
            }

            if (gameState.ShouldInitialize)
            {
                var playerSpawner = SystemAPI.GetSingletonRW<PlayerSpawner>();
                playerSpawner.ValueRW.shouldSpawn = true;
                gameState.Score = 0;
                gameState.Wave = 0;
                gameState.OnWaveChanged = true;
            }

            if (gameState.Restart)
            {
                var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged);
                foreach (var (unit, entity) in SystemAPI.Query<RefRO<Unit>>().WithEntityAccess())
                    entityCommandBuffer.DestroyEntity(entity);
            }

            if (gameState.OnWaveChanged)
            {
                foreach (var (player, health) in SystemAPI.Query<RefRO<Player>, RefRW<Health>>())
                {
                    health.ValueRW.amount = health.ValueRO.max;
                    health.ValueRW.onHealthChanged = true;
                }
                
                gameState.EnemiesLeftCount = wavesArray.Array[gameState.Wave].EnemiesCount;
            }

            SystemAPI.SetSingleton(gameState);
        }
    }
}