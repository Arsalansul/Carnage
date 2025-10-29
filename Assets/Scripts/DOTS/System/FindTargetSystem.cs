using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

internal partial struct FindTargetSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var gameConfig = SystemAPI.GetSingleton<GameConfigComponent>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;
        var distanceHitList = new NativeList<DistanceHit>(Allocator.Temp);
        var collisionFilter = new CollisionFilter
        {
            BelongsTo = ~0u,
            CollidesWith = 1u << gameConfig.UnitsLayer,
            GroupIndex = 0
        };
        foreach (var (localTransform, findTarget, target) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>, RefRW<Target>>())
        {
            findTarget.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (findTarget.ValueRO.timer > 0) continue;

            findTarget.ValueRW.timer = findTarget.ValueRO.timerMax;

            distanceHitList.Clear();
            if (collisionWorld.OverlapSphere(localTransform.ValueRO.Position, findTarget.ValueRO.range,
                    ref distanceHitList, collisionFilter))
                foreach (var distanceHit in distanceHitList)
                {
                    if (!SystemAPI.Exists(distanceHit.Entity) ||
                        !SystemAPI.HasComponent<Unit>(distanceHit.Entity)) continue;

                    var targetUnit = SystemAPI.GetComponent<Unit>(distanceHit.Entity);
                    if (targetUnit.faction == findTarget.ValueRO.targetFaction)
                    {
                        target.ValueRW.targetEntity = distanceHit.Entity;
                        break;
                    }
                }
        }
    }
}