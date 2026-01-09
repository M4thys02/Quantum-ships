using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using TMPro;

public class GameSetup : MonoBehaviour {
    public GridManager _gridManager;
    public ProbabilitySquaresManager _probSquareManager;
    [SerializeField] Camera _camera;
    [SerializeField] TextMeshProUGUI probabilityPercentige;

    private int defaultGridSize = 3;
    private int defaultProbability = 10;
    private int defaultHeight = 850;
    private int oneTileSizes = 64;
    public bool playWithAI = false;
    public int currentGridSize;
    public int currentProbability;
    public float gridScale {  get; private set; }

    public void GoToScene(string sceneName) {
        if (_probSquareManager.RemainingSquares > 0) {
            return;
        }

        if (playWithAI) {
            SceneManager.LoadScene("MainGame");
        }
        else if (sceneName == "Player2") {
            PlayersSetUps.ChangePlayer();
            SceneManager.LoadScene(sceneName);
        }
        else {
            //PlayersSetUps.ShowDictionaries();
            SceneManager.LoadScene(sceneName);
        }
    }

    void Awake() {
        currentGridSize = (int)PlayerPrefs.GetFloat("GridSlider", defaultGridSize);
        currentProbability = (int)PlayerPrefs.GetFloat("SquareSlider", defaultProbability);

        gridScale = (float)defaultHeight / (oneTileSizes * currentGridSize);

        _gridManager.setUpGrid(currentGridSize, gridScale);

        _probSquareManager.setUpProbSquares(currentProbability, currentGridSize, gridScale);

        Vector3 gridBasedPosition = new Vector3((float)currentGridSize / 2 * gridScale, (float)currentGridSize / 2 * gridScale, -10);
        _camera.transform.position = gridBasedPosition;
    }
}
