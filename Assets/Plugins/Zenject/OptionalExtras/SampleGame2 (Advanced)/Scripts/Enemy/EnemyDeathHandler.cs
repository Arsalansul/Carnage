using System;
using UnityEngine;

namespace Zenject.SpaceFighter
{
    public class EnemyDeathHandler
    {
        private readonly AudioPlayer _audioPlayer;
        private readonly Explosion.Factory _explosionFactory;
        private readonly EnemyFacade _facade;
        private readonly Settings _settings;
        private readonly SignalBus _signalBus;
        private readonly EnemyView _view;

        public EnemyDeathHandler(
            EnemyView view,
            AudioPlayer audioPlayer,
            Explosion.Factory explosionFactory,
            Settings settings,
            SignalBus signalBus,
            EnemyFacade facade)
        {
            _facade = facade;
            _signalBus = signalBus;
            _settings = settings;
            _explosionFactory = explosionFactory;
            _audioPlayer = audioPlayer;
            _view = view;
        }

        public void Die()
        {
            var explosion = _explosionFactory.Create();
            explosion.transform.position = _view.Position;

            _audioPlayer.Play(_settings.DeathSound, _settings.DeathSoundVolume);

            _signalBus.Fire<EnemyKilledSignal>();

            _facade.Dispose();
        }

        [Serializable]
        public class Settings
        {
            public AudioClip DeathSound;
            public float DeathSoundVolume = 1.0f;
        }
    }
}