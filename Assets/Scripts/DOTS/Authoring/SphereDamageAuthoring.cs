using Unity.Entities;
using UnityEngine;

public class SphereDamageAuthoring : MonoBehaviour
{
    private class Baker : Baker<SphereDamageAuthoring>
    {
        public override void Bake(SphereDamageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SphereDamage
            {
            });
            SetComponentEnabled<SphereDamage>(entity, false);
        }
    }
}

public struct SphereDamage : IComponentData, IEnableableComponent
{
    public int Damage;
    public float ExplosionRadius;
}