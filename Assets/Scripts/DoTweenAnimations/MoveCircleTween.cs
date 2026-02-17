using DG.Tweening;
using UnityEngine;

namespace DoTweenAnimations
{
    public class MoveCircleTween : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Vector3 endLocalPosition;
        [SerializeField] private float duration;

        private Tween tween;

        private void OnEnable()
        {
            tween = targetTransform.DOLocalMove(endLocalPosition, duration).SetEase(Ease.InSine).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
            tween.Kill();
        }
    }
}