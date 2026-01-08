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
        OffOnButtons(true);
        _changePlayerManager.ChangePlayer();
        _measureManager.UpdateMeasurementsVisibility(_changePlayerManager.GetActivatePlayer());
    }

    public void PlayerAttack() {
        OffOnButtons(false);
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
            nextPlayerButton.gameObject.SetActive(false);
        }
    }


    public void PlayerMeasure() {
        OffOnButtons(false);
        int opponent= _changePlayerManager.GetOpponent();
        int attacker = _changePlayerManager.GetActivatePlayer();
        Vector2Int measuredTile = PlayersSetUps.GetWeightedRandomTileForPlayer(opponent);
        _measureManager.CurrentMeasurement(measuredTile, attacker);
    }

    private void OffOnButtons(bool onOff) {
        measureButton.gameObject.SetActive(onOff);
        attackPlayerButton.gameObject.SetActive(onOff);
    }

    private void AddGuessedSquares(int player, int amount) {
        if (player == 0)
            p0GuessedSquaresCount += amount;
        else
            p1GuessedSquaresCount += amount;
    }
}
