using UnityEngine;

namespace Zenject.SpaceFighter
{
    public class PlayerFacade : MonoBehaviour
    {
        private PlayerDamageHandler _hitHandler;
        private Player _model;

        public bool IsDead => _model.IsDead;

        public Vector3 Position => _model.Position;

        public Quaternion Rotation => _model.Rotation;

        [Inject]
        public void Construct(Player player, PlayerDamageHandler hitHandler)
        {
            _model = player;
            _hitHandler = hitHandler;
        }

        public void TakeDamage(Vector3 moveDirection)
        {
            _hitHandler.TakeDamage(moveDirection);
        }
    }
}