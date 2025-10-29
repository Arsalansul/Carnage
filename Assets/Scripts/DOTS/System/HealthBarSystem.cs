using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))]
internal partial struct HealthBarSystem : ISystem
{
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var cameraForward = Vector3.zero;
        if (Camera.main != null) cameraForward = Camera.main.transform.forward;

        foreach (var (healthBar, localTransform) in
                 SystemAPI.Query<RefRO<HealthBar>, RefRW<LocalTransform>>())
        {
            var parentLocalTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);
            if (localTransform.ValueRO.Scale > 0f)
                localTransform.ValueRW.Rotation =
                    parentLocalTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));

            var healthEntity = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

            if (!healthEntity.onHealthChanged) continue;
            var healthNormalized = (float)healthEntity.amount / healthEntity.max;

            // localTransform.ValueRW.Scale = healthNormalized == 1f ? 0 : 1;

            var barVisualPostTransformMatrix =
                SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);
        }
    }
}