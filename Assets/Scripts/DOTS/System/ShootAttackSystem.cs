using System;
using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.System
{
    public partial struct ShootAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = SystemAPI.GetSingleton<GameConfigComponent>();
            ref var bulletsArray = ref config.Bullets.Value;

            foreach (var (localTransform, shootAttack, unitMover, entity) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRW<ShootAttack>, RefRO<UnitMover>>().WithEntityAccess())
            {
                shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (shootAttack.ValueRO.timer > 0f) continue;
            
                if (!shootAttack.ValueRO.canShoot) continue;

                shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

                var bulletType = shootAttack.ValueRO.currentWeapon.bulletType;
                BulletSettings bulletConfig = default;
                for (int i = 0; i < bulletsArray.Array.Length; i++)
                {
                    if (bulletsArray.Array[i].type == bulletType)
                    {
                        bulletConfig = bulletsArray.Array[i];
                        break;
                    }
                }

                var bulletEntity = state.EntityManager.Instantiate(GetBulletEntity(bulletType, ref state));
                var spawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnPosition);
                SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(spawnWorldPosition));

                var bulletComponent = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
                bulletComponent.ValueRW.direction = shootAttack.ValueRO.shootDirection;
                bulletComponent.ValueRW.maxDistance = bulletConfig.maxDistance;
                bulletComponent.ValueRW.speed = bulletConfig.speed;
            
                var damageOnTrigger = SystemAPI.GetComponentRW<DamageOnTrigger>(bulletEntity);
                damageOnTrigger.ValueRW.triggered = false;
                damageOnTrigger.ValueRW.damageTargetFaction = shootAttack.ValueRO.targetFaction;
                damageOnTrigger.ValueRW.amount = bulletConfig.damageOnTrigger;

                if (bulletType == BulletsType.explosion)
                {
                    var sphereDamage = SystemAPI.GetComponentRW<SphereDamage>(bulletEntity);
                    sphereDamage.ValueRW.Damage = bulletConfig.explosionDamage;
                    sphereDamage.ValueRW.ExplosionRadius = bulletConfig.explosionRadius;
                }

                shootAttack.ValueRW.onShoot.isTrigger = true;
                shootAttack.ValueRW.onShoot.shootFromPosition = spawnWorldPosition;
            }
        }

        [BurstCompile]
        private Entity GetBulletEntity(BulletsType bulletType, ref SystemState state)
        {
            var entitiesReferencesEntity = SystemAPI.GetSingletonEntity<EntitiesReferences>();
        
            var map = state.EntityManager.GetBuffer<BulletsEntityMap>(entitiesReferencesEntity);
            for (int i = 0; i < map.Length; i++)
            {
                if (map[i].type == bulletType)
                {
                    return map[i].entity;
                }
            }

            throw new Exception("bullet not found in map");
        }
    }
}