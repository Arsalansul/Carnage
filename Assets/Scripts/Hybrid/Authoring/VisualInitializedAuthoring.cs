using Unity.Entities;
using UnityEngine;

public class VisualInitializedAuthoring : MonoBehaviour
{
    private class Baker : Baker<VisualInitializedAuthoring>
    {
        public override void Bake(VisualInitializedAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new VisualInitialized());
            SetComponentEnabled<VisualInitialized>(false);
        }
    }
}

public struct VisualInitialized : IComponentData, IEnableableComponent
{
}