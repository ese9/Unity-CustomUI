using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Elements
{
    [ExecuteAlways]
    [RequireComponent(typeof(Slider))]
    public class ColorSlider : MonoBehaviour
    {
        [SerializeField] private Gradient colorGradient;
        [SerializeField] private Image targetGraphics;

        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(ChangeColor);
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(ChangeColor);
        }

        public void ChangeColor(float value)
        {
            if (!targetGraphics) return;

            targetGraphics.color = colorGradient.Evaluate(slider.normalizedValue);
        }
    }
}