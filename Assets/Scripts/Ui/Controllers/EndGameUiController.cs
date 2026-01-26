using System;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.Controllers
{
    public enum EndGameAction
    {
        show,
        hide,
    }
    
    public class EndGameUiController : MonoBehaviour
    {
        [SerializeField] private Transform gameOverPanel;
        [SerializeField] private Button restartButton;
        
        public event Action OnRestartButton;
        
        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveAllListeners();
        }

        public void InvokeAction(EndGameAction action)
        {
            switch (action)
            {
                case EndGameAction.show:
                    ShowGameOverPanel();
                    break;
                case EndGameAction.hide:
                    HideGameOverPanel();
                    break;
            }
        }

        private void ShowGameOverPanel()
        {
            gameOverPanel.gameObject.SetActive(true);
        }

        private void HideGameOverPanel()
        {
            gameOverPanel.gameObject.SetActive(false);
        }

        private void OnRestartButtonClick()
        {
            OnRestartButton?.Invoke();
        }
    }
}