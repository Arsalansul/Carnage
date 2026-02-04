using System;
using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace DOTS.System
{
    public partial struct DropSystem : ISystem
    {
        private Random random;
        
        // [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            random = new Random((uint)DateTime.Now.Ticks);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<GameConfigComponent>();
            var dropSettings = config.dropSettings;
            ref var pickupSettings = ref config.pickupSettings.Value.Array;
            
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (var (tryDrop, enabledTryDrop) in SystemAPI.Query<RefRO<TryDropItem>, EnabledRefRW<TryDropItem>>())
            {
                enabledTryDrop.ValueRW = false;
                if (random.NextFloat() > dropSettings.chance) continue;
                
                var pickupEntity = state.EntityManager.Instantiate(entitiesReferences.pickupPrefab);
                var pickup = SystemAPI.GetComponentRW<Pickup>(pickupEntity);
                
                pickup.ValueRW.type = GetNextPickupType(ref pickupSettings);
                
                SystemAPI.SetComponent(pickupEntity, LocalTransform.FromPosition(tryDrop.ValueRO.position));
            }
        }
        
        [BurstCompile]
        private PickupType GetNextPickupType(ref BlobArray<PickupSettings> pickupSettings)
        {
            var sumWeight = 0;
            for (int i = 0; i < pickupSettings.Length; i++)
            {
                sumWeight += pickupSettings[i].weight;
            }

            var randomValue = random.NextFloat() * sumWeight;

            var currentWeight = 0f;
            for (int i = 0; i < pickupSettings.Length; i++)
            {
                currentWeight += pickupSettings[i].weight;
                
                if (randomValue <= currentWeight) return pickupSettings[i].type;
            }
            
            return pickupSettings[0].type;
        }
    }
}