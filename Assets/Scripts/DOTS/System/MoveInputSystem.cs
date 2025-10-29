using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace System
{
    public partial struct MoveInputSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (moveInput, unitMover, localTransform) in
                     SystemAPI.Query<RefRO<MoveInput>, RefRW<UnitMover>, RefRO<LocalTransform>>())
            {
                var inputData = SystemAPI.GetSingleton<InputData>();
                var moveDirection = new float3(inputData.Movement.x, 0, inputData.Movement.y);
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position + moveDirection;
                unitMover.ValueRW.lookPosition = inputData.MousePos;
            }
        }
    }
}