using DOTS.Authoring;
using Unity.Burst;
using Unity.Entities;

namespace DOTS.System
{
    [UpdateBefore(typeof(PlayerSpawnerSystem))]
    public partial struct SwitchWeaponSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (onSwitchWeapon, entity) in SystemAPI.Query<RefRO<OnSwitchWeapon>>().WithEntityAccess())
            {
                if (SystemAPI.IsComponentEnabled<OnSwitchWeaponSystem>(entity)) continue;
                
                var config = SystemAPI.GetSingleton<GameConfigComponent>();
                ref var weaponsReference = ref config.Weapons;
                ref var weaponsArray = ref weaponsReference.Value;

                var weaponBlob = weaponsArray.Array[0];

                for (int i = 0; i < weaponsArray.Array.Length; i++)
                {
                    if (weaponsArray.Array[i].type == onSwitchWeapon.ValueRO.weaponType)
                    {
                        weaponBlob = weaponsArray.Array[i];
                        break;
                    }
                }

                foreach (var (shootAttack, player) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Player>>())
                {
                    shootAttack.ValueRW.currentWeapon = weaponBlob;
                    shootAttack.ValueRW.timerMax = weaponBlob.TimeMax;

                    SystemAPI.SetComponentEnabled<OnSwitchWeaponSystem>(entity, true);
                }
            }
        }
    }
}