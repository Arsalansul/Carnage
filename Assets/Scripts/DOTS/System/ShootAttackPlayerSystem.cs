using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace DOTS.System
{
    [UpdateBefore(typeof(ShootAttackSystem))]
    public partial struct ShootAttackPlayerSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var inputData = SystemAPI.GetSingleton<InputData>();
            
            foreach (var (localTransform, shootAttack, player) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShootAttack>, RefRO<Player>>())
            {
                var spawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnPosition);
                shootAttack.ValueRW.canShoot = inputData.Fire;
                shootAttack.ValueRW.shootDirection = inputData.MousePos - spawnWorldPosition;
                shootAttack.ValueRW.shootDirection.y = 0;
            }
        }
    }
}