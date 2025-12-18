using System;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text enemiesLeftText;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
    }

    public event Action OnRestartButton;

    public void ShowGameOverPanel()
    {
        gameOverPanel.gameObject.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.gameObject.SetActive(false);
    }

    private void OnRestartButtonClick()
    {
        OnRestartButton?.Invoke();
    }

    public void SetScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void SetWave(int wave)
    {
        waveText.text = $"Wave {wave + 1}";
    }

    public void SetEnemiesLeftCount(int count)
    {
        enemiesLeftText.text = $"Left: {count}";
    }
}