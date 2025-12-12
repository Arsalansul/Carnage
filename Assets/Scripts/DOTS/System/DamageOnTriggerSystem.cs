using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct DamageOnTriggerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = new OnDamageTriggerEventsJob()
        {
            units = SystemAPI.GetComponentLookup<Unit>(true),
            healths = SystemAPI.GetComponentLookup<Health>(),
            damages = SystemAPI.GetComponentLookup<DamageOnTrigger>(),
        }.Schedule(simulation, state.Dependency);
        
        var cleanupJob = new CleanupEntitiesJob()
        {
            ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            sphereDamageLookup = SystemAPI.GetComponentLookup<SphereDamage>(true),
        };
        cleanupJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct OnDamageTriggerEventsJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<Unit> units;
    public ComponentLookup<Health> healths;
    public ComponentLookup<DamageOnTrigger> damages;
    
    public void Execute(TriggerEvent triggerEvent)
    {
        var entityA = triggerEvent.EntityA;
        var entityB = triggerEvent.EntityB;
        
        if (!healths.EntityExists(entityA) && !healths.EntityExists(entityB)) return;
        if (!damages.EntityExists(entityA) && !damages.EntityExists(entityB)) return;

        if (healths.TryGetRefRW(entityB, out var targetHealth) && units.TryGetRefRO(entityB, out var unit) && damages.TryGetRefRW(entityA, out var damageOnTrigger) ||
            healths.TryGetRefRW(entityA, out targetHealth) && units.TryGetRefRO(entityA, out unit) && damages.TryGetRefRW(entityB, out damageOnTrigger))
        {
            if (damageOnTrigger.ValueRO.damageTargetFaction != unit.ValueRO.faction)
            {
                return;
            }
            damageOnTrigger.ValueRW.triggered = true;
            targetHealth.ValueRW.amount -= damageOnTrigger.ValueRO.amount;
            targetHealth.ValueRW.onHealthChanged = true;
        }
    }
}

[BurstCompile]
public partial struct CleanupEntitiesJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter ecb;
    [ReadOnly] public ComponentLookup<SphereDamage> sphereDamageLookup;
    public void Execute(ref DamageOnTrigger damageOnTrigger, [ChunkIndexInQuery] int chunkIndex, Entity entity)
    {
        if (!damageOnTrigger.triggered) return;

        if (sphereDamageLookup.HasComponent(entity))
        {
            ecb.SetComponentEnabled<SphereDamage>(chunkIndex, entity, true);
        }
        else
        {
            ecb.DestroyEntity(chunkIndex, entity);
        }
    }
}