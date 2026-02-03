using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.System
{
    [UpdateBefore(typeof(ShootAttackSystem))]
    public partial struct ShootAttackEnemySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (enemyShootAttack, shootAttack, enemy, localTransform, target, unitMover) in 
                     SystemAPI.Query<RefRO<EnemyShootAttack>, RefRW<ShootAttack>, RefRO<Enemy>, RefRO<LocalTransform>, RefRO<Target>, RefRW<UnitMover> >())
            {
                if (target.ValueRO.targetEntity == Entity.Null) continue;
                
                var targetTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity);
                var spawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnPosition);
                
                shootAttack.ValueRW.canShoot = math.length(targetTransform.ValueRO.Position - localTransform.ValueRO.Position) < enemyShootAttack.ValueRO.attackRange;
                
                if (!shootAttack.ValueRO.canShoot) continue;
                
                shootAttack.ValueRW.shootDirection = targetTransform.ValueRO.Position - spawnWorldPosition;
                shootAttack.ValueRW.shootDirection.y = 0;
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }
        }
    }
}