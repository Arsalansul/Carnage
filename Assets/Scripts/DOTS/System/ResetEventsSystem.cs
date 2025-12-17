using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
internal partial struct ResetEventsSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.onHealthChanged = false;
        }
        
        foreach (var inputData in SystemAPI.Query<RefRW<InputData>>())
        {
            inputData.ValueRW.MouseRight = false;
        }

        foreach (var meleeAttack in SystemAPI.Query<RefRW<MeleeAttack>>())
        {
            meleeAttack.ValueRW.animateAttack = false;
        }
        
        foreach (var gameState in SystemAPI.Query<RefRW<GameState>>())
        {
            gameState.ValueRW.Restart = false;
            gameState.ValueRW.ShouldInitialize = false;
        }

        foreach (var shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
        {
            shootAttack.ValueRW.onShoot.isTrigger = false;
        }
    }
}