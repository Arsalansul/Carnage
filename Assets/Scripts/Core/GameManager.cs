using Core;
using Ui;
using Ui.Controllers;
using Ui.Models;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private UiManager uiManager;
    [Inject] private HybridHandler hybridHandler;
    [Inject] private IInventory inventory;

    private void Start()
    {
        uiManager.invokeEndGameUiAction(EndGameAction.hide);
        
        inventory.AddItem(ItemType.SimpleGun); //todo
        inventory.AddItem(ItemType.RocketGun);
        
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