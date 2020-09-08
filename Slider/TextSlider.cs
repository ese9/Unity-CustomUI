using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI.Elements
{
    [ExecuteAlways]
    [RequireComponent(typeof(Slider))]
    public class TextSlider : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField, TextArea] private string prefix = string.Empty;
        [SerializeField, TextArea] private string postfix = string.Empty;

        private Slider progressSlider;

        private void Awake()
        {
            progressSlider = GetComponent<Slider>();
            progressSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnDestroy()
        {
            progressSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        private void OnSliderValueChanged(float value)
        {
            if (displayText == null)
            {
                Debug.LogError("Slider progress text is null");
                return;
            }

            displayText.text = $"{prefix}{value}{postfix}";
        }
    }
}