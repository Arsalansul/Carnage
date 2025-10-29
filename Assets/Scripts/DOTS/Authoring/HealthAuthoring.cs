using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    [Range(0, 100)] public int amount;
    [Range(0, 100)] public int max;

    public class Baker : Baker<HealthAuthoring>
    {
        public override void Bake(HealthAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Health
            {
                amount = authoring.amount,
                max = authoring.max,
                onHealthChanged = true
            });
        }
    }
}

public struct Health : IComponentData
{
    public int amount;
    public int max;
    public bool onHealthChanged;
}