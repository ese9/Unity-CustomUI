using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Elements
{
    [RequireComponent(typeof(Slider))]
    public class SegmentSlider : MonoBehaviour
    {
        [SerializeField] private bool isEqualParts = true;
        [SerializeField] private SegmentInfo[] segments;
        [SerializeField] private RectTransform backgroundRoot;
        [HideInInspector, SerializeField] private Slider slider;

        private float[] sliderWeights = new float[0];
        private RectTransform[] childSegments;

        public float value
        {
            get => slider.value;
            set => slider.value = value;
        }

        public float[] weights => sliderWeights;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            childSegments = new RectTransform[backgroundRoot.childCount];
            sliderWeights = new float[segments.Length];

            for (int i = 0; i < segments.Length; i++)
            {
                sliderWeights[i] = segments[i].value;
            }

            for (int i = backgroundRoot.childCount - 1; i >= 0; i--)
            {
                childSegments[(childSegments.Length - 1) - i] =
                    backgroundRoot.transform.GetChild(i).transform as RectTransform;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            slider = GetComponent<Slider>();
            if (!backgroundRoot) return;

            if (segments.Length != backgroundRoot.transform.childCount)
            {
                UpdateSlider();
            }
        }

        private void ClearSegments(Action done)
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                for (int i = backgroundRoot.transform.childCount - 1; i >= 0; i--)
                {
                    var seg = backgroundRoot.transform.GetChild(i);
                    DestroyImmediate(seg.gameObject);
                }

                done?.Invoke();
            };
        }

        [ContextMenu("Update slider")]
        private void UpdateSlider()
        {
            ClearSegments(() =>
            {
                var tempSlider = GetComponent<Slider>();
                var totalSegments = segments.Length;
                var size = backgroundRoot.rect.size;
                switch (tempSlider.direction)
                {
                    case Slider.Direction.LeftToRight:
                        XFlow(-1);
                        break;
                    case Slider.Direction.RightToLeft:
                        XFlow(1);
                        break;
                    case Slider.Direction.TopToBottom:
                        YFlow(-1);
                        break;
                    case Slider.Direction.BottomToTop:
                        YFlow(1);
                        break;
                }

                void XFlow(int sign) =>
                    InnerUpdateSlider(size.x, totalSegments,
                        x => new Vector2(x, size.y),
                        x => new Vector2(x * sign, 0));

                void YFlow(int sign) =>
                    InnerUpdateSlider(size.y, totalSegments,
                        y => new Vector2(size.x, y),
                        y => new Vector2(0, y * sign));
            });
        }
#endif

        public void UpdateSliderSegmentsWeight(float[] segmentWeights)
        {
            var size = backgroundRoot.rect.size;
            switch (slider.direction)
            {
                case Slider.Direction.LeftToRight:
                    XFlow(-1);
                    break;
                case Slider.Direction.RightToLeft:
                    XFlow(1);
                    break;
                case Slider.Direction.TopToBottom:
                    YFlow(-1);
                    break;
                case Slider.Direction.BottomToTop:
                    YFlow(1);
                    break;
            }

            void XFlow(int sign) =>
                InnerUpdateWeight(size.x, segmentWeights,
                    x => new Vector2(x, size.y),
                    x => new Vector2(x * sign, 0));

            void YFlow(int sign) =>
                InnerUpdateWeight(size.y, segmentWeights,
                    y => new Vector2(size.x, y),
                    y => new Vector2(0, y * sign));
        }

        private void InnerUpdateWeight(float mainValue, float[] segmentWeights, Func<float, Vector2> sizeFunc,
            Func<float, Vector2> anchorFunc)
        {
            float prevValue = 0;
            for (int i = 0; i < segmentWeights.Length && i < childSegments.Length && i < segments.Length; i++)
            {
                var weight = segmentWeights[i];
                segments[i].value = sliderWeights[i] = weight;
                var rectTransform = childSegments[i];
                var val = mainValue * weight - prevValue;
                rectTransform.sizeDelta = sizeFunc(val);
                rectTransform.anchoredPosition = anchorFunc((val - mainValue) * 0.5f + prevValue);
                prevValue += val;
            }
        }

        private void InnerUpdateSlider(float mainValue, int totalSegments, Func<float, Vector2> sizeFunc,
            Func<float, Vector2> anchorFunc)
        {
            var stepSize = mainValue / totalSegments;
            float prevValue = 0;
            for (int i = 0; i < totalSegments; i++)
            {
                var info = segments[i];
                var segmentImage = new GameObject($"Segment {i}").AddComponent<Image>();
                segmentImage.color = info.color;
                segmentImage.transform.SetParent(backgroundRoot.transform);
                segmentImage.transform.SetAsFirstSibling();
                var segmentTransform = segmentImage.rectTransform;
                segmentTransform.localPosition = Vector3.zero;
                segmentTransform.localRotation = Quaternion.identity;
                segmentTransform.localScale = Vector3.one;
                if (isEqualParts)
                {
                    segmentTransform.sizeDelta = sizeFunc(stepSize);
                    segmentTransform.anchoredPosition =
                        anchorFunc(stepSize * (i - 0.5f * (totalSegments - 1)));
                }
                else
                {
                    var size = mainValue * info.value - prevValue;
                    segmentTransform.sizeDelta = sizeFunc(size);
                    segmentTransform.anchoredPosition = anchorFunc((size - mainValue) * 0.5f + prevValue);
                    prevValue += size;
                }
            }
        }


        [Serializable]
        public struct SegmentInfo
        {
            public Color color;
            public float value;
        }
    }
}