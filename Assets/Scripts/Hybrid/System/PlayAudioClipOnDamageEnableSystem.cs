using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateAfter(typeof(TransformSystemGroup))]
[UpdateBefore(typeof(LateSimulationSystemGroup))]
public partial struct PlayAudioClipOnDamageEnableSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (health, playClip) in
                 SystemAPI.Query<RefRO<Health>, EnabledRefRW<PlayAudioClipOnDamageData>>().WithDisabled<PlayAudioClipOnDamageData>())
        {
            if (!health.ValueRO.onHealthChanged || health.ValueRO.amount >= health.ValueRO.max) continue;
            playClip.ValueRW = true;
        }
    }
}