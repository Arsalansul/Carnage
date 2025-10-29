using UnityEngine;

namespace Zenject.SpaceFighter
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;

        [SerializeField] private Collider _collider;

        [SerializeField] private Rigidbody _rigidBody;

        [Inject] public EnemyFacade Facade { get; set; }

        public MeshRenderer Renderer => _renderer;

        public Collider Collider => _collider;

        public Rigidbody Rigidbody => _rigidBody;

        public Vector3 LookDir => -_rigidBody.transform.right;

        public Vector3 RightDir => _rigidBody.transform.up;

        public Vector3 ForwardDir => _rigidBody.transform.right;

        public Vector3 Position
        {
            get => _rigidBody.transform.position;
            set => _rigidBody.transform.position = value;
        }

        public Quaternion Rotation
        {
            get => _rigidBody.rotation;
            set => _rigidBody.rotation = value;
        }

        public Vector3 Velocity => _rigidBody.velocity;

        public Vector3 AngularVelocity
        {
            get => _rigidBody.angularVelocity;
            set => _rigidBody.angularVelocity = value;
        }

        public void AddForce(Vector3 force)
        {
            _rigidBody.AddForce(force);
        }

        public void AddTorque(float value)
        {
            _rigidBody.AddTorque(Vector3.forward * value);
        }
    }
}