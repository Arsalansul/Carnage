using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

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
                        Debug.Log("Boom");
                        break;
                    case PickupType.chainLightning:
                        Debug.Log("ChainLightning");
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