using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.System
{
    public partial struct OnPickupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityReferencesEntity = SystemAPI.GetSingletonEntity<EntitiesReferences>();
            var config = SystemAPI.GetSingleton<GameConfigComponent>();
            ref var pickupSettings = ref config.pickupSettings.Value.Array;
            
            foreach (var (onPickup, enabledOnPickup) in SystemAPI.Query<RefRO<OnPickup>, EnabledRefRW<OnPickup>>())
            {
                switch (onPickup.ValueRO.pickupType)
                {
                    case PickupType.heal:
                        var playerEntity = SystemAPI.GetSingletonEntity<Player>();
                        var playerHealth = SystemAPI.GetComponentRW<Health>(playerEntity);
                        Heal(playerHealth);
                        break;
                    case PickupType.bomb:
                        ActivateBomb(ref state, onPickup.ValueRO.position, SystemAPI.GetBuffer<ConsumablesEntityMap>(entityReferencesEntity), config.bombSettings);
                        break;
                    case PickupType.chainLightning:
                        ActivateChainLightning();
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

        [BurstCompile]
        private void ActivateBomb(ref SystemState state, float3 position, DynamicBuffer<ConsumablesEntityMap> consumables, BombConsumableSettings bombSettings)
        {
            foreach (var item in consumables)
            {
                if (item.type == PickupType.bomb)
                {
                    var entity = state.EntityManager.Instantiate(item.entity);
                    state.EntityManager.SetComponentData(entity, LocalTransform.FromPosition(position));
                    state.EntityManager.SetComponentData(entity, new SphereDamage()
                    {
                        Damage = bombSettings.damage,
                        ExplosionRadius = bombSettings.explosionRange
                    });
                    state.EntityManager.SetComponentEnabled<SphereDamage>(entity, true);
                    Debug.Log("bomb activated");
                    return;
                }
            }
            Debug.LogError("bomb not found in buffer");
        }

        [BurstCompile]
        private void ActivateChainLightning()
        {
            Debug.Log("chain lightning activated");
        }
    }
}