using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    [RequireComponent(typeof(ShootAttackAuthoring))]
    public class EnemyShootAttackAuthoring : MonoBehaviour
    {
        public WeaponType weaponType;
        public float attackRange;
        private class Baker : Baker<EnemyShootAttackAuthoring>
        {
            public override void Bake(EnemyShootAttackAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemyShootAttack()
                {
                    weaponType = authoring.weaponType,
                    attackRange = authoring.attackRange
                });
            }
        }
    }
    
    public struct EnemyShootAttack : IComponentData
    {
        public WeaponType weaponType;
        public float attackRange;
    }
}