using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

internal partial struct UnitMoverSystem : ISystem
{
    public const float ReachedTargetPositionDistanceSQ = 0.1f;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var unitMoverJob = new UnitMoverJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        unitMoverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity
{
    [ReadOnly] public float deltaTime;

    public void Execute(ref LocalTransform localTransform, ref UnitMover unitMover, ref PhysicsVelocity physicsVelocity)
    {
        var moveDirection = unitMover.targetPosition - localTransform.Position;
        var lookDirection = unitMover.lookPosition - localTransform.Position;
        lookDirection.y = 0;

        if (math.lengthsq(lookDirection) > UnitMoverSystem.ReachedTargetPositionDistanceSQ)
        {
            localTransform.Rotation = math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(lookDirection, math.up()),
                deltaTime * unitMover.rotationSpeed);
        }

        unitMover.reachedTarget = math.lengthsq(moveDirection) <= UnitMoverSystem.ReachedTargetPositionDistanceSQ;
        
        if (unitMover.reachedTarget)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

        moveDirection = math.normalize(moveDirection);

        // localTransform.Rotation = math.slerp(localTransform.Rotation, 
        //     quaternion.LookRotation(moveDirection, math.up()), 
        //     deltaTime * unitMover.rotationSpeed);

        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
        // localTransform.Position += moveDirection * deltaTime * unitMover.moveSpeed;
    }
}