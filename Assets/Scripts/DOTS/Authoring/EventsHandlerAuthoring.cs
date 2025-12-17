using Unity.Entities;
using UnityEngine;

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