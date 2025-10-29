using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct ShootLightSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new ShootLightJob()
        {
            entitiesReferences = entitiesReferences ,
            entityCommandBuffer = ecb.AsParallelWriter()
        };
        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ShootLightJob : IJobEntity
{
    [ReadOnly] public EntitiesReferences entitiesReferences;
    public EntityCommandBuffer.ParallelWriter entityCommandBuffer;

    public void Execute(in ShootAttack shootAttack, [ChunkIndexInQuery] int chunkIndex)
    {
        if (shootAttack.onShoot.isTrigger)
        {
            var shootLight = entityCommandBuffer.Instantiate(chunkIndex, entitiesReferences.shootLightPrefabEntity);
            entityCommandBuffer.SetComponent(chunkIndex, shootLight, LocalTransform.FromPosition(shootAttack.onShoot.shootFromPosition));
        }
    }
}