using DOTS.Authoring;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS.System
{
    public partial struct PickupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = new OnPickupTriggerEventsJob()
            {
                playerLookup = SystemAPI.GetComponentLookup<Player>(true),
                pickupLookup = SystemAPI.GetComponentLookup<Pickup>()
            }.Schedule(simulation, state.Dependency);
            
            var pickupJob = new PickupEntitiesJob()
            {
                ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                eventsHandlerEntity = SystemAPI.GetSingletonEntity<EventsHandler>(),
                healthLookup = SystemAPI.GetComponentLookup<Health>(true),
                pickupWeaponLookup = SystemAPI.GetComponentLookup<PickupWeapon>(true),
                config = SystemAPI.GetSingleton<GameConfigComponent>()
            };
            pickupJob.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct OnPickupTriggerEventsJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<Player> playerLookup;
        public ComponentLookup<Pickup> pickupLookup;
    
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            if ((playerLookup.TryGetRefRO(entityA, out var player) &&  pickupLookup.TryGetRefRW(entityB, out var pickup) || 
                playerLookup.TryGetRefRO(entityB, out player) && pickupLookup.TryGetRefRW(entityA, out pickup)) &&
                !pickup.ValueRO.triggered)
            {
                pickup.ValueRW.triggered = true;
                pickup.ValueRW.activator = playerLookup.TryGetRefRO(entityA, out player) ? entityA : entityB;
            }
        }
    }

    [BurstCompile]
    public partial struct PickupEntitiesJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public Entity eventsHandlerEntity;
        [ReadOnly] public ComponentLookup<Health> healthLookup;
        [ReadOnly] public ComponentLookup<PickupWeapon> pickupWeaponLookup;
        [ReadOnly] public GameConfigComponent config;
        
        public void Execute(ref Pickup pickup, in LocalTransform localTransform, [ChunkIndexInQuery] int chunkIndex, Entity entity)
        {
            if (!pickup.triggered) return;
            
            switch (pickup.type)
            {
                case PickupType.heal:
                    var playerHealth = healthLookup.GetRefRO(pickup.activator);
                    Heal(playerHealth, pickup.activator, chunkIndex);
                    ecb.DestroyEntity(chunkIndex, entity);
                    break;
                case PickupType.bomb:
                    ActivateBomb(chunkIndex, entity, config.bombSettings);
                    break;
                case PickupType.weapon:
                    GiveWeapon(out var weaponType, entity);
                    ecb.SetComponent(chunkIndex, eventsHandlerEntity, new OnSwitchWeapon(){weaponType = weaponType});
                    ecb.SetComponentEnabled<OnSwitchWeapon>(chunkIndex, eventsHandlerEntity, true);
                    ecb.SetComponentEnabled<OnSwitchWeaponSystem>(chunkIndex, eventsHandlerEntity, false);
                    ecb.SetComponentEnabled<OnSwitchWeaponAnim>(chunkIndex, eventsHandlerEntity, false);
                    ecb.SetComponentEnabled<OnSwitchWeaponUi>(chunkIndex, eventsHandlerEntity, false);
                    ecb.DestroyEntity(chunkIndex, entity);
                    break;
            }
        }
        
        [BurstCompile]
        private void Heal(RefRO<Health> health, Entity playerEntity, [ChunkIndexInQuery] int chunkIndex)
        {
            var playerHealth = health.ValueRO;
            playerHealth.amount = health.ValueRO.max;
            playerHealth.onHealthChanged = true;
            ecb.SetComponent(chunkIndex, playerEntity, playerHealth);
        }

        [BurstCompile]
        private void ActivateBomb([ChunkIndexInQuery] int chunkIndex, Entity pickupEntity, BombConsumableSettings bombSettings)
        {
            ecb.SetComponent(chunkIndex, pickupEntity, new SphereDamage()
            {
                Damage = bombSettings.damage,
                ExplosionRadius = bombSettings.explosionRange
            });
            ecb.SetComponentEnabled<SphereDamage>(chunkIndex, pickupEntity, true);
        }

        [BurstCompile]
        private void GiveWeapon(out WeaponType weaponType, Entity entity)
        {
            pickupWeaponLookup.TryGetRefRO(entity, out var pickupWeapon);
            weaponType = pickupWeapon.ValueRO.weaponType;
        }
    }
}