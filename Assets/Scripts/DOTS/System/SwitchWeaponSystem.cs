using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateBefore(typeof(PlayerSpawnerSystem))]
public partial struct SwitchWeaponSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var inputData = SystemAPI.GetSingletonRW<InputData>();
        
        if (!inputData.ValueRO.MouseRight) return;
        
        var config = SystemAPI.GetSingleton<GameConfigComponent>();
        ref var weaponsReference = ref config.Weapons;
        ref var weaponsArray = ref weaponsReference.Value;

        var weaponIndex = (inputData.ValueRO.WeaponIndex + 1) % weaponsArray.Array.Length;

        inputData.ValueRW.WeaponIndex = weaponIndex;
        foreach (var (shootAttack, player) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Player>>())
        {
            shootAttack.ValueRW.timerMax = weaponsArray.Array[inputData.ValueRO.WeaponIndex].TimeMax;
        }
    }
}