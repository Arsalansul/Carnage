using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(DamageOnTriggerSystem))]
public partial struct SphereDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var gameConfig = SystemAPI.GetSingleton<GameConfigComponent>();

        var job = new SphereDamageJob
        {
            Enemies = SystemAPI.GetComponentLookup<Enemy>(true),
            Healths = SystemAPI.GetComponentLookup<Health>(true),
            gameConfig = gameConfig,
            physicsWorld = physicsWorld,
            ecb = ecb.AsParallelWriter()
        };
        
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct SphereDamageJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<Enemy> Enemies;
    [ReadOnly] public ComponentLookup<Health> Healths;
    [ReadOnly] public GameConfigComponent gameConfig;
    [ReadOnly] public PhysicsWorldSingleton physicsWorld;

    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(ref SphereDamage sphereDamage, in LocalTransform localTransform, in Entity entity,
        [ChunkIndexInQuery] int chunkIndex)
    {
        var explosionHits = new NativeList<DistanceHit>(Allocator.Temp);
        var collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << gameConfig.UnitsLayer,
            GroupIndex = 0
        };

        physicsWorld.OverlapSphere(localTransform.Position,
            sphereDamage.ExplosionRadius,
            ref explosionHits,
            collisionFilter);

        foreach (var hit in explosionHits)
        {
            if (Enemies.TryGetComponent(hit.Entity, out var enemy) &&
                Healths.TryGetComponent(hit.Entity, out var targetHealth))
            {
                targetHealth.amount -= sphereDamage.Damage;
                targetHealth.onHealthChanged = true;
                ecb.SetComponent(chunkIndex, hit.Entity, targetHealth);
            }
        }

        explosionHits.Dispose();
        ecb.DestroyEntity(chunkIndex, entity);
    }
}