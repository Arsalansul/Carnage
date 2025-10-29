using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Zenject.Asteroids
{
    public class AsteroidManager : ITickable, IFixedTickable
    {
        private readonly Asteroid.Factory _asteroidFactory;
        private readonly List<Asteroid> _asteroids = new();
        private readonly Queue<AsteroidAttributes> _cachedAttributes = new();
        private readonly LevelHelper _level;
        private readonly Settings _settings;

        [InjectOptional] private bool _autoSpawn = true;

        private bool _started;
        private readonly float _timeIntervalBetweenSpawns;

        private float _timeToNextSpawn;

        public AsteroidManager(
            Settings settings, Asteroid.Factory asteroidFactory, LevelHelper level)
        {
            _settings = settings;
            _timeIntervalBetweenSpawns = _settings.maxSpawnTime / (_settings.maxSpawns - _settings.startingSpawns);
            _timeToNextSpawn = _timeIntervalBetweenSpawns;
            _asteroidFactory = asteroidFactory;
            _level = level;
        }

        public IEnumerable<Asteroid> Asteroids => _asteroids;

        public void FixedTick()
        {
            for (var i = 0; i < _asteroids.Count; i++) _asteroids[i].FixedTick();
        }

        public void Tick()
        {
            for (var i = 0; i < _asteroids.Count; i++) _asteroids[i].Tick();

            if (_started && _autoSpawn)
            {
                _timeToNextSpawn -= Time.deltaTime;

                if (_timeToNextSpawn < 0 && _asteroids.Count < _settings.maxSpawns)
                {
                    _timeToNextSpawn = _timeIntervalBetweenSpawns;
                    SpawnNext();
                }
            }
        }

        public void Start()
        {
            Assert.That(!_started);
            _started = true;

            ResetAll();
            GenerateRandomAttributes();

            for (var i = 0; i < _settings.startingSpawns; i++) SpawnNext();
        }

        // Generate the full list of size and speeds so that we can maintain an approximate average
        // this way we don't get wildly different difficulties each time the game is run
        // For example, if we just chose speed randomly each time we spawned an asteroid, in some
        // cases that might result in the first set of asteroids all going at max speed, or min speed
        private void GenerateRandomAttributes()
        {
            Assert.That(_cachedAttributes.Count == 0);

            var speedTotal = 0.0f;
            var sizeTotal = 0.0f;

            for (var i = 0; i < _settings.maxSpawns; i++)
            {
                var sizePx = Random.Range(0.0f, 1.0f);
                var speed = Random.Range(_settings.minSpeed, _settings.maxSpeed);

                _cachedAttributes.Enqueue(new AsteroidAttributes
                {
                    SizePx = sizePx,
                    InitialSpeed = speed
                });

                speedTotal += speed;
                sizeTotal += sizePx;
            }

            var desiredAverageSpeed = (_settings.minSpeed + _settings.maxSpeed) * 0.5f;
            var desiredAverageSize = 0.5f;

            var averageSize = sizeTotal / _settings.maxSpawns;
            var averageSpeed = speedTotal / _settings.maxSpawns;

            var speedScaleFactor = desiredAverageSpeed / averageSpeed;
            var sizeScaleFactor = desiredAverageSize / averageSize;

            foreach (var attributes in _cachedAttributes)
            {
                attributes.SizePx *= sizeScaleFactor;
                attributes.InitialSpeed *= speedScaleFactor;
            }

            Assert.That(Mathf.Approximately(_cachedAttributes.Average(x => x.InitialSpeed), desiredAverageSpeed));
            Assert.That(Mathf.Approximately(_cachedAttributes.Average(x => x.SizePx), desiredAverageSize));
        }

        private void ResetAll()
        {
            foreach (var asteroid in _asteroids) Object.Destroy(asteroid.gameObject);

            _asteroids.Clear();
            _cachedAttributes.Clear();
        }

        public void Stop()
        {
            Assert.That(_started);
            _started = false;
        }

        public void SpawnNext()
        {
            var asteroid = _asteroidFactory.Create();

            var attributes = _cachedAttributes.Dequeue();

            asteroid.Scale = Mathf.Lerp(_settings.minScale, _settings.maxScale, attributes.SizePx);
            asteroid.Mass = Mathf.Lerp(_settings.minMass, _settings.maxMass, attributes.SizePx);
            asteroid.Position = GetRandomStartPosition(asteroid.Scale);
            asteroid.Velocity = GetRandomDirection() * attributes.InitialSpeed;

            _asteroids.Add(asteroid);
        }

        private Vector3 GetRandomDirection()
        {
            var theta = Random.Range(0, Mathf.PI * 2.0f);
            return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
        }

        private Vector3 GetRandomStartPosition(float scale)
        {
            var side = (Side)Random.Range(0, (int)Side.Count);
            var rand = Random.Range(0.0f, 1.0f);

            switch (side)
            {
                case Side.Top:
                {
                    return new Vector3(_level.Left + rand * _level.Width, _level.Top + scale, 0);
                }
                case Side.Bottom:
                {
                    return new Vector3(_level.Left + rand * _level.Width, _level.Bottom - scale, 0);
                }
                case Side.Right:
                {
                    return new Vector3(_level.Right + scale, _level.Bottom + rand * _level.Height, 0);
                }
                case Side.Left:
                {
                    return new Vector3(_level.Left - scale, _level.Bottom + rand * _level.Height, 0);
                }
            }

            throw Assert.CreateException();
        }

        private enum Side
        {
            Top,
            Bottom,
            Left,
            Right,
            Count
        }

        [Serializable]
        public class Settings
        {
            public float minSpeed;
            public float maxSpeed;

            public float minScale;
            public float maxScale;

            public int startingSpawns;
            public int maxSpawns;

            public float maxSpawnTime;

            public float maxMass;
            public float minMass;
        }

        private class AsteroidAttributes
        {
            public float InitialSpeed;
            public float SizePx;
        }
    }
}