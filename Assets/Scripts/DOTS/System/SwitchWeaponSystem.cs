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
            var inputData = SystemAPI.GetSingletonRW<InputData>();
        
            if (!inputData.ValueRO.SwitchWeapon) return;
        
            var config = SystemAPI.GetSingleton<GameConfigComponent>();
            ref var weaponsReference = ref config.Weapons;
            ref var weaponsArray = ref weaponsReference.Value;

            var weaponBlob = weaponsArray.Array[0];
        
            for (int i = 0; i < weaponsArray.Array.Length; i++)
            {
                if (weaponsArray.Array[i].type == inputData.ValueRO.WeaponType)
                {
                    weaponBlob = weaponsArray.Array[i];
                    break;
                }
            }
        
            foreach (var (shootAttack, player) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Player>>())
            {
                shootAttack.ValueRW.currentWeapon = weaponBlob;
                shootAttack.ValueRW.timerMax = weaponBlob.TimeMax;
                inputData.ValueRW.SwitchWeapon = false;
            
                var eventsHandlerEntity = SystemAPI.GetSingletonEntity<EventsHandler>();
                var onSwitchWeapon = SystemAPI.GetComponentRW<OnSwitchWeaponAnim>(eventsHandlerEntity);
                onSwitchWeapon.ValueRW.weaponType = weaponBlob.type;
                SystemAPI.SetComponentEnabled<OnSwitchWeaponAnim>(eventsHandlerEntity, true);
            }
        }
    }
}