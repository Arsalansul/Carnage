using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using RaycastHit = Unity.Physics.RaycastHit;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderFirst = true)]
internal partial struct MeleeAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var collisionWorld = physicsWorldSingleton.CollisionWorld;
        var raycastHitList = new NativeList<RaycastHit>(Allocator.Temp);

        foreach (var (localTransform, meleeAttack, target, unitMover)
                 in SystemAPI.Query<RefRO<LocalTransform>, RefRW<MeleeAttack>, RefRO<Target>, RefRW<UnitMover>>())
        {
            if (target.ValueRO.targetEntity == Entity.Null) continue;

            var targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            var isCloseEnoughToAttack =
                math.distancesq(localTransform.ValueRO.Position, targetLocalTransform.Position) <=
                meleeAttack.ValueRO.attackDistance;
            var isTouchingTarget = false;

            if (!isCloseEnoughToAttack)
            {
                var dirToTarget = targetLocalTransform.Position - localTransform.ValueRO.Position;
                dirToTarget = math.normalize(dirToTarget);
                var distanceExtraToTestRaycast = 0.4f;
                var raycastInput = new RaycastInput
                {
                    Start = localTransform.ValueRO.Position,
                    End = localTransform.ValueRO.Position +
                          dirToTarget * (meleeAttack.ValueRO.colliderSize + distanceExtraToTestRaycast),
                    Filter = CollisionFilter.Default
                };
                raycastHitList.Clear();
                if (collisionWorld.CastRay(raycastInput, ref raycastHitList))
                    foreach (var raycastHit in raycastHitList)
                        if (raycastHit.Entity == target.ValueRO.targetEntity)
                        {
                            isTouchingTarget = true;
                            break;
                        }
            }

            if (!isCloseEnoughToAttack && !isTouchingTarget)
            {
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                unitMover.ValueRW.lookPosition = targetLocalTransform.Position;
            }
            else
            {
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
                unitMover.ValueRW.lookPosition = targetLocalTransform.Position;

                meleeAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (meleeAttack.ValueRO.timer > 0f) continue;

                meleeAttack.ValueRW.timer = meleeAttack.ValueRO.timerMax;
                meleeAttack.ValueRW.animateAttack = true;

                var targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.amount -= meleeAttack.ValueRO.damage;
                targetHealth.ValueRW.onHealthChanged = true;
            }
        }
    }
}