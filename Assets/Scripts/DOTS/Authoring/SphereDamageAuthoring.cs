using Unity.Entities;
using UnityEngine;

public class SphereDamageAuthoring : MonoBehaviour
{
    public float explosionRadius;
    public float triggerRadius;
    public int damage;

    private class Baker : Baker<SphereDamageAuthoring>
    {
        public override void Bake(SphereDamageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SphereDamage
            {
                ExplosionRadius = authoring.explosionRadius,
                TriggerRadius = authoring.triggerRadius,
                Damage = authoring.damage
            });
        }
    }
}

public struct SphereDamage : IComponentData
{
    public int Damage;
    public float ExplosionRadius;
    public float TriggerRadius;
    public bool onHit;
}