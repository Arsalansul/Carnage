using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct SphereDamageSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var ecbSingleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var gameConfig = SystemAPI.GetSingleton<GameConfigComponent>();

        var job = new SphereDamageJob
        {
            Enemies = SystemAPI.GetComponentLookup<Enemy>(true),
            Healths = SystemAPI.GetComponentLookup<Health>(true),
            gameConfig = gameConfig,
            physicsWorld = physicsWorld,
            ECB = ecb.AsParallelWriter()
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

    public EntityCommandBuffer.ParallelWriter ECB;

    public void Execute(ref SphereDamage sphereDamage, in LocalTransform localTransform, in Entity entity,
        [ChunkIndexInQuery] int chunkIndex)
    {
        var triggerHits = new NativeList<DistanceHit>(Allocator.Temp);
        var explosionHits = new NativeList<DistanceHit>(Allocator.Temp);
        var collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << gameConfig.UnitsLayer,
            GroupIndex = 0
        };
        physicsWorld.OverlapSphere(localTransform.Position,
            sphereDamage.TriggerRadius,
            ref triggerHits,
            collisionFilter);

        if (triggerHits.IsEmpty) return;

        physicsWorld.OverlapSphere(localTransform.Position,
            sphereDamage.ExplosionRadius,
            ref explosionHits,
            collisionFilter);

        foreach (var hit in explosionHits)
            if (Enemies.TryGetComponent(hit.Entity, out var enemy) &&
                Healths.TryGetComponent(hit.Entity, out var targetHealth))
            {
                targetHealth.amount -= sphereDamage.Damage;
                targetHealth.onHealthChanged = true;
                ECB.SetComponent(chunkIndex, hit.Entity, targetHealth);

                sphereDamage.onHit = true;
            }

        explosionHits.Dispose();
        triggerHits.Dispose();
    }
}