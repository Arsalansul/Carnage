using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;

namespace DOTS.System
{
    public partial struct OnPickupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (onPickup, enabledOnPickup) in SystemAPI.Query<RefRO<OnPickup>, EnabledRefRW<OnPickup>>())
            {
                var playerEntity = SystemAPI.GetSingletonEntity<Player>();

                switch (onPickup.ValueRO.pickupType)
                {
                    case PickupType.heal:
                        var playerHealth = SystemAPI.GetComponentRW<Health>(playerEntity);
                        Heal(playerHealth);
                        break;
                    case PickupType.bomb:
                    case PickupType.chainLightning:
                    default:
                        break;
                }

                enabledOnPickup.ValueRW = false;
            }
        }

        [BurstCompile]
        private void Heal(RefRW<Health> health)
        {
            health.ValueRW.amount = health.ValueRO.max;
            health.ValueRW.onHealthChanged = true;
        }
    }
}