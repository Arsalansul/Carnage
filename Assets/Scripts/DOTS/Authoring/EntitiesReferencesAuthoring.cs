using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public GameObject playerPrefab;
    public List<GameObject> bulletPrefabs;
    public GameObject shootLightPrefab;
    public int UnitsLayer;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                enemyPrefab_0 = GetEntity(authoring.enemyPrefabs[0], TransformUsageFlags.Dynamic),
                enemyPrefab_1 = GetEntity(authoring.enemyPrefabs[1], TransformUsageFlags.Dynamic),
                playerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                bulletPrefabEntity_0 = GetEntity(authoring.bulletPrefabs[0], TransformUsageFlags.Dynamic),
                bulletPrefabEntity_1 = GetEntity(authoring.bulletPrefabs[1], TransformUsageFlags.Dynamic),
                shootLightPrefabEntity = GetEntity(authoring.shootLightPrefab, TransformUsageFlags.Dynamic),
                UnitsLayer = authoring.UnitsLayer
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity enemyPrefab_0;
    public Entity enemyPrefab_1;
    public Entity playerPrefab;
    public Entity bulletPrefabEntity_0;
    public Entity bulletPrefabEntity_1;
    public Entity shootLightPrefabEntity;
    public int UnitsLayer;
}