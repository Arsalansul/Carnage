using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Zenject.SpaceFighter
{
    public class EnemyStateIdle : IEnemyState
    {
        private readonly EnemyRotationHandler _rotationHandler;
        private readonly Settings _settings;
        private readonly EnemyView _view;
        private Vector3 _startLookDir;

        private Vector3 _startPos;
        private float _theta;

        public EnemyStateIdle(
            EnemyView view, Settings settings,
            EnemyRotationHandler rotationHandler)
        {
            _rotationHandler = rotationHandler;
            _settings = settings;
            _view = view;
        }

        public void EnterState()
        {
            _startPos = _view.Position;
            _theta = Random.Range(0, 2.0f * Mathf.PI);
            _startLookDir = _view.LookDir;
        }

        public void ExitState()
        {
        }

        // Just add a bit of subtle movement
        public void Update()
        {
            _theta += Time.deltaTime * _settings.Frequency;

            _view.Position = _startPos + _view.RightDir * _settings.Amplitude * Mathf.Sin(_theta);

            _rotationHandler.DesiredLookDir = _startLookDir;
        }

        public void FixedUpdate()
        {
        }

        [Serializable]
        public class Settings
        {
            public float Amplitude;
            public float Frequency;
        }
    }
}