using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Extensions.ExMath;
using Extensions.ExTask;

namespace CustomUI.Elements
{
    public class UIElementPresenter : MonoBehaviour
    {
        [SerializeField] private float animationTime = 0.5f;
        [SerializeField] private AnimationCurve animationCurve;
        [SerializeField] private Vector2 showStatePivot;
        [SerializeField] private Vector2 hideStatePivot;
        [SerializeField] private RectTransform targetRect;

        private bool IsShown => targetRect.pivot == showStatePivot;
        private bool IsHidden => targetRect.pivot == hideStatePivot;

        public UnityEvent onShow;
        public UnityEvent onHide;

        private CancellationTokenSource cancellationTokenSource;

        public async Task Show(bool animated = true)
        {
            if (IsShown) return;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationSourceRoot.Token);

            if (animated)
            {
                var pivot = targetRect.pivot;
                var time = CalculateTime(showStatePivot, hideStatePivot);
                await MathExtensions.InterpolateTime(time,
                    t => targetRect.pivot = Vector2.Lerp(pivot, showStatePivot,
                        animationCurve.Evaluate(t)), cancellationTokenSource.Token);
                InnerForceShow();
            }
            else
            {
                InnerForceShow();
            }
        }

        public async Task Hide(bool animated = true)
        {
            if (IsHidden) return;

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationSourceRoot.Token);

            if (animated)
            {
                var pivot = targetRect.pivot;
                var time = CalculateTime(hideStatePivot, showStatePivot);
                await MathExtensions.InterpolateTime(time,
                    t => targetRect.pivot = Vector2.Lerp(pivot, hideStatePivot,
                        animationCurve.Evaluate(t)), cancellationTokenSource.Token);
                InnerForceHide();
            }
            else
            {
                InnerForceHide();
            }
        }

        private float CalculateTime(Vector2 v1, Vector2 v2)
        {
            var pivot = targetRect.pivot;
            var xInverse = Mathf.InverseLerp(v1.x, v2.x, pivot.x);
            var yInverse = Mathf.InverseLerp(v1.y, v2.y, pivot.y);
            return Mathf.Max(xInverse, yInverse) * animationTime;
        }

        private void InnerForceShow()
        {
            targetRect.pivot = showStatePivot;
            onShow.Invoke();
        }

        private void InnerForceHide()
        {
            targetRect.pivot = hideStatePivot;
            onHide.Invoke();
        }

        [ContextMenu("Show")]
        private async void ContextShow() => await Show();

        [ContextMenu("Hide")]
        private async void ContextHide() => await Hide();
    }
}