using System;
using UnityEngine;

namespace Zenject.SpaceFighter
{
    // Here we can add some high-level methods to give some info to other
    // parts of the codebase outside of our enemy facade
    public class EnemyFacade : MonoBehaviour, IPoolable<float, float, IMemoryPool>, IDisposable
    {
        private EnemyDeathHandler _deathHandler;
        private IMemoryPool _pool;
        private EnemyRegistry _registry;
        private EnemyStateManager _stateManager;
        private EnemyTunables _tunables;
        private EnemyView _view;

        public EnemyStates State => _stateManager.CurrentState;

        public float Accuracy => _tunables.Accuracy;

        public float Speed => _tunables.Speed;

        public Vector3 Position
        {
            get => _view.Position;
            set => _view.Position = value;
        }

        public void Dispose()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
            _registry.RemoveEnemy(this);
            _pool = null;
        }

        public void OnSpawned(float accuracy, float speed, IMemoryPool pool)
        {
            _pool = pool;
            _tunables.Accuracy = accuracy;
            _tunables.Speed = speed;

            _registry.AddEnemy(this);
        }

        [Inject]
        public void Construct(
            EnemyView view,
            EnemyTunables tunables,
            EnemyDeathHandler deathHandler,
            EnemyStateManager stateManager,
            EnemyRegistry registry)
        {
            _view = view;
            _tunables = tunables;
            _deathHandler = deathHandler;
            _stateManager = stateManager;
            _registry = registry;
        }

        public void Die()
        {
            _deathHandler.Die();
        }

        public class Factory : PlaceholderFactory<float, float, EnemyFacade>
        {
        }
    }
}