using DG.Tweening;
using UnityEngine;

namespace DoTweenAnimations
{
    public class RotateCircleTween : MonoBehaviour
    {
        [SerializeField] private float duration;
        
        private Tween tween;

        private void OnEnable()
        {
            tween = transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        private void OnDisable()
        {
            tween.Kill();
        }
    }
}