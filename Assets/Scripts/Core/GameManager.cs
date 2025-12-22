using Core;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private UiManager uiManager;
    [Inject] private HybridHandler hybridHandler;

    private void Start()
    {
        uiManager.HideGameOverPanel();
        hybridHandler.InitializeEcs();
    }

    private void Update()
    {
        if (hybridHandler.IsGameOver())
        {
            uiManager.ShowGameOverPanel();
            return;
        }
        
        if (hybridHandler.IsScoreChanged(out var score)) uiManager.SetScore(score);
        if (hybridHandler.IsWaveChanged(out var wave)) uiManager.SetWave(wave);
        if (hybridHandler.IsEnemiesLeftChanged(out var enemiesLeftCount)) uiManager.SetEnemiesLeftCount(enemiesLeftCount);
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
        hybridHandler.RestartEcsGame();
        uiManager.HideGameOverPanel();
    }
}