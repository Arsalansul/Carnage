using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authoring
{
    public class EventsHandlerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<EventsHandlerAuthoring>
        {
            public override void Bake(EventsHandlerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EventsHandler());
                AddComponent<OnScoreChanged>(entity);
                SetComponentEnabled<OnScoreChanged>(entity, true);
                AddComponent<OnWaveChanged>(entity);
                SetComponentEnabled<OnWaveChanged>(entity, true);
                AddComponent<OnEnemiesLeftCountChanged>(entity);
                SetComponentEnabled<OnEnemiesLeftCountChanged>(entity, true);
                AddComponent<OnSwitchWeapon>(entity);
                SetComponentEnabled<OnSwitchWeapon>(entity, false);
                AddComponent<OnSwitchWeaponSystem>(entity);
                SetComponentEnabled<OnSwitchWeaponSystem>(entity, false);
                AddComponent<OnSwitchWeaponAnim>(entity);
                SetComponentEnabled<OnSwitchWeaponAnim>(entity, false);
                AddComponent<OnSwitchWeaponUi>(entity);
                SetComponentEnabled<OnSwitchWeaponUi>(entity, false);
                AddComponent<TryDropItem>(entity);
                SetComponentEnabled<TryDropItem>(entity, false);
            }
        }
    }

    public struct EventsHandler : IComponentData{}

    public struct OnScoreChanged : IComponentData, IEnableableComponent
    {
        public int score;
    }

    public struct OnWaveChanged : IComponentData, IEnableableComponent
    {
        public int wave;
    }

    public struct OnEnemiesLeftCountChanged : IComponentData, IEnableableComponent
    {
        public int enemiesLeftCount;
    }

    public struct OnSwitchWeapon : IComponentData, IEnableableComponent
    {
        public WeaponType weaponType;
    }

    public struct OnSwitchWeaponSystem : IComponentData, IEnableableComponent{}
    public struct OnSwitchWeaponAnim : IComponentData, IEnableableComponent{}
    public struct OnSwitchWeaponUi : IComponentData, IEnableableComponent{}

    public struct TryDropItem : IComponentData, IEnableableComponent
    {
        public float3 position;
    }
}