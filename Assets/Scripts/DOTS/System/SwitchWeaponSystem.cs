using DOTS;
using Unity.Burst;
using Unity.Entities;

[UpdateBefore(typeof(PlayerSpawnerSystem))]
public partial struct SwitchWeaponSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputData = SystemAPI.GetSingletonRW<InputData>();
        
        if (!inputData.ValueRO.SwitchWeapon) return;
        
        var config = SystemAPI.GetSingleton<GameConfigComponent>();
        ref var weaponsReference = ref config.Weapons;
        ref var weaponsArray = ref weaponsReference.Value;
        
        var weaponIndex = inputData.ValueRO.WeaponIndex % weaponsArray.Array.Length;
        inputData.ValueRW.WeaponIndex = weaponIndex;
        
        foreach (var (shootAttack, player) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Player>>())
        {
            shootAttack.ValueRW.timerMax = weaponsArray.Array[inputData.ValueRO.WeaponIndex].TimeMax;
        }
    }
}