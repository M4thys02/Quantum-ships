using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sliders : MonoBehaviour {
    [SerializeField] private string sliderKey = "MySlider";
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text textField;

    private void Awake() {
        slider = GetComponent<Slider>();
        textField = GetComponentInChildren<TMP_Text>();

        float savedValue = PlayerPrefs.GetFloat(sliderKey, slider.value);
        slider.value = savedValue;
        HandleSliderValueChange(savedValue);

        slider.onValueChanged.AddListener(HandleSliderValueChange);
    }

    public void HandleSliderValueChange(float value) {
        textField.SetText(value.ToString());
        PlayerPrefs.SetFloat(sliderKey, value);
        PlayerPrefs.Save();
    }

    public float GetSliderValue() => slider.value;
}
