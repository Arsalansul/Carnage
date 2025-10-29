using System;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public delegate void SetTextDelegate(string text);

    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text waveText;
    [SerializeField] private Text enemiesLeftText;
    
    public SetTextDelegate OnScoreChangedDelegate;
    public SetTextDelegate OnWaveChangedDelegate;
    public SetTextDelegate OnEnemiesLeftChangedDelegate;

    private void OnEnable()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
        OnScoreChangedDelegate += OnScoreChanged;
        OnWaveChangedDelegate += OnWaveChanged;
        OnEnemiesLeftChangedDelegate += OnEnemiesLeftChanged;
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        OnScoreChangedDelegate -= OnScoreChanged;
        OnWaveChangedDelegate -= OnWaveChanged;
        OnEnemiesLeftChangedDelegate -= OnEnemiesLeftChanged;
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

    private void OnScoreChanged(string text)
    {
        scoreText.text = text;
    }

    private void OnWaveChanged(string text)
    {
        waveText.text = text;
    }

    private void OnEnemiesLeftChanged(string text)
    {
        enemiesLeftText.text = text;
    }
}