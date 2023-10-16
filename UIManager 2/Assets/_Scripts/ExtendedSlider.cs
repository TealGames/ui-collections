using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ExtendedSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI valueText;
        [Tooltip("If true, on Start() will set the slider's default value for OnValueChanged() UnityEvent")][SerializeField] private bool callValueChangedOnDefaultValue;
        //public MethodInfo minMaxMethodInfo { get; set; } = null;

        // Start is called before the first frame update
        void Start()
        {
            SetCurrentSliderValueText();
            slider.onValueChanged.AddListener((float newValue) => SetCurrentSliderValueText());

            if (callValueChangedOnDefaultValue) slider.onValueChanged.Invoke(slider.value);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetCurrentSliderValueText() => valueText.text = slider.value.ToString();

        public void SetSliderMinAndMax(Vector2 minMax)
        {
            slider.minValue = minMax.x;
            slider.maxValue = minMax.y;
        }

        public void SetSliderValue(float value)
        {
            if (slider.wholeNumbers) slider.value = (int)value;
            else slider.value = value;
        }
    }
}
