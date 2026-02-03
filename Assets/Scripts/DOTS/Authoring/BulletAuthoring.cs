using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authoring
{
    public class BulletAuthoring : MonoBehaviour
    {
        public class Baker : Baker<BulletAuthoring>
        {
            public override void Bake(BulletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Bullet
                {
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
}