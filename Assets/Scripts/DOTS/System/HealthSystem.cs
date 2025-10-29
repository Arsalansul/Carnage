using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
internal partial struct HealthSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var healthJob = new HealthJob
        {
            entityCommandBuffer = entityCommandBuffer.AsParallelWriter()
        };

        healthJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct HealthJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;

    public void Execute([ChunkIndexInQuery] int chunkIndex, in Health health, in Entity entity)
    {
        if (health.amount <= 0) entityCommandBuffer.DestroyEntity(chunkIndex, entity);
    }
}