using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
    public bool MouseLeft;
    public bool MouseRight;
    public float3 MousePos;
    public int WeaponIndex;
}