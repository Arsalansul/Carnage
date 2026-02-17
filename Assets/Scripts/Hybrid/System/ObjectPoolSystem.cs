using Hybrid.Authoring;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

//todo remove
namespace Hybrid.System
{
    public partial class ObjectPoolSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (localTransform, unitGameObjectPrefab, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>, UnitGameObjectPrefab>().WithDisabled<VisualInitialized>()
                         .WithEntityAccess())
            {
                var unitView = PoolManager.Instance.GetUnitFromPool(unitGameObjectPrefab.value.name,
                    localTransform.ValueRO.Position);
                var newAnimatorReference = new UnitGameObjectReference
                {
                    unitView = unitView
                };
                unitView.SetPoolName(unitGameObjectPrefab.value.name);
                entityCommandBuffer.SetComponentEnabled<VisualInitialized>(entity, true);
                entityCommandBuffer.AddComponent(entity, newAnimatorReference);
            }

            foreach (var (animatorReference, entity) in SystemAPI.Query<UnitGameObjectReference>()
                         .WithNone<UnitGameObjectPrefab, LocalTransform>().WithEntityAccess())
            {
                animatorReference.unitView.Dead(true);
                entityCommandBuffer.RemoveComponent<UnitGameObjectReference>(entity);
            }

            foreach (var (poolableGameObject, entity) in SystemAPI.Query<PoolableGameObject>()
                         .WithDisabled<VisualInitialized>().WithEntityAccess())
            {
                var poolName = poolableGameObject.poolName;
                var view = PoolManager.Instance.GetCleanObjectFromPool(poolName.ToString());
                entityCommandBuffer.AddComponent(entity, new GameObjectCleanup()
                {
                    transform = view.transform,
                    poolname = poolName
                });
                entityCommandBuffer.SetComponentEnabled<VisualInitialized>(entity, true);
            }

            foreach (var (cleanup, entity) in SystemAPI.Query<GameObjectCleanup>()
                         .WithNone<PoolableGameObject, LocalTransform>().WithEntityAccess())
            {
                PoolManager.Instance.ReturnCleanObjectToPool(cleanup.poolname.ToString(), cleanup.transform);
                entityCommandBuffer.RemoveComponent<GameObjectCleanup>(entity);
            }

            entityCommandBuffer.Playback(EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
}