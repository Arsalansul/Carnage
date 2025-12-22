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
        new PlayAudioClipOnDamageEnableJob().ScheduleParallel();
    }
}

[BurstCompile]
[WithDisabled(typeof(PlayAudioClipOnDamageData))]
public partial struct PlayAudioClipOnDamageEnableJob : IJobEntity
{
    public void Execute(in Health health, EnabledRefRW<PlayAudioClipOnDamageData> playClip)
    {
        if (!health.onHealthChanged || health.amount >= health.max) return;

        playClip.ValueRW = true;
    }
}