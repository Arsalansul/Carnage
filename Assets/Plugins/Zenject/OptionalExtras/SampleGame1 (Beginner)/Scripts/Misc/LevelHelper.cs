using UnityEngine;

namespace Zenject.Asteroids
{
    public class LevelHelper
    {
        private readonly Camera _camera;

        public LevelHelper(
            [Inject(Id = "Main")] Camera camera)
        {
            _camera = camera;
        }

        public float Bottom => -ExtentHeight;

        public float Top => ExtentHeight;

        public float Left => -ExtentWidth;

        public float Right => ExtentWidth;

        public float ExtentHeight => _camera.orthographicSize;

        public float Height => ExtentHeight * 2.0f;

        public float ExtentWidth => _camera.aspect * _camera.orthographicSize;

        public float Width => ExtentWidth * 2.0f;
    }
}