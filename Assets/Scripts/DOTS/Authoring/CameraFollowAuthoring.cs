using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollowAuthoring : MonoBehaviour
{
    public class Baker : Baker<CameraFollowAuthoring>
    {
        public override void Bake(CameraFollowAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CameraFollow
            {
            });
        }
    }
}

public struct CameraFollow : IComponentData
{
    public float moveSpeed;
    public float3 offset;
}