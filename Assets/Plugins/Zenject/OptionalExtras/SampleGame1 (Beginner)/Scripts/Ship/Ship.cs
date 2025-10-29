using UnityEngine;

#pragma warning disable 649
#pragma warning disable 618

namespace Zenject.Asteroids
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

#if UNITY_2018_1_OR_NEWER
        [SerializeField] private ParticleSystem _particleSystem;
#else
        [SerializeField]
        ParticleEmitter _particleEmitter;
#endif

        private ShipStateFactory _stateFactory;
        private ShipState _state;

        [Inject]
        public void Construct(ShipStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public MeshRenderer MeshRenderer => _meshRenderer;

#if UNITY_2018_1_OR_NEWER
        public ParticleSystem ParticleEmitter => _particleSystem;
#else
        public ParticleEmitter ParticleEmitter
        {
            get { return _particleEmitter; }
        }
#endif

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public void Start()
        {
            ChangeState(ShipStates.WaitingToStart);
        }

        public void Update()
        {
            _state.Update();
        }

        public void OnTriggerEnter(Collider other)
        {
            _state.OnTriggerEnter(other);
        }

        public void ChangeState(ShipStates state)
        {
            if (_state != null)
            {
                _state.Dispose();
                _state = null;
            }

            _state = _stateFactory.CreateState(state);
            _state.Start();
        }
    }
}