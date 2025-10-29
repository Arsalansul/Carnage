using Unity.Burst;
using Unity.Entities;

public partial struct SwitchWeaponSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputData = SystemAPI.GetSingletonRW<InputData>();
        var gameState = SystemAPI.GetSingleton<GameState>();
        var config = SystemAPI.GetSingleton<GameConfigComponent>();
        ref var weaponsReference = ref config.Weapons;
        ref var weaponsArray = ref weaponsReference.Value;

        var weaponIndex = (inputData.ValueRO.WeaponIndex + 1) % 2;

        if (inputData.ValueRO.MouseRight)
        {
            inputData.ValueRW.WeaponIndex = weaponIndex;
        }

        if (inputData.ValueRO.WeaponIndex != weaponIndex || gameState.ShouldInitialize)
        {
            foreach (var (shootAttack, player) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Player>>())
            {
                shootAttack.ValueRW.timerMax = weaponsArray.Array[inputData.ValueRO.WeaponIndex].TimeMax;
            }
        }
    }
}