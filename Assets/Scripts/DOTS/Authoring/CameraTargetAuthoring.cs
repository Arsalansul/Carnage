using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class CameraTargetAuthoring : MonoBehaviour
    {
        private class Baker : Baker<CameraTargetAuthoring>
        {
            public override void Bake(CameraTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraTarget());
            }
        }
    }

    public struct CameraTarget : IComponentData
    {
    }
}