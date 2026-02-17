using Hybrid.Authoring;
using Unity.Entities;
using Unity.Transforms;

namespace Hybrid.System
{
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
}