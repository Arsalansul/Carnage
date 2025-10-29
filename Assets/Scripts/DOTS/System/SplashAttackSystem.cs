using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace System
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct SplashAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (localTransform, splashAttack)
                     in SystemAPI.Query<RefRO<LocalTransform>, RefRW<SplashAttack>>())
            {
                if (splashAttack.ValueRO.timer > 0f) splashAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (splashAttack.ValueRO.timer > 0f) continue;

                var inputData = SystemAPI.GetSingleton<InputData>();

                if (inputData.MouseLeft)
                {
                    splashAttack.ValueRW.timer = splashAttack.ValueRO.timerMax;
                    var gameConfig = SystemAPI.GetSingleton<GameConfigComponent>();

                    var allHits = new NativeList<DistanceHit>(Allocator.Temp);
                    var collisionFilter = new CollisionFilter
                    {
                        BelongsTo = ~0u,
                        CollidesWith = 1u << gameConfig.UnitsLayer,
                        GroupIndex = 0
                    };
                    physicsWorld.OverlapSphere(localTransform.ValueRO.Position,
                        splashAttack.ValueRO.raidus,
                        ref allHits,
                        collisionFilter);

                    foreach (var hit in allHits)
                    {
                        var unit = SystemAPI.GetComponent<Unit>(hit.Entity);
                        if (unit.faction == Faction.Enemy)
                        {
                            var targetHealth = SystemAPI.GetComponentRW<Health>(hit.Entity);
                            targetHealth.ValueRW.amount -= splashAttack.ValueRO.damage;
                            targetHealth.ValueRW.onHealthChanged = true;
                        }
                    }

                    allHits.Dispose();
                }
            }
        }
    }
}