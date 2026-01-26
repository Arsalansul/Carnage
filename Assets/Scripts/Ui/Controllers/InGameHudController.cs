using UnityEngine;
using UnityEngine.UI;

namespace Ui.Controllers
{
    public class InGameHudController : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text waveText;
        [SerializeField] private Text enemiesLeftText;

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
}