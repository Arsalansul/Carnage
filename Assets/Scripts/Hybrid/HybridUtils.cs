using Unity.Collections;
using Unity.Entities;

namespace Hybrid
{
    public class HybridUtils
    {
        public void GetComponentAndEntityWithAll<T>(out T component, out Entity entity, out EntityManager entityManager) where T : unmanaged, IComponentData
        {
            TryGetComponentAndEntityWithAll<T>(out component, out entity, out entityManager);
        }

        public bool TryGetComponentAndEntityWithAll<T>(out T component, out Entity entity,
            out EntityManager entityManager) where T : unmanaged, IComponentData
        {
            component = default;
            entity = default;
            GetComponentsAndEntitiesWithAll<T>(out var components, out var entities, out entityManager);

            if (components.Length == 0) return false;
            component = components[0];
            entity = entities[0];
            return true;
        }

        public void GetComponentAndEntityWithPresent<T>(out T component, out Entity entity,
            out EntityManager entityManager) where T : unmanaged, IComponentData
        {
            GetComponentsAndEntitiesWithPresent<T>(out var componentArray, out var entities, out entityManager);
            component = componentArray[0];
            entity = entities[0];
        }

        public void GetComponentsAndEntitiesWithPresent<T>(out NativeArray<T> components, out NativeArray<Entity> entities, out EntityManager entityManager) where T : unmanaged, IComponentData
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithPresent<T>()
                .Build(entityManager);
            components = entityQuery.ToComponentDataArray<T>(Allocator.Temp);
            entities = entityQuery.ToEntityArray(Allocator.Temp);
        }

        public void GetComponentsAndEntitiesWithAll<T>(out NativeArray<T> components, out NativeArray<Entity> entities,
            out EntityManager entityManager) where T : unmanaged, IComponentData
        {
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<T>()
                .Build(entityManager);
            components = entityQuery.ToComponentDataArray<T>(Allocator.Temp);
            entities = entityQuery.ToEntityArray(Allocator.Temp);
        }
    }
}