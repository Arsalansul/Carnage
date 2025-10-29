using System;
using UnityEngine;

namespace Zenject.SpaceFighter
{
    public class PlayerShootHandler : ITickable
    {
        private readonly AudioPlayer _audioPlayer;
        private readonly Bullet.Factory _bulletFactory;
        private readonly PlayerInputState _inputState;
        private readonly Player _player;
        private readonly Settings _settings;

        private float _lastFireTime;

        public PlayerShootHandler(
            PlayerInputState inputState,
            Bullet.Factory bulletFactory,
            Settings settings,
            Player player,
            AudioPlayer audioPlayer)
        {
            _audioPlayer = audioPlayer;
            _player = player;
            _settings = settings;
            _bulletFactory = bulletFactory;
            _inputState = inputState;
        }

        public void Tick()
        {
            if (_player.IsDead) return;

            if (_inputState.IsFiring && Time.realtimeSinceStartup - _lastFireTime > _settings.MaxShootInterval)
            {
                _lastFireTime = Time.realtimeSinceStartup;
                Fire();
            }
        }

        private void Fire()
        {
            _audioPlayer.Play(_settings.Laser, _settings.LaserVolume);

            var bullet = _bulletFactory.Create(
                _settings.BulletSpeed, _settings.BulletLifetime, BulletTypes.FromPlayer);

            bullet.transform.position = _player.Position + _player.LookDir * _settings.BulletOffsetDistance;
            bullet.transform.rotation = _player.Rotation;
        }

        [Serializable]
        public class Settings
        {
            public AudioClip Laser;
            public float LaserVolume = 1.0f;

            public float BulletLifetime;
            public float BulletSpeed;
            public float MaxShootInterval;
            public float BulletOffsetDistance;
        }
    }
}