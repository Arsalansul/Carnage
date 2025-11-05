using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

public partial struct PlayerSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, playerSpawner) in
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<PlayerSpawner>>())
        {
            if (!playerSpawner.ValueRO.shouldSpawn) continue;

            var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
            var cameraFollowEntity = SystemAPI.GetSingletonEntity<CameraFollow>();
            var cameraFollowLocalTransform = SystemAPI.GetComponentRO<LocalTransform>(cameraFollowEntity);
            var cameraFollow = SystemAPI.GetComponentRO<CameraFollow>(cameraFollowEntity);

            var playerEntity = state.EntityManager.Instantiate(entitiesReferences.playerPrefab);
            playerSpawner.ValueRW.shouldSpawn = false;

            if (SystemAPI.HasComponent<ShootAttack>(playerEntity))
            {
                var inputData = SystemAPI.GetSingleton<InputData>();
                var config = SystemAPI.GetSingleton<GameConfigComponent>();
                ref var weaponsReference = ref config.Weapons;
                ref var weaponsArray = ref weaponsReference.Value;
                var shootAttack = SystemAPI.GetComponentRW<ShootAttack>(playerEntity);
                shootAttack.ValueRW.timerMax = weaponsArray.Array[inputData.WeaponIndex].TimeMax;
            }
            SystemAPI.SetComponent(playerEntity, LocalTransform.FromPosition(cameraFollowLocalTransform.ValueRO.Position - cameraFollow.ValueRO.offset));
        }
    }
}