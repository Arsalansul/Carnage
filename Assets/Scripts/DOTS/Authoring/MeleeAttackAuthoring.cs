using Unity.Entities;
using UnityEngine;

public class MeleeAttackAuthoring : MonoBehaviour
{
    public float timerMax;
    public int damage;
    public float colliderSize;
    public float attackDistance;

    public class Baker : Baker<MeleeAttackAuthoring>
    {
        public override void Bake(MeleeAttackAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MeleeAttack
            {
                timerMax = authoring.timerMax,
                damage = authoring.damage,
                colliderSize = authoring.colliderSize,
                attackDistance = authoring.attackDistance
            });
        }
    }
}

public struct MeleeAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public int damage;
    public float colliderSize;
    public float attackDistance;
    public bool animateAttack;
}