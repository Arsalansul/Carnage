using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class DamageOnTriggerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<DamageOnTriggerAuthoring>
        {
            public override void Bake(DamageOnTriggerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new DamageOnTrigger()
                {
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
}