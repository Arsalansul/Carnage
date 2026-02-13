using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct CleanObjectViewMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, cleanup) in SystemAPI.Query<RefRW<LocalTransform>, GameObjectCleanup>())
        {
            cleanup.transform.position = localTransform.ValueRO.Position;
        }
    }
}