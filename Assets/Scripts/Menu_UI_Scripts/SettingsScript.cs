using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private Sliders _gridSlider;
    [SerializeField] private Sliders _squaresSlider;
    private string togglePrefsKey = "ColorBlindMode";
    [SerializeField] private Toggle _toggle;
    [SerializeField] public int _gridSize {  get; set; }
    [SerializeField] public int _probSquaresCount { get; set; }

    private void Awake() {
        _toggle.isOn = PlayerPrefs.GetInt("ColorBlindMode") == 1;
    }
    public void GoToScene(string sceneName) {
        setUpGame();
        SceneManager.LoadScene(sceneName);
    }

    public void setUpGame() {
        _gridSize = (int)_gridSlider.GetSliderValue();
        _probSquaresCount = (int)_squaresSlider.GetSliderValue();

        //Debug.Log($"Grid size is: {_gridSize}, Number of probability squares is: {_probSquaresCount}");

    }

    public void SaveToggle() {
        PlayerPrefs.SetInt(togglePrefsKey, _toggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
