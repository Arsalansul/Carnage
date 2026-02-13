using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class PickupAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PickupAuthoring>
        {
            public override void Bake(PickupAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Pickup()
                {
                    triggered = false
                });
            }
        }
    }

    public struct Pickup : IComponentData
    {
        public bool triggered;
        public PickupType type;
        public Entity activator;
    }
}