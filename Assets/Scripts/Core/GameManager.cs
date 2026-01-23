using Ui;
using Ui.Controller;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private UiManager uiManager;
    [Inject] private HybridHandler hybridHandler;

    private void Start()
    {
        uiManager.invokeEndGameUiAction(EndGameAction.hide);
        hybridHandler.SetGameConfig();
        hybridHandler.InitializeEcs();
    }

    private void Update()
    {
        if (hybridHandler.IsGameOver())
        {
            uiManager.invokeEndGameUiAction(EndGameAction.show);
            return;
        }
        
        if (hybridHandler.IsScoreChanged(out var score)) uiManager.setInGameScore(score);
        if (hybridHandler.IsWaveChanged(out var wave))
        {
            uiManager.setInGameWave(wave);
            hybridHandler.SetWaveSettings(wave);
        }
        if (hybridHandler.IsEnemiesLeftChanged(out var enemiesLeftCount)) uiManager.setInGameEnemiesLeft(enemiesLeftCount);
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
        uiManager.invokeEndGameUiAction(EndGameAction.hide);
    }
}