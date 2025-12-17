using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class RandomWalkingAuthoring : MonoBehaviour
{
    public float distanceMin;
    public float distanceMax;

    public class Baker : Baker<RandomWalkingAuthoring>
    {
        public override void Bake(RandomWalkingAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomWalking
            {
                distanceMin = authoring.distanceMin,
                distanceMax = authoring.distanceMax
            });
            SetComponentEnabled<RandomWalking>(entity, true);
        }
    }
}

public struct RandomWalking : IComponentData, IEnableableComponent
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    public Random random;
}