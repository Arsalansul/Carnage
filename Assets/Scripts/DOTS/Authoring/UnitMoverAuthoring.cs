using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authoring
{
    public class UnitMoverAuthoring : MonoBehaviour
    {

        public class Baker : Baker<UnitMoverAuthoring>
        {
            public override void Bake(UnitMoverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitMover
                {
                });
            }
        }
    }

    public struct UnitMover : IComponentData
    {
        public float moveSpeed;
        public float rotationSpeed;
        public float3 targetPosition;
        public float3 lookPosition;
        public bool reachedTarget;
    }
}