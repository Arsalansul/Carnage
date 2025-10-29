using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Zenject.SpaceFighter
{
    public class GameRestartHandler : IInitializable, IDisposable, ITickable
    {
        private readonly Settings _settings;
        private readonly SignalBus _signalBus;
        private float _delayStartTime;

        private bool _isDelaying;

        public GameRestartHandler(
            Settings settings,
            SignalBus signalBus)
        {
            _signalBus = signalBus;
            _settings = settings;
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void Initialize()
        {
            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void Tick()
        {
            if (_isDelaying)
                if (Time.realtimeSinceStartup - _delayStartTime > _settings.RestartDelay)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void OnPlayerDied()
        {
            // Wait a bit before restarting the scene
            _delayStartTime = Time.realtimeSinceStartup;
            _isDelaying = true;
        }

        [Serializable]
        public class Settings
        {
            public float RestartDelay;
        }
    }
}