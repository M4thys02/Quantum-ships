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
    private int p0GuessedSquaresCount = 0;
    private int p1GuessedSquaresCount = 0;

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
        int attacker = _changePlayerManager.GetActivatePlayer();
        int defender = attacker ^ 1;

        Dictionary<Vector3Int, List<GameObject>> attackSquares = _changePlayerManager.GetActiveSquaresDict();
        Dictionary<Vector2Int, int> defenderTiles = PlayersSetUps.GetKeyValuePairs(defender);

        foreach (var kv in defenderTiles) {
            Vector3Int tilePos = new(kv.Key.x, kv.Key.y, 0);
            int expected = kv.Value;

            int guessed = attackSquares.TryGetValue(tilePos, out var list) ? list.Count : 0;

            if (guessed == expected) {
                _changePlayerManager.CreateGuessedSquares(tilePos, expected);
                AddGuessedSquares(attacker, guessed);
            }
        }

        if (p0GuessedSquaresCount == (int)PlayerPrefs.GetFloat("SquareSlider", defaultProbability) || p1GuessedSquaresCount == (int)PlayerPrefs.GetFloat("SquareSlider", defaultProbability)) { 
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

    private void AddGuessedSquares(int player, int amount) {
        if (player == 0)
            p0GuessedSquaresCount += amount;
        else
            p1GuessedSquaresCount += amount;
    }
}
