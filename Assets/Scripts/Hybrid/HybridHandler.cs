using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

public class HybridHandler
{
    private NewInputActions inputActions;

    public HybridHandler(NewInputActions inputActions)
    {
        this.inputActions = inputActions;
    }
    
    public void SetInputDataField(InputDataActionType inputAction, InputAction.CallbackContext context = default)
    {
        TryGetComponentAndEntityWithAll<InputData>(out var component, out var entity, out var entityManager);

        switch (inputAction)
        {
            case InputDataActionType.Move:
                component.Movement = inputActions.GameMap.Move.ReadValue<Vector2>();
                break;
            case InputDataActionType.MousePos:
                var inputMousePosition = context.ReadValue<Vector2>();
                component.MousePos =
                    Camera.main.ScreenToWorldPoint(new Vector3(inputMousePosition.x, inputMousePosition.y, 10));
                break;
            case InputDataActionType.MouseLeftButton:
                component.MouseLeft = true;
                break;
            case InputDataActionType.MouseRightButton:
                component.MouseRight = true;
                break;
            case InputDataActionType.MouseLeftButtonCancel:
                component.MouseLeft = false;
                break;
        }

        entityManager.SetComponentData(entity, component);
    }
    
    public void RestartEcsGame()
    {
        TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
    
        component.GameOver = false;
        component.Restart = true;
        component.Score = 0;
        component.ShouldInitialize = true;
    
        entityManager.SetComponentData(entity, component);
    }

    public void InitializeEcs()
    {
        TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
    
        component.ShouldInitialize = true;
        entityManager.SetComponentData(entity, component);
    }

    public bool IsGameOver()
    {
        TryGetComponentAndEntityWithAll<GameState>(out var component, out var entity, out var entityManager);
        
        return component.GameOver;
    }
    
    public bool IsScoreChanged(out int score)
    {
        score = 0;
        
        if (!TryGetComponentAndEntityWithAll<OnScoreChanged>(out var onScoreChangedEvent, out var entity, out var entityManager)) return false;
        
        score = onScoreChangedEvent.score;
        entityManager.SetComponentEnabled<OnScoreChanged>(entity, false);
        return true;
    }
    
    public bool IsWaveChanged(out int wave)
    {
        wave = 0;
        
        if (!TryGetComponentAndEntityWithAll<OnWaveChanged>(out var onWaveChangedEvent, out var entity, out var entityManager)) return false;
        
        wave = onWaveChangedEvent.wave;
        entityManager.SetComponentEnabled<OnWaveChanged>(entity, false);
        return true;
    }
    
    public bool IsEnemiesLeftChanged(out int enemiesLeftCount)
    {
        enemiesLeftCount = 0;
        
        if (!TryGetComponentAndEntityWithAll<OnEnemiesLeftCountChanged>(out var onEnemiesLeftCountChangedEvent, out var entity, out var entityManager)) return false;
        
        enemiesLeftCount = onEnemiesLeftCountChangedEvent.enemiesLeftCount;
        entityManager.SetComponentEnabled<OnEnemiesLeftCountChanged>(entity, false);
        return true;
    }

    public bool TryGetCameraPosition(out float3 position)
    {
        position = default;
        
        if (!TryGetComponentAndEntityWithAll<CameraFollow>(out var component, out var entity, out var entityManager)) return false;
        
        var cameraFollowLocalTransform = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(entity);
        position = cameraFollowLocalTransform.Position;
        return true;
    }
    
    private void GetComponentAndEntityWithAll<T>(out T component, out Entity entity, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        TryGetComponentAndEntityWithAll<T>(out component, out entity, out entityManager);
    }

    private bool TryGetComponentAndEntityWithAll<T>(out T component, out Entity entity,
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
    
    private void GetComponentAndEntityWithPresent<T>(out T component, out Entity entity, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        GetComponentsAndEntitiesWithPresent<T>(out var componentArray, out var entities, out entityManager);
        component = componentArray[0];
        entity = entities[0];
    }

    private void GetComponentsAndEntitiesWithPresent<T>(out NativeArray<T> components, out NativeArray<Entity> entities, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithPresent<T>()
            .Build(entityManager);
        components = entityQuery.ToComponentDataArray<T>(Allocator.Temp);
        entities = entityQuery.ToEntityArray(Allocator.Temp);
    }

    private void GetComponentsAndEntitiesWithAll<T>(out NativeArray<T> components, out NativeArray<Entity> entities, out EntityManager entityManager) where T : unmanaged, IComponentData
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<T>()
            .Build(entityManager);
        components = entityQuery.ToComponentDataArray<T>(Allocator.Temp);
        entities = entityQuery.ToEntityArray(Allocator.Temp);
    }
}