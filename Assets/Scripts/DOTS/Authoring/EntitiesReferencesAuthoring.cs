using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public List<EnemiesMap> enemyPrefabs;
    public GameObject playerPrefab;
    public List<BulletsMap> bulletsMaps;
    public GameObject shootLightPrefab;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                enemy_Arachnid = GetEntity(authoring.enemyPrefabs.Find(b => b.type == EnemyType.Arachnid).prefab, TransformUsageFlags.Dynamic),
                enemy_Cockroach = GetEntity(authoring.enemyPrefabs.Find(b => b.type == EnemyType.Cockroach).prefab, TransformUsageFlags.Dynamic),
                playerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                smallBulletPrefab = GetEntity(authoring.bulletsMaps.Find(b => b.type == BulletsType.small).prefab, TransformUsageFlags.Dynamic),
                explosionBulletPrefab = GetEntity(authoring.bulletsMaps.Find(b => b.type == BulletsType.explosion).prefab, TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity enemy_Arachnid;
    public Entity enemy_Cockroach;
    public Entity playerPrefab;
    public Entity smallBulletPrefab;
    public Entity explosionBulletPrefab;
    public Entity shootLightPrefabEntity;
}

[Serializable]
public struct BulletsMap
{
    public BulletsType type;
    public GameObject prefab;
}

[Serializable]
public struct EnemiesMap
{
    public EnemyType type;
    public GameObject prefab;
}