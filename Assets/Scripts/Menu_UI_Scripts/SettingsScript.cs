using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private Sliders _gridSlider;
    [SerializeField] private Sliders _squaresSlider;
    [SerializeField] public int _gridSize {  get; set; }
    [SerializeField] public int _probSquaresCount { get; set; }
    public void GoToScene(string sceneName) {
        setUpGame();
        SceneManager.LoadScene(sceneName);
    }

    public void setUpGame() {
        _gridSize = (int)_gridSlider.GetSliderValue();
        _probSquaresCount = (int)_squaresSlider.GetSliderValue();

        //Debug.Log($"Grid size is: {_gridSize}, Number of probability squares is: {_probSquaresCount}");

    }
}
