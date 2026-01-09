using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {
    private ChangePlayerManager _changePlayerManager;
    private MeasureManager _measureManager;

    [SerializeField] TextMeshProUGUI probabilityPercentige;
    private int defaultProbability = 10;
    private int maximumProbability = 100;
    private int currentProbability;
    private int p0GuessedSquaresCount = 0;
    private int p1GuessedSquaresCount = 0;
    private HashSet<Vector3Int> p0ResolvedTiles = new();
    private HashSet<Vector3Int> p1ResolvedTiles = new();

    [SerializeField] Button measureButton;
    [SerializeField] Button attackPlayerButton;
    [SerializeField] Button nextPlayerButton;

    private void Awake() {
        _changePlayerManager = GetComponentInChildren<ChangePlayerManager>();
        _measureManager = GetComponentInChildren<MeasureManager>();
        currentProbability = (int)PlayerPrefs.GetFloat("SquareSlider", defaultProbability);

        float oneSquareProbability = maximumProbability / (float)currentProbability;
        probabilityPercentige.text = $"= {oneSquareProbability:F2} %";
    }
    public void GameFinished() {
        p0ResolvedTiles.Clear();
        p1ResolvedTiles.Clear();
        PlayersSetUps.Cleanup(); // destroy persistent object
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangePlayer() {
        Debug.Log($"player0: {p0GuessedSquaresCount}");
        Debug.Log($"player1: {p1GuessedSquaresCount}");
        OffOnButtons(true);
        _changePlayerManager.ChangePlayer();
        _measureManager.UpdateMeasurementsVisibility(_changePlayerManager.GetActivatePlayer());
    }

    public void PlayerAttack() {
        OffOnButtons(false);
        int attacker = _changePlayerManager.GetActivatePlayer();
        int defender = attacker ^ 1;

        var attackSquares = _changePlayerManager.GetActiveSquaresDict();
        var defenderTiles = PlayersSetUps.GetKeyValuePairs(defender);
        var resolvedTiles = (attacker == 0) ? p0ResolvedTiles : p1ResolvedTiles;

        foreach (var kv in defenderTiles) {
            Vector3Int tilePos = new(kv.Key.x, kv.Key.y, 0);

            // If this tile was already resolved, skip it
            if (resolvedTiles.Contains(tilePos))
                continue;

            if (!attackSquares.TryGetValue(tilePos, out var list))
                continue;

            int expected = kv.Value;
            int guessed = list.Count;

            if (guessed == expected) {
                resolvedTiles.Add(tilePos);
                _changePlayerManager.CreateGuessedSquares(tilePos, expected);
                AddGuessedSquares(attacker, guessed);
            }
        }

        CheckWinCondition();
    }


    public void PlayerMeasure() {
        OffOnButtons(false);
        int opponent = _changePlayerManager.GetOpponent();
        int attacker = _changePlayerManager.GetActivatePlayer();
        Vector2Int measuredTile = PlayersSetUps.GetWeightedRandomTileForPlayer(opponent);
        _measureManager.CurrentMeasurement(measuredTile, attacker);
    }

    private void OffOnButtons(bool onOff) {
        measureButton.gameObject.SetActive(onOff);
        attackPlayerButton.gameObject.SetActive(onOff);
    }

    private void AddGuessedSquares(int player, int amount) {
        if (player == 0) p0GuessedSquaresCount += amount; 
        else p1GuessedSquaresCount += amount;
    }

    private void CheckWinCondition() {
        if (p0GuessedSquaresCount >= currentProbability || p1GuessedSquaresCount >= currentProbability) {
            nextPlayerButton.gameObject.SetActive(false);
            //TODO: winning screen
        }
    }
}
