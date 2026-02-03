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
            foreach (var (enemyShootAttack, shootAttack, enemy, localTransform, target) in 
                     SystemAPI.Query<RefRO<EnemyShootAttack>, RefRW<ShootAttack>, RefRO<Enemy>, RefRO<LocalTransform>, RefRO<Target>>())
            {
                if (target.ValueRO.targetEntity == Entity.Null) continue;
                
                var targetTransform = SystemAPI.GetComponentRO<LocalTransform>(target.ValueRO.targetEntity);
                var spawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnPosition);
                
                shootAttack.ValueRW.canShoot = math.length(targetTransform.ValueRO.Position - localTransform.ValueRO.Position) < enemyShootAttack.ValueRO.attackRange;
                shootAttack.ValueRW.shootDirection = targetTransform.ValueRO.Position - spawnWorldPosition;
            }
        }
    }
}