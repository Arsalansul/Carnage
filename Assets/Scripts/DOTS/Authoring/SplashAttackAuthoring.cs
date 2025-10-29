using Unity.Entities;
using UnityEngine;

public class SplashAttackAuthoring : MonoBehaviour
{
    public float timerMax;
    public int damage;
    public float radius;

    private class Baker : Baker<SplashAttackAuthoring>
    {
        public override void Bake(SplashAttackAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SplashAttack
            {
                timerMax = authoring.timerMax,
                damage = authoring.damage,
                raidus = authoring.radius
            });
        }
    }
}

public struct SplashAttack : IComponentData
{
    public float timer;
    public float timerMax;
    public int damage;
    public float raidus;
}