using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authoring
{
    public class NavAgentAuthoring : MonoBehaviour
    {
        private class NavAgentAuthoringBaker : Baker<NavAgentAuthoring>
        {
            public override void Bake(NavAgentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new NavAgentComponent()
                {
                });
                AddBuffer<WaypointBuffer>(entity);
            }
        }
    }

    public struct NavAgentComponent : IComponentData
    {
        public float3 targetPosition;
        public bool pathCalculated;
        public int currentWaypoint;
    }

    public struct WaypointBuffer : IBufferElementData
    {
        public float3 wayPoint;
    }
}