using UnityEngine;

#pragma warning disable 649

namespace Zenject.Asteroids
{
    public class TilingBackground : MonoBehaviour
    {
        [SerializeField] private float _speed;

        private Vector2 _offset;
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            _offset.y += _speed * Time.deltaTime;
            _renderer.material.mainTextureOffset = _offset;
        }
    }
}