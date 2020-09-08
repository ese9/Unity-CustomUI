using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Elements
{
    public class PageDots : MonoBehaviour
    {
        [SerializeField] private Image originDot;
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;
        [SerializeField] private FixedScrollView fixedScrollView;
        private List<Image> dotImages = null;

        private void Awake()
        {
            fixedScrollView.onScrollPageCreated.AddListener(OnScrollPageCreatedHandler);
            fixedScrollView.onScrollPageChanged.AddListener(OnScrollPageChangedHandler);
            dotImages = new List<Image>();
            originDot.gameObject.SetActive(false);
        }

        private void OnScrollPageCreatedHandler(int pagesCount)
        {
            ClearDots();

            dotImages = new List<Image>();
            if (pagesCount <= 1)
            {
                return;
            }

            dotImages.Add(originDot);
            originDot.gameObject.SetActive(true);
            originDot.color = activeColor;

            for (int i = 1; i < pagesCount; i++)
            {
                var dot = Instantiate(originDot, originDot.transform.parent, true);
                dot.color = inactiveColor;
                dotImages.Add(dot);
            }
        }

        private void ClearDots()
        {
            dotImages.Remove(originDot);
            originDot.gameObject.SetActive(false);
            for (int i = 0; i < dotImages.Count; i++)
            {
                var dot = dotImages[i];
                Destroy(dot.gameObject);
            }

            dotImages.Clear();
        }

        private void OnScrollPageChangedHandler(int pageNumber)
        {
            for (int i = 0; i < dotImages.Count; i++)
            {
                dotImages[i].color = i == pageNumber ? activeColor : inactiveColor;
            }
        }
    }
}