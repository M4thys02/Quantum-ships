using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private ChangePlayerManager _changePlayerManager;
    private MeasureManager _measureManager;

    private int defaultProbability = 10;
    private int guessedSquaresCount = 0;

    [SerializeField] Button measureButton;
    [SerializeField] Button attackPlayerButton;
    [SerializeField] Button nextPlayerButton;

    private void Awake() {
        _changePlayerManager = GetComponentInChildren<ChangePlayerManager>();
        _measureManager = GetComponentInChildren<MeasureManager>();
    }
    public void GameFinished() {
        PlayersSetUps.Cleanup(); // destroy persistent object
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangePlayer() {
        _changePlayerManager.ChangePlayer();
        _measureManager.UpdateMeasurementsVisibility(_changePlayerManager.GetActivatePlayer());
    }

    public void PlayerAttack() {
        int currentPlayer = _changePlayerManager.GetActivatePlayer();
        Dictionary<Vector3Int, List<GameObject>> playerSquares = _changePlayerManager.GetActiveSquaresDict();
        Dictionary<Vector2Int, int> expectedTiles = PlayersSetUps.GetKeyValuePairs(currentPlayer);

        foreach (var kv in expectedTiles) {
            Vector2Int tilePos2D = kv.Key;
            int expectedCount = kv.Value;
            Vector3Int tilePos3D = new Vector3Int(tilePos2D.x, tilePos2D.y, 0);

            int actualCount = playerSquares.ContainsKey(tilePos3D) ? playerSquares[tilePos3D].Count : 0;

            if (actualCount == expectedCount) {
                //Debug.Log($"Player {currentPlayer} uhodl políčko {tilePos3D}! ({actualCount}/{expectedCount})");
                _changePlayerManager.CreateGuessedSquares(tilePos3D, expectedCount);
                guessedSquaresCount += actualCount;
            }
        }

        if (guessedSquaresCount >= (int)PlayerPrefs.GetFloat("SquareSlider", defaultProbability)) {
            PlayerWinGame();
        }
    }

    public void PlayerMeasure() {
        int currentPlayer = _changePlayerManager.GetActivatePlayer();
        Vector2Int measuredTile = PlayersSetUps.GetWeightedRandomTileForPlayer(currentPlayer);
        _measureManager.CurrentMeasurement(measuredTile, currentPlayer);
    }

    private void PlayerWinGame() {
        measureButton.gameObject.SetActive(false);
        attackPlayerButton.gameObject.SetActive(false);
        nextPlayerButton.gameObject.SetActive(false);
    }
}
