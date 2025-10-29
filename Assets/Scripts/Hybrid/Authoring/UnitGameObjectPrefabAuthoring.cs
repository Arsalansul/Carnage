using Unity.Entities;
using UnityEngine;

public class UnitGameObjectPrefabAuthoring : MonoBehaviour
{
    public GameObject UnitGameObjectPrefab;

    public class Baker : Baker<UnitGameObjectPrefabAuthoring>
    {
        public override void Bake(UnitGameObjectPrefabAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new UnitGameObjectPrefab
            {
                value = authoring.UnitGameObjectPrefab
            });
        }
    }
}

public class UnitGameObjectPrefab : IComponentData
{
    public GameObject value;
}

public class UnitGameObjectReference : ICleanupComponentData
{
    public IUnitView unitView;
}