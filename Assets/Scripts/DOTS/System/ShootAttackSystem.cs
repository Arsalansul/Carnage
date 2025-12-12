using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

internal partial struct ShootAttackSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputData = SystemAPI.GetSingleton<InputData>();
        var config = SystemAPI.GetSingleton<GameConfigComponent>();
        ref var weaponsReference = ref config.Weapons;
        ref var weaponsArray = ref weaponsReference.Value;

        foreach (var (localTransform, shootAttack) in
                 SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShootAttack>>())
        {
            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

            if (shootAttack.ValueRO.timer > 0f) continue;

            if (!inputData.MouseLeft) continue;

            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            var bulletIndex = weaponsArray.Array[inputData.WeaponIndex].BulletIndex;

            var bulletEntity = state.EntityManager.Instantiate(GetBulletEntity(bulletIndex));
            var spawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(spawnWorldPosition));

            var bulletComponent = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletComponent.ValueRW.direction = inputData.MousePos - spawnWorldPosition;
            
            var damageOnTrigger = SystemAPI.GetComponentRW<DamageOnTrigger>(bulletEntity);
            damageOnTrigger.ValueRW.triggered = false;
            damageOnTrigger.ValueRW.damageTargetFaction = Faction.Enemy;

            shootAttack.ValueRW.onShoot.isTrigger = true;
            shootAttack.ValueRW.onShoot.shootFromPosition = spawnWorldPosition;
        }
    }

    private Entity GetBulletEntity(int bulletIndex)
    {
        var entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        return bulletIndex == 0 ? entitiesReferences.bulletPrefabEntity_0 : entitiesReferences.bulletPrefabEntity_1;
    }
}