using Unity.Entities;
using UnityEngine;

public class SphereDamageAuthoring : MonoBehaviour
{
    public float explosionRadius;
    public int damage;

    private class Baker : Baker<SphereDamageAuthoring>
    {
        public override void Bake(SphereDamageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SphereDamage
            {
                ExplosionRadius = authoring.explosionRadius,
                Damage = authoring.damage
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