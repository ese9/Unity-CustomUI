using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Elements
{
    public class PageNavigation : MonoBehaviour
    {
        [SerializeField] private FixedScrollView fixedScrollView;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button prevButton;

        private void Awake()
        {
            fixedScrollView.onScrollPageChanged.AddListener(OnPageChangedHandler);
            nextButton.onClick.AddListener(OnNextButtonClick);
            prevButton.onClick.AddListener(OnPrevButtonClick);
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
        }

        private void OnPrevButtonClick()
        {
            fixedScrollView.PreviousPage();
        }

        private void OnNextButtonClick()
        {
            fixedScrollView.NextPage();
        }

        private void OnPageChangedHandler(int pageIndex)
        {
            nextButton.gameObject.SetActive(pageIndex < fixedScrollView.PagesCount - 1);
            prevButton.gameObject.SetActive(pageIndex > 0);
        }
    }
}