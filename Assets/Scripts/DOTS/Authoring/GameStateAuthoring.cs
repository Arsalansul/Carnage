using Unity.Entities;
using UnityEngine;

public class GameStateAuthoring : MonoBehaviour
{
    private class Baker : Baker<GameStateAuthoring>
    {
        public override void Bake(GameStateAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GameState());
        }
    }
}

public struct GameState : IComponentData
{
    public bool GameOver;
    public bool Restart;
    public int Score;
    public bool ShouldInitialize;
    public int Wave;
    public bool OnWaveChanged;
    public int SpawnedEnemiesCount;
    public int EnemiesLeftCount;
}