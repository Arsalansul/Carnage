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
            ref var pickupSettingsArray = ref config.pickupSettings.Value.Array;
            
            var entitiesReferencesEntity = SystemAPI.GetSingletonEntity<EntitiesReferences>();

            foreach (var (tryDrop, enabledTryDrop) in SystemAPI.Query<RefRO<TryDropItem>, EnabledRefRW<TryDropItem>>())
            {
                enabledTryDrop.ValueRW = false;
                if (random.NextFloat() > dropSettings.chance) continue;
                
                var nextType = GetNextDrop<PickupSettings>(ref pickupSettingsArray);

                var nextEntity = Entity.Null;
                
                if (nextType.type == PickupType.weapon)
                {
                    var bufferWeapons = SystemAPI.GetBuffer<WeaponsEntityMap>(entitiesReferencesEntity);
                    ref var weaponBlob = ref config.Weapons.Value;
                    
                    nextEntity = GetNextWeaponEntity(bufferWeapons, ref weaponBlob);
                }
                else
                {
                    var bufferConsumables = SystemAPI.GetBuffer<ConsumablesEntityMap>(entitiesReferencesEntity);
                    nextEntity = GetNextConsumablesEntity(bufferConsumables, nextType.type);
                }
                
                var pickupEntity = state.EntityManager.Instantiate(nextEntity);
                var pickup = SystemAPI.GetComponentRW<Pickup>(pickupEntity);
                
                pickup.ValueRW.type = nextType.type;

                if (nextType.type == PickupType.bomb)
                {
                    SystemAPI.SetComponentEnabled<SphereDamage>(pickupEntity, false);
                }
                
                SystemAPI.SetComponent(pickupEntity, LocalTransform.FromPosition(tryDrop.ValueRO.position));
            }
        }
        
        [BurstCompile]
        private T GetNextDrop<T>(ref BlobArray<T> array) where T : unmanaged, IDropWeight
        {
            var sumWeight = 0;
            for (int i = 0; i < array.Length; i++)
            {
                sumWeight += array[i].Weight;
            }

            var randomValue = random.NextFloat() * sumWeight;

            var currentWeight = 0f;
            for (int i = 0; i < array.Length; i++)
            {
                currentWeight += array[i].Weight;
                
                if (randomValue <= currentWeight) return array[i];
            }
            
            return array[0];
        }

        [BurstCompile]
        private Entity GetNextWeaponEntity(DynamicBuffer<WeaponsEntityMap> weaponsBuffer, ref WeaponsBlob weaponsBlob)
        {
            var settings = GetNextDrop<WeaponBlob>(ref weaponsBlob.Array);

            foreach (var item in weaponsBuffer)
            {
                if (item.type == settings.type)
                {
                    return item.entity;
                }
            }
            return Entity.Null;
        }
        
        [BurstCompile]
        private Entity GetNextConsumablesEntity(DynamicBuffer<ConsumablesEntityMap> consumablesBuffer, PickupType pickupType)
        {
            foreach (var item in consumablesBuffer)
            {
                if (item.type == pickupType)
                {
                    return item.entity;
                }
            }
            
            return Entity.Null;
        }
    }
}