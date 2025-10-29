using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private UiManager uiManager;
    
    private int score;
    private int wave;
    private int enemiesLeft;
    private string WaveString => $"Wave {wave + 1}";
    private string ScoreString => $"Score: {score}";
    private string EnemiesLeftString => $"Left: {enemiesLeft}";

    private void Start()
    {
        uiManager.HideGameOverPanel();
        InitializeEcs();
        uiManager.OnScoreChangedDelegate?.Invoke(ScoreString);
        uiManager.OnWaveChangedDelegate?.Invoke(WaveString);
        uiManager.OnEnemiesLeftChangedDelegate?.Invoke(EnemiesLeftString);
    }

    private void Update()
    {
        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GameState>()
            .Build(World.DefaultGameObjectInjectionWorld.EntityManager);
        var gameState = entityQuery.ToComponentDataArray<GameState>(Allocator.Temp)[0];
        if (gameState.GameOver)
        {
            uiManager.ShowGameOverPanel();
            return;
        }

        if (gameState.Score != score)
        {
            score = gameState.Score;
            uiManager.OnScoreChangedDelegate?.Invoke(ScoreString);
        }

        if (gameState.Wave != wave)
        {
            wave = gameState.Wave;
            uiManager.OnWaveChangedDelegate?.Invoke(WaveString);
        }

        if (gameState.EnemiesLeftCount != enemiesLeft)
        {
            enemiesLeft = gameState.EnemiesLeftCount;
            uiManager.OnEnemiesLeftChangedDelegate?.Invoke(EnemiesLeftString);
        }
    }

    private void OnEnable()
    {
        uiManager.OnRestartButton += RestartGame;
    }

    private void OnDisable()
    {
        uiManager.OnRestartButton -= RestartGame;
    }

    private void RestartGame()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GameState>()
            .Build(World.DefaultGameObjectInjectionWorld.EntityManager);
        var gameState = entityQuery.ToComponentDataArray<GameState>(Allocator.Temp)[0];
        var entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        gameState.GameOver = false;
        gameState.Restart = true;
        gameState.Score = 0;
        gameState.ShouldInitialize = true;

        entityManager.SetComponentData(entityArray[0], gameState);

        uiManager.HideGameOverPanel();
    }

    private void InitializeEcs()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<GameState>()
            .Build(World.DefaultGameObjectInjectionWorld.EntityManager);
        var gameState = entityQuery.ToComponentDataArray<GameState>(Allocator.Temp)[0];
        var entityArray = entityQuery.ToEntityArray(Allocator.Temp);

        gameState.ShouldInitialize = true;
        entityManager.SetComponentData(entityArray[0], gameState);
    }
}