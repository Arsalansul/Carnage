using DOTS;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var playerSpawner in SystemAPI.Query<RefRW<PlayerSpawner>>())
        {
            if (!playerSpawner.ValueRO.shouldSpawn) continue;

            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            var cameraFollowEntity = SystemAPI.GetSingletonEntity<CameraFollow>();
            var config = SystemAPI.GetSingleton<GameConfigComponent>();
            var cameraFollowLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(cameraFollowEntity);
            var cameraFollow = SystemAPI.GetComponentRO<CameraFollow>(cameraFollowEntity);

            var playerEntity = state.EntityManager.Instantiate(entitiesReferences.playerPrefab);
            playerSpawner.ValueRW.shouldSpawn = false;
            
            var unitMover = SystemAPI.GetComponentRW<UnitMover>(playerEntity);
            unitMover.ValueRW.moveSpeed = config.playerSettings.moveSpeed;
            unitMover.ValueRW.rotationSpeed = config.playerSettings.rotationSpeed;
            
            SystemAPI.SetComponent(playerEntity, LocalTransform.FromPosition(cameraFollowLocalTransform.ValueRO.Position - cameraFollow.ValueRO.offset));
        }
    }
}