using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class PlayerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Player());
            }
        }
    }

    public struct Player : IComponentData
    {
    }
}