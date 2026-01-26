using System;
using Ui.Controllers;
using UnityEngine;
using Zenject;

namespace Ui
{
    public class UiManager : MonoBehaviour
    {
        [Inject] private InventoryController inventoryController;
        [Inject] private InGameHudController inGameHudController;
        [Inject] private EndGameUiController endGameUiController;

        public delegate void SetInGameData(int value);
        public delegate void InvokeEndGameUiAction(EndGameAction action);

        public SetInGameData setInGameScore;
        public SetInGameData setInGameWave;
        public SetInGameData setInGameEnemiesLeft;
        public InvokeEndGameUiAction invokeEndGameUiAction;
        public event Action OnRestartButton;
        

        private void OnEnable()
        {
            setInGameScore += inGameHudController.SetScore;
            setInGameWave += inGameHudController.SetWave;
            setInGameEnemiesLeft += inGameHudController.SetEnemiesLeftCount;
            invokeEndGameUiAction += endGameUiController.InvokeAction;
            endGameUiController.OnRestartButton += OnRestartButtonInvoke;
        }

        private void OnDisable()
        {
            setInGameScore -= inGameHudController.SetScore;
            setInGameWave -= inGameHudController.SetWave;
            setInGameEnemiesLeft -= inGameHudController.SetEnemiesLeftCount;
            invokeEndGameUiAction -= endGameUiController.InvokeAction;
            endGameUiController.OnRestartButton -= OnRestartButtonInvoke;
        }

        private void OnRestartButtonInvoke()
        {
            OnRestartButton?.Invoke();
        }
    }
}