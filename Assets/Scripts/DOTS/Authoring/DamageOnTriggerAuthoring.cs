using Unity.Entities;
using UnityEngine;

public class DamageOnTriggerAuthoring : MonoBehaviour
{
    public int damage;
    private class Baker : Baker<DamageOnTriggerAuthoring>
    {
        public override void Bake(DamageOnTriggerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DamageOnTrigger()
            {
                amount = authoring.damage,
            });
        }
    }
}

public struct DamageOnTrigger : IComponentData
{
    public int amount;
    public Faction damageTargetFaction;
    public bool triggered;
}