using Unity.Entities;
using UnityEngine;

namespace Hybrid.Authoring
{
    public class VisualInitializedAuthoring : MonoBehaviour
    {
        private class Baker : Baker<VisualInitializedAuthoring>
        {
            public override void Bake(VisualInitializedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new VisualInitialized());
                SetComponentEnabled<VisualInitialized>(entity, false);
            }
        }
    }

    public struct VisualInitialized : IComponentData, IEnableableComponent
    {
    }
}