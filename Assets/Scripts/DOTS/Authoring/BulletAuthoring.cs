using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BulletAuthoring : MonoBehaviour
{
    public float speed;
    public float maxDistance;

    public class Baker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Bullet
            {
                speed = authoring.speed,
                maxDistance = authoring.maxDistance
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float speed;
    public float3 direction;
    public float maxDistance;
}