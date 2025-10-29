using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

internal partial struct RandomWalkingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var randomWalkingJob = new RandomWalkingJob();
        randomWalkingJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct RandomWalkingJob : IJobEntity
{
    public void Execute(in LocalTransform localTransform, ref RandomWalking randomWalking, ref UnitMover unitMover)
    {
        if (math.distancesq(localTransform.Position, randomWalking.targetPosition) <=
            UnitMoverSystem.ReachedTargetPositionDistanceSQ)
        {
            var random = randomWalking.random;
            var randomDirection = new float3(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
            randomDirection = math.normalize(randomDirection);
            randomWalking.targetPosition =
                randomWalking.originPosition +
                randomDirection * random.NextFloat(randomWalking.distanceMin, randomWalking.distanceMax);

            randomWalking.random = random;
        }
        else
        {
            unitMover.targetPosition = randomWalking.targetPosition;
            unitMover.lookPosition = randomWalking.targetPosition;
        }
    }
}