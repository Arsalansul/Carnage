using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class ObjectPoolSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (localTransform, unitGameObjectPrefab, entity) in
                 SystemAPI.Query<RefRO<LocalTransform>, UnitGameObjectPrefab>()
                     .WithDisabled<VisualInitialized>().WithEntityAccess())
        {
            var unitView = PoolManager.Instance.GetUnitFromPool(unitGameObjectPrefab.value.name, localTransform.ValueRO.Position);
            var newAnimatorReference = new UnitGameObjectReference
            {
                unitView = unitView
            };
            unitView.SetPoolName(unitGameObjectPrefab.value.name);
            entityCommandBuffer.SetComponentEnabled<VisualInitialized>(entity, true);
            entityCommandBuffer.AddComponent(entity, newAnimatorReference);
        }

        foreach (var (animatorReference, entity) in
                 SystemAPI.Query<UnitGameObjectReference>()
                     .WithNone<UnitGameObjectPrefab, LocalTransform>()
                     .WithEntityAccess())
        {
            animatorReference.unitView.Dead(true);
            entityCommandBuffer.RemoveComponent<UnitGameObjectReference>(entity);
        }

        foreach (var (bullet, bulletGameObject, entity) in 
                 SystemAPI.Query<RefRO<Bullet>, BulletGameObject>()
                     .WithDisabled<VisualInitialized>()
                     .WithEntityAccess())
        {
            var poolName = bulletGameObject.gameObject.name;
            var bulletView = PoolManager.Instance.GetBulletFromPool(poolName);
            entityCommandBuffer.AddComponent(entity, new BulletCleanup()
            {
                transform = bulletView.transform,
                poolname = poolName
            });
            entityCommandBuffer.SetComponentEnabled<VisualInitialized>(entity,true);
        }

        foreach (var (bulletCleanup, entity) in
                 SystemAPI.Query<BulletCleanup>()
                     .WithNone<BulletGameObject, LocalTransform>()
                     .WithEntityAccess())
        {
            PoolManager.Instance.ReturnBulletToPool(bulletCleanup.poolname, bulletCleanup.transform);
            entityCommandBuffer.RemoveComponent<BulletCleanup>(entity);
        }

        entityCommandBuffer.Playback(EntityManager);
        entityCommandBuffer.Dispose();
    }
}