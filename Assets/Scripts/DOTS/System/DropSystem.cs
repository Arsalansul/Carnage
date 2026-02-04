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
            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            foreach (var (tryDrop, enabledTryDrop) in SystemAPI.Query<RefRO<TryDropItem>, EnabledRefRW<TryDropItem>>())
            {
                enabledTryDrop.ValueRW = false;
                if (random.NextFloat() > dropSettings.chance) continue;
                
                var pickupEntity = state.EntityManager.Instantiate(entitiesReferences.pickupPrefab);
                SystemAPI.SetComponent(pickupEntity, LocalTransform.FromPosition(tryDrop.ValueRO.position));
            }
        }
        
        // [BurstCompile]
        // private Entity GetNextPickupEntity(ref SystemState state)
        // {
        //     var entitiesReferencesEntity = SystemAPI.GetSingletonEntity<EntitiesReferences>();
        //
        //     // var index = random.NextInt(0, waveBlob.Array.Length - 1);
        //     //
        //     // for (int i = 0; i < waveBlob.Array.Length; i++)
        //     // {
        //     //     if (waveBlob.Array[index].count > 0)
        //     //         break;
        //     //     index = (index + 1) % waveBlob.Array.Length;
        //     // }
        //     //
        //     // enemyInWaveConf = waveBlob.Array[index];
        //     //
        //     // if (waveBlob.Array[index].count <= 0)
        //     // {
        //     //     return Entity.Null;
        //     // }
        //     //
        //     // waveBlob.Array[index].count--;
        //     //
        //     // var enemiesMap = state.EntityManager.GetBuffer<EnemiesEntityMap>(entitiesReferencesEntity);
        //     //
        //     // for (int i = 0; i < enemiesMap.Length; i++)
        //     // {
        //     //     if (enemiesMap[i].type == waveBlob.Array[index].type)
        //     //     {
        //     //         return enemiesMap[i].entity;
        //     //     }
        //     // }
        //     //
        //     // throw new Exception("enemy not found in map");
        // }
    }
}