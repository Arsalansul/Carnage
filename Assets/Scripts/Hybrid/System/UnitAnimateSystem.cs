using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace System
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial struct UnitAnimateSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (localTransform, animatorReference, unitMover) in
                     SystemAPI.Query<RefRO<LocalTransform>, UnitGameObjectReference, RefRO<UnitMover>>())
            {
                animatorReference.unitView.Move(unitMover.ValueRO.moveSpeed);
                animatorReference.unitView.SetTransform(localTransform.ValueRO.Position,
                    localTransform.ValueRO.Rotation);
            }

            foreach (var (localTransform, animatorReference, unitMover, moveInput) in
                     SystemAPI
                         .Query<RefRO<LocalTransform>, UnitGameObjectReference, RefRO<UnitMover>, RefRO<MoveInput>>())
            {
                var inputData = SystemAPI.GetSingleton<InputData>();

                var speed = math.length(inputData.Movement) * unitMover.ValueRO.moveSpeed;
                animatorReference.unitView.Move(speed);

                if (inputData.MouseLeft) animatorReference.unitView.Attack();

                if (inputData.MouseRight)
                {
                    var playerView = (PlayerView)animatorReference.unitView;
                    playerView.ShowWeapon(inputData.WeaponIndex);
                }

                if (speed < 0.1f) continue;
                var lookDirection = inputData.MousePos - localTransform.ValueRO.Position;
                var moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;

                var angle = GetAngleBetweenFloat3(lookDirection, moveDirection);

                animatorReference.unitView.SetLookAngle(angle, math.cross(lookDirection, moveDirection).y > 0);
            }

            foreach (var (health, animatorReference, entity) in
                     SystemAPI.Query<RefRO<Health>, UnitGameObjectReference>().WithEntityAccess())
            {
                if (!health.ValueRO.onHealthChanged) continue;

                animatorReference.unitView.TakeDamage();
                animatorReference.unitView.Dead(health.ValueRO.amount <= 0);
            }

            foreach (var (meleeAttack, animatorReference)
                     in SystemAPI.Query<RefRO<MeleeAttack>, UnitGameObjectReference>())
                if (meleeAttack.ValueRO.animateAttack)
                    animatorReference.unitView.Attack();

            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }

        private float GetAngleBetweenFloat3(float3 vectorA, float3 vectorB)
        {
            var dotProduct = math.dot(vectorA, vectorB);
            var magnitudeA = math.length(vectorA);
            var magnitudeB = math.length(vectorB);
            var cosAngle = dotProduct / (magnitudeA * magnitudeB);
            cosAngle = math.clamp(cosAngle, -1f, 1f);
            var angleRadians = math.acos(cosAngle);
            var angleDegrees = math.degrees(angleRadians);

            return angleDegrees;
        }
    }
}