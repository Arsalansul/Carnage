using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;

namespace DOTS.System
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateBefore(typeof(ResetEventsSystem))]
    [UpdateAfter(typeof(HealthSystem))]
    public partial struct GameStateSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingletonRW<GameConfigComponent>();
            ref var waveReference = ref config.ValueRW.Wave;
            ref var enemiesArray = ref waveReference.Value;

            var gameOverInThisFrame = false; //TODO is it necessary?

            foreach (var (health, player, entity) in
                     SystemAPI.Query<RefRO<Health>, RefRO<Player>>().WithEntityAccess())
            {
                if (!health.ValueRO.onHealthChanged) continue;

                if (health.ValueRO.amount <= 0) gameOverInThisFrame = true;
            }

            var gameState = SystemAPI.GetSingletonRW<GameState>();
            var eventsHandlerEntity = SystemAPI.GetSingletonEntity<EventsHandler>();

            gameState.ValueRW.OnWaveChanged = false;

            if (gameOverInThisFrame)
            {
                gameState.ValueRW.GameOver = true;
                return;
            }

            foreach (var (health, enemy, entity) in SystemAPI.Query<RefRO<Health>, RefRO<Enemy>>().WithEntityAccess())
            {
                if (!health.ValueRO.onHealthChanged) continue;

                if (health.ValueRO.amount <= 0)
                {
                    gameState.ValueRW.Score += enemy.ValueRO.Points;
                    config.ValueRW.enemiesCountLeft --;

                    OnScoreChanged(gameState.ValueRO.Score, SystemAPI.GetComponentRW<OnScoreChanged>(eventsHandlerEntity), eventsHandlerEntity, ref state);

                    var onEnemiesLeftCountChanged = SystemAPI.GetComponentRW<OnEnemiesLeftCountChanged>(eventsHandlerEntity);
                    onEnemiesLeftCountChanged.ValueRW.enemiesLeftCount = config.ValueRO.enemiesCountLeft;
                    SystemAPI.SetComponentEnabled<OnEnemiesLeftCountChanged>(eventsHandlerEntity, true);
                }
            }
            
            if (config.ValueRO.enemiesCountLeft <= 0)
            {
                gameState.ValueRW.Wave++;
                gameState.ValueRW.OnWaveChanged = true;
                OnWaveChanged(gameState, SystemAPI.GetComponentRW<OnWaveChanged>(eventsHandlerEntity), eventsHandlerEntity, ref enemiesArray, ref state);
            }

            if (gameState.ValueRO.ShouldInitialize)
            {
                var playerSpawner = SystemAPI.GetSingletonRW<PlayerSpawner>();
                playerSpawner.ValueRW.shouldSpawn = true;
                gameState.ValueRW.Score = 0;
                gameState.ValueRW.Wave = 0;
                gameState.ValueRW.OnWaveChanged = true;
                
                OnScoreChanged(gameState.ValueRO.Score, SystemAPI.GetComponentRW<OnScoreChanged>(eventsHandlerEntity), eventsHandlerEntity, ref state);
                OnWaveChanged(gameState, SystemAPI.GetComponentRW<OnWaveChanged>(eventsHandlerEntity), eventsHandlerEntity, ref enemiesArray, ref state);
            }

            if (gameState.ValueRO.Restart)
            {
                var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
                foreach (var (unit, entity) in SystemAPI.Query<RefRO<Unit>>().WithEntityAccess())
                    entityCommandBuffer.DestroyEntity(entity);
            }
        }

        [BurstCompile]
        private void OnScoreChanged(int score, RefRW<OnScoreChanged> onScoreChanged, Entity entity, ref SystemState state)
        {
            onScoreChanged.ValueRW.score = score;
            state.EntityManager.SetComponentEnabled<OnScoreChanged>(entity, true);
        }
        
        [BurstCompile]
        private void OnWaveChanged(RefRW<GameState> gameState, RefRW<OnWaveChanged> onWaveChanged, Entity eventsHandlerEntity, 
            ref WaveBlob waveBlob, ref SystemState state)
        {
            onWaveChanged.ValueRW.wave = gameState.ValueRO.Wave;
            
            foreach (var (player, health) in SystemAPI.Query<RefRO<Player>, RefRW<Health>>())
            {
                health.ValueRW.amount = health.ValueRO.max;
                health.ValueRW.onHealthChanged = true;
            }
            
            state.EntityManager.SetComponentEnabled<OnWaveChanged>(eventsHandlerEntity, true);
        }
    }
}