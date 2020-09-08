using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Extensions.ExMath;
using Extensions.ExTask;

namespace CustomUI.Elements
{
    [RequireComponent(typeof(ScrollRect))]
    public class FixedScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform pageOrigin;
        [SerializeField] private float swipeThreshold;
        [SerializeField] private float animationTime = 0.4f;

        private readonly List<Transform> createdPages = new List<Transform>();
        private ScrollRect scrollRect;
        private Vector2 startPosition;
        private CancellationTokenSource cancellationTokenSource;
        private int pages = 0;
        private int currentPage = 0;
        private bool isScrolling = false;

        public ScrollPageChanged onScrollPageChanged;
        public ScrollPageCreated onScrollPageCreated;

        public int PagesCount => pages;
        public int CurrentPage => currentPage;

        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            pageOrigin.gameObject.SetActive(false);
        }

        public void ClearPages()
        {
            for (int i = 0; i < createdPages.Count; i++)
            {
                Destroy(createdPages[i].gameObject);
            }

            pages = 0;
            currentPage = 0;
            createdPages.Clear();
        }

        public Transform CreateNewPage()
        {
            var page = Instantiate(pageOrigin, pageOrigin.parent, true);
            pages++;
            createdPages.Add(page);
            onScrollPageCreated.Invoke(pages);
            return page;
        }

        public void ShowPage(int pageIndex, bool animated = true)
        {
            if (pages == 0) return;

            var normalizedPage = pageIndex * (1 / (float) (pages - 1));
            currentPage = pageIndex;
            InnerPageChange(normalizedPage, animated ? animationTime : 0);
        }

        public void NextPage() => ChangePage(1);
        public void PreviousPage() => ChangePage(-1);

        public void OnBeginDrag(PointerEventData eventData) => startPosition = eventData.position;

        public void OnEndDrag(PointerEventData eventData)
        {
            var swipeDirection = eventData.position - startPosition;
            var swipeMag = swipeDirection.sqrMagnitude;
            if (swipeMag > swipeThreshold * swipeThreshold)
            {
                ChangePage(swipeDirection.normalized.x > 0 ? -1 : 1);
            }
        }

        private void ChangePage(int horizontalDirection)
        {
            if (isScrolling) return;

            if (currentPage == 0 && horizontalDirection < 0) return;
            if (currentPage >= pages - 1 && horizontalDirection > 0) return;

            StartScroll();
            ShowPage(currentPage + horizontalDirection);
        }

        private void FinishScroll()
        {
            onScrollPageChanged.Invoke(currentPage);
            isScrolling = false;
        }

        private void StartScroll() => isScrolling = true;

        private async Task InnerPageChange(float page, float time)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(CancellationSourceRoot.Token);
            var start = scrollRect.horizontalNormalizedPosition;
            await MathExtensions
                .InterpolateTime(time, t => scrollRect.horizontalNormalizedPosition = Mathf.Lerp(start, page, t),
                    cancellationTokenSource.Token).ContinueWithCurrentContext(FinishScroll);
        }

        [Serializable]
        public class ScrollPageChanged : UnityEvent<int>
        {
        }

        [Serializable]
        public class ScrollPageCreated : UnityEvent<int>
        {
        }
    }
}