using Unity.Entities;
using UnityEngine;

public class PoolableGameObjectAuthoring : MonoBehaviour
{
    public PoolName poolName;
    private class Baker : Baker<PoolableGameObjectAuthoring>
    {
        public override void Bake(PoolableGameObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PoolableGameObject()
            {
                poolName = authoring.poolName
            });
        }
    }
}

public struct PoolableGameObject : IComponentData
{
    public PoolName poolName;
}

public class GameObjectCleanup : ICleanupComponentData
{
    public PoolName poolname;
    public Transform transform;
}