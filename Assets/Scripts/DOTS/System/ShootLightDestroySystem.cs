using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

internal partial struct ShootLightDestroySystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        var job = new SpotLightDestroyJob()
        {
            entityCommandBuffer = entityCommandBuffer.AsParallelWriter(),
            deltaTime = SystemAPI.Time.DeltaTime
        };
        job.ScheduleParallel();
    }
}

public partial struct SpotLightDestroyJob : IJobEntity
{
    [ReadOnly] public float deltaTime;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;
    public void Execute(ref ShootLight shootLight, in Entity entity, [ChunkIndexInQuery] int chunkIndex)
    {
        shootLight.timer -= deltaTime;
        if (shootLight.timer <= 0f) entityCommandBuffer.DestroyEntity(chunkIndex, entity);
    }
}