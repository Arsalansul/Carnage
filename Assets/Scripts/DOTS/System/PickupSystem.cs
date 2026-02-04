using DOTS.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS.System
{
    public partial struct PickupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = new OnPickupTriggerEventsJob()
            {
                playerLookup = SystemAPI.GetComponentLookup<Player>(true),
                pickupLookup = SystemAPI.GetComponentLookup<Pickup>()
            }.Schedule(simulation, state.Dependency);
            
            var cleanupJob = new CleanupPickupEntitiesJob()
            {
                ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                eventsHandlerEntity = SystemAPI.GetSingletonEntity<EventsHandler>()
            };
            cleanupJob.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct OnPickupTriggerEventsJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<Player> playerLookup;
        public ComponentLookup<Pickup> pickupLookup;
    
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            if (playerLookup.TryGetRefRO(entityA, out var player) &&  pickupLookup.TryGetRefRW(entityB, out var pickup) || 
                playerLookup.TryGetRefRO(entityB, out player) && pickupLookup.TryGetRefRW(entityA, out pickup))
            {
                pickup.ValueRW.triggered = true;
            }
        }
    }

    [BurstCompile]
    public partial struct CleanupPickupEntitiesJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public Entity eventsHandlerEntity;
        
        public void Execute(ref Pickup pickup, in LocalTransform localTransform, [ChunkIndexInQuery] int chunkIndex, Entity entity)
        {
            if (!pickup.triggered) return;
            
            ecb.SetComponent(chunkIndex, eventsHandlerEntity, new OnPickup
            {
                pickupType = pickup.type,
                position = localTransform.Position
            });
            ecb.SetComponentEnabled<OnPickup>(chunkIndex, eventsHandlerEntity, true);
            ecb.DestroyEntity(chunkIndex, entity);
        }
    }
}