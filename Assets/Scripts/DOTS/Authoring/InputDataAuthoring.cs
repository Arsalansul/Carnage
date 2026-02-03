using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authoring
{
    public class InputDataAuthoring : MonoBehaviour
    {
        private class Baker : Baker<InputDataAuthoring>
        {
            public override void Bake(InputDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new InputData());
            }
        }
    }

    public struct InputData : IComponentData
    {
        public float2 Movement;
        public bool Fire;
        public bool SwitchWeapon;
        public float3 MousePos;
        public WeaponType WeaponType;
    }
}