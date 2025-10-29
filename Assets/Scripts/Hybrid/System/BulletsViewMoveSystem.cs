using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
public partial struct BulletsViewMoveSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, bulletCleanup) in SystemAPI.Query<RefRW<LocalTransform>, BulletCleanup>())
        {
            bulletCleanup.transform.position = localTransform.ValueRO.Position;
        }
    }
}