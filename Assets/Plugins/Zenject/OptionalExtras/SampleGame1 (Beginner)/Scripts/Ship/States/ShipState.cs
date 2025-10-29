using System;
using UnityEngine;

namespace Zenject.Asteroids
{
    public abstract class ShipState : IDisposable
    {
        public virtual void Dispose()
        {
            // optionally overridden
        }

        public abstract void Update();

        public virtual void Start()
        {
            // optionally overridden
        }

        public virtual void OnTriggerEnter(Collider other)
        {
            // optionally overridden
        }
    }
}