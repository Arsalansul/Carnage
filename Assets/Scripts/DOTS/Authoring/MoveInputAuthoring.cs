using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class MoveInputAuthoring : MonoBehaviour
    {
        private class Baker : Baker<MoveInputAuthoring>
        {
            public override void Bake(MoveInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MoveInput());
            }
        }
    }

    public struct MoveInput : IComponentData
    {
    }
}