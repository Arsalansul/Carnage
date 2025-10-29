using Unity.Entities;
using UnityEngine;

public class BulletGameObjectAuthoring : MonoBehaviour
{
    public GameObject BulletPrefab;
    private class BulletGameObjectAuthoringBaker : Baker<BulletGameObjectAuthoring>
    {
        public override void Bake(BulletGameObjectAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponentObject(entity, new BulletGameObject()
            {
                gameObject = authoring.BulletPrefab
            });
        }
    }
}

public class BulletGameObject : IComponentData
{
    public GameObject gameObject;
}

public class BulletCleanup : ICleanupComponentData
{
    public string poolname;
    public Transform transform;
}