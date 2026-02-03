using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authoring
{
    public class ShootAttackAuthoring : MonoBehaviour
    {
        public Transform bulletSpawnTransform;
        public Faction targetFaction;

        public class Baker : Baker<ShootAttackAuthoring>
        {
            public override void Bake(ShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ShootAttack
                {
                    bulletSpawnPosition = authoring.bulletSpawnTransform.localPosition,
                    targetFaction = authoring.targetFaction,
                });
            }
        }
    }

    public struct ShootAttack : IComponentData
    {
        public float timer;
        public float timerMax;
        public float3 bulletSpawnPosition;
        public OnShootEvent onShoot;
        public WeaponBlob currentWeapon;
        public Faction targetFaction;
        public bool canShoot;
        public float3 shootDirection;

        public struct OnShootEvent
        {
            public bool isTrigger;
            public float3 shootFromPosition;
        }
    }
}