using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class ShootAttackAuthoring : MonoBehaviour
{
    public float timerMax;
    public Transform bulletSpawnTransform;

    public class Baker : Baker<ShootAttackAuthoring>
    {
        public override void Bake(ShootAttackAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ShootAttack
            {
                timerMax = authoring.timerMax,
                bulletSpawnPosition = authoring.bulletSpawnTransform.localPosition
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

    public struct OnShootEvent
    {
        public bool isTrigger;
        public float3 shootFromPosition;
    }
}