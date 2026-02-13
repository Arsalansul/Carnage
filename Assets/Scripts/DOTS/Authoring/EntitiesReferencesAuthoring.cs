using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class EntitiesReferencesAuthoring : MonoBehaviour
    {
        public List<EnemyPrefabsMap> enemyMap;
        public GameObject playerPrefab;
        public List<BulletsPrefabsMap> bulletsMaps;
        public GameObject shootLightPrefab;
        public List<ConsumablesMap> consumablesMap;

        public class Baker : Baker<EntitiesReferencesAuthoring>
        {
            public override void Bake(EntitiesReferencesAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntitiesReferences
                {
                    playerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                    shootLightPrefabEntity = GetEntity(authoring.shootLightPrefab, TransformUsageFlags.Dynamic),
                });
            
                var enemyBuffer = AddBuffer<EnemiesEntityMap>(entity);
                for (int i = 0; i < authoring.enemyMap.Count; i++)
                {
                    enemyBuffer.Add(new EnemiesEntityMap()
                    {
                        type = authoring.enemyMap[i].type,
                        entity = GetEntity(authoring.enemyMap[i].prefab, TransformUsageFlags.Dynamic)
                    });
                }
            
                var bulletsBuffer = AddBuffer<BulletsEntityMap>(entity);
                for (int i = 0; i < authoring.bulletsMaps.Count; i++)
                {
                    bulletsBuffer.Add(new BulletsEntityMap()
                    {
                        type = authoring.bulletsMaps[i].type,
                        entity = GetEntity(authoring.bulletsMaps[i].prefab, TransformUsageFlags.Dynamic)
                    });
                }
                
                var consumablesBuffer = AddBuffer<ConsumablesEntityMap>(entity);
                for (int i = 0; i < authoring.consumablesMap.Count; i++)
                {
                    consumablesBuffer.Add(new ConsumablesEntityMap()
                    {
                        type = authoring.consumablesMap[i].type,
                        entity = GetEntity(authoring.consumablesMap[i].prefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }

    public struct EntitiesReferences : IComponentData
    {
        public Entity playerPrefab;
        public Entity shootLightPrefabEntity;
    }

    [Serializable]
    public struct BulletsPrefabsMap
    {
        public BulletsType type;
        public GameObject prefab;
    }

    public struct BulletsEntityMap : IBufferElementData
    {
        public BulletsType type;
        public Entity entity;
    }

    [Serializable]
    public struct EnemyPrefabsMap
    {
        public EnemyType type;
        public GameObject prefab;
    }

    public struct EnemiesEntityMap : IBufferElementData
    {
        public EnemyType type;
        public Entity entity;
    }

    [Serializable]
    public struct ConsumablesMap
    {
        public PickupType type;
        public GameObject prefab;
    }

    public struct ConsumablesEntityMap : IBufferElementData
    {
        public PickupType type;
        public Entity entity;
    }
}