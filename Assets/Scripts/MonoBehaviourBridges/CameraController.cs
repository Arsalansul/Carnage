using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void LateUpdate()
    {
        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<CameraFollow>()
            .Build(World.DefaultGameObjectInjectionWorld.EntityManager);
        if (entityQuery.IsEmpty) return;
        var entities = entityQuery.ToEntityArray(Allocator.Temp);
        var cameraFollowLocalTransform =
            World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entities[0]);
        transform.position = cameraFollowLocalTransform.Position;
    }
}