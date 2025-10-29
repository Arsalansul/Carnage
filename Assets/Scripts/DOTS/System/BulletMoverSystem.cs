using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct BulletMoverSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var job = new BulletMoveJob()
        {
            entityCommandBuffer = entityCommandBuffer.AsParallelWriter(),
            deltaTime = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct BulletMoveJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;

    public void Execute(
        ref LocalTransform localTransform, 
        ref Bullet bullet, 
        in SphereDamage sphereDamage, 
        in Entity entity, 
        [ChunkIndexInQuery] int chunkIndex)
    {
        if (sphereDamage.onHit)
        {
            entityCommandBuffer.DestroyEntity(chunkIndex, entity);
            return;
        }

        var moveDirection = bullet.direction;
        moveDirection = math.normalize(moveDirection);

        localTransform.Position += moveDirection * bullet.speed * deltaTime;

        var moveDistance = math.length(moveDirection * bullet.speed * deltaTime);
        bullet.maxDistance -= moveDistance;

        if (bullet.maxDistance <= 0) entityCommandBuffer.DestroyEntity(chunkIndex, entity);
    }
}