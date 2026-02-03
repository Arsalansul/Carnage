using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class PlayerSpawnerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerSpawnerAuthoring>
        {
            public override void Bake(PlayerSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayerSpawner());
            }
        }
    }

    public struct PlayerSpawner : IComponentData
    {
        public bool shouldSpawn;
    }
}