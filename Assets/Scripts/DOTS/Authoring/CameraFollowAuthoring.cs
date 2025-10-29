using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollowAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float3 offset;

    public class Baker : Baker<CameraFollowAuthoring>
    {
        public override void Bake(CameraFollowAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CameraFollow
            {
                moveSpeed = authoring.moveSpeed,
                offset = authoring.offset
            });
        }
    }
}

public struct CameraFollow : IComponentData
{
    public float moveSpeed;
    public float3 offset;
}