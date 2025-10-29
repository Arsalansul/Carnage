using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace System
{
    public partial struct CameraFollowSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (cameraTarget, localTransform) in
                     SystemAPI.Query<RefRO<CameraTarget>, RefRO<LocalTransform>>())
            {
                var cameraFollowEntity = SystemAPI.GetSingletonEntity<CameraFollow>();
                var cameraFollow = SystemAPI.GetComponentRW<CameraFollow>(cameraFollowEntity);
                var cameraFollowLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(cameraFollowEntity);

                var targetPosition = localTransform.ValueRO.Position + cameraFollow.ValueRO.offset;
                var moveDirection = targetPosition - cameraFollowLocalTransform.ValueRO.Position;
                if (math.lengthsq(moveDirection) <= UnitMoverSystem.ReachedTargetPositionDistanceSQ) return;
                moveDirection = math.normalize(moveDirection);
                cameraFollowLocalTransform.ValueRW.Position +=
                    moveDirection * SystemAPI.Time.DeltaTime * cameraFollow.ValueRO.moveSpeed;
            }
        }
    }
}