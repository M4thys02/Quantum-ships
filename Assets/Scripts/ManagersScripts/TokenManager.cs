using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TokenManager : MonoBehaviour {
    [Header("All squares prefabs")]
    [SerializeField] private GameObject _attackSquarePrefab;
    [SerializeField] private GameObject _guessedSquarePrefab;
    [SerializeField] private GameObject _probabilitySquarePrefab;
    [SerializeField] private GameObject _colorBlindModeSquares;
    private GameObject _guessedSquares;

    // Data: [PlayerIndex][Coordinates] -> List of objects
    private Dictionary<Vector3Int, List<GameObject>>[] _playerSquares;
    private int[] _attackSquaresCount = new int[2];
    private int[] _guessedSquaresCount = new int[2];
    private int _maxSquaresPerTurn;

    private TurnManager _turnManager;
    private BoardManager _boardManager;
    private UIManager _uiManager;

    public void Initialize(TurnManager tm, BoardManager bm, UIManager ui) {
        _turnManager = tm;
        _boardManager = bm;
        _uiManager = ui;

        _maxSquaresPerTurn = (int)PlayerPrefs.GetFloat("SquareSlider", 10);
        bool value = PlayerPrefs.GetInt("ColorBlindMode") == 1;

        _guessedSquares = value ? _colorBlindModeSquares : _guessedSquarePrefab;

        _playerSquares = new Dictionary<Vector3Int, List<GameObject>>[2];
        _playerSquares[0] = new Dictionary<Vector3Int, List<GameObject>>();
        _playerSquares[1] = new Dictionary<Vector3Int, List<GameObject>>();

        _turnManager.OnTurnChanged += (prev, curr) => {
            UpdateVisuals(curr);
        };
    }

    public void OnTileInteract(Vector3Int cellPos, bool isAdding) {
        int player = _turnManager.CurrentPlayer;
        int available = _maxSquaresPerTurn - _attackSquaresCount[player] - _guessedSquaresCount[player];

        if (!_playerSquares[player].ContainsKey(cellPos))
            _playerSquares[player][cellPos] = new List<GameObject>();

        var list = _playerSquares[player][cellPos];

        if (IsTileResolved(list)) return;

        if (isAdding) {
            if (available <= 0) return;
            AddSquare(cellPos, list);
            _attackSquaresCount[player]++;
        }
        else {
            if (list.Count == 0) return;
            RemoveSquare(list);
            _attackSquaresCount[player] = Mathf.Max(0, _attackSquaresCount[player] - 1);
        }
        Tilemap activeTilemap = _boardManager.GetActiveTilemap();
        Vector3 worldPos = activeTilemap.GetCellCenterWorld(cellPos);
        _uiManager.UpdateTileCounter(player, cellPos, list.Count, worldPos, _boardManager.GridSize, activeTilemap);
    }

    private void AddSquare(Vector3Int pos, List<GameObject> list) {
        Tilemap targetMap = _boardManager.GetActiveTilemap();
        Vector3 worldPos = targetMap.GetCellCenterWorld(pos);
        GameObject sq = Instantiate(_attackSquarePrefab, worldPos, Quaternion.identity, targetMap.transform); // ← parent = tilemap
        sq.transform.localScale = Vector3.one;
        list.Add(sq);
    }

    private void RemoveSquare(List<GameObject> list) {
        GameObject sq = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        Destroy(sq);
    }

    public void MarkTileAsGuessed(Vector3Int pos, int count) {
        int attacker = _turnManager.CurrentPlayer;
        var dict = _playerSquares[attacker];

        // Removing attackers attack squares
        if (dict.TryGetValue(pos, out var list)) {
            foreach (var sq in list) Destroy(sq);
            list.Clear();
        }
        else {
            dict[pos] = new List<GameObject>();
            list = dict[pos];
        }

        Tilemap targetMap = _boardManager.GetActiveTilemap();
        Vector3 worldPos = targetMap.GetCellCenterWorld(pos);

        for (int i = 0; i < count; i++) {
            GameObject g = Instantiate(_guessedSquares, worldPos, Quaternion.identity, targetMap.transform);
            g.transform.localScale = Vector3.one;
            list.Add(g);
        }

        _attackSquaresCount[attacker] -= count;
        _guessedSquaresCount[attacker] += count;
    }

    // Method for updating visuals between individual turns
    private void UpdateVisuals(int activePlayer) {
        for (int i = 0; i < 2; i++) {
            bool isVisible = (i == activePlayer);
            foreach (var kvp in _playerSquares[i]) {
                foreach (var obj in kvp.Value) {
                    if (obj != null) obj.SetActive(isVisible);
                }
            }
        }
    }

    public void ShowVisualsForBothPlayers() {
        for (int i = 0; i < 2; i++) {
            foreach (var kvp in _playerSquares[i]) {
                foreach (var obj in kvp.Value) {
                    if (obj != null) obj.SetActive(true);
                }
            }
        }
    }

    public Dictionary<Vector3Int, List<GameObject>> GetActiveSquares() {
        return _playerSquares[_turnManager.CurrentPlayer];
    }

    private bool IsTileResolved(List<GameObject> list) {
        return list.Count > 0 && list[0].CompareTag("GuessedSquare");
    }

    public void RevealLoserBoard(int loserIndex) {

        // ======================================================
        // PURPOSE
        // Reveals the final state of the board for the losing player.
        // Shows:
        //  - Blue probability squares where the player failed to guess
        //  - Numeric counters comparing guesses vs actual distribution
        // ======================================================

        // ------------------------------------------------------
        // 1) Determine opponent and retrieve data
        // ------------------------------------------------------

        // Opponent index (the player whose true setup we reveal)
        int opponent = (loserIndex == 0) ? 1 : 0;

        // True probability distribution of the opponent
        // Key: Vector2Int (cell position), Value: probability count
        var trueSolution = PlayersSetUps.GetKeyValuePairs(opponent);

        // Player's guesses (red estimation squares)
        // Key: Vector3Int (cell position), Value: list of guessed squares
        var playerGuesses = _playerSquares[loserIndex];

        // ------------------------------------------------------
        // 2) Collect all relevant board positions
        // ------------------------------------------------------
        // We need to process the union of:
        //  - cells the player guessed
        //  - cells that actually contain probability squares
        HashSet<Vector3Int> allPositions = new HashSet<Vector3Int>();
        foreach (var kvp in playerGuesses) allPositions.Add(kvp.Key);
        foreach (var kvp in trueSolution) allPositions.Add(new Vector3Int(kvp.Key.x, kvp.Key.y, 0));

        // ------------------------------------------------------
        // 3) Iterate over all relevant cells
        // ------------------------------------------------------

        foreach (Vector3Int pos in allPositions) {

            Vector2Int pos2D = new Vector2Int(pos.x, pos.y);

            // Actual number of probability squares on this cell
            int actualCount = trueSolution.ContainsKey(pos2D) ? trueSolution[pos2D] : 0;

            // Number of squares guessed by the losing player
            int guessedCount = 0;
            if (playerGuesses.TryGetValue(pos, out var list)) {
                guessedCount = list.Count;
            }

            // --------------------------------------------------
            // A) Reveal missing probability squares (blue)
            // Case: player guessed nothing, but something was there
            // --------------------------------------------------
            if (guessedCount == 0 && actualCount > 0) {

                // Ensure internal storage exists
                if (!_playerSquares[loserIndex].ContainsKey(pos)) {
                    _playerSquares[loserIndex][pos] = new List<GameObject>();
                }

                // Determine which tilemap the loser was attacking
                // Player 0 attacks Player 1's tilemap and vice versa
                Tilemap attackMap = (loserIndex == 0) ? _boardManager.Player1TilemapRef : _boardManager.Player0TilemapRef;

                // World position of the cell center
                Vector3 worldPos = attackMap.GetCellCenterWorld(pos);

                // Instantiate missing blue probability squares
                for (int i = 0; i < actualCount; i++) {
                    GameObject blueSq = Instantiate(_probabilitySquarePrefab, worldPos, Quaternion.identity, attackMap.transform);

                    blueSq.transform.localScale = Vector3.one;
                    _playerSquares[loserIndex][pos].Add(blueSq);
                }
            }

            // --------------------------------------------------
            // B) Update numeric UI counters
            // Shows guessed vs actual values where relevant
            // --------------------------------------------------

            Tilemap correctMap = (loserIndex == 0)
                ? _boardManager.Player1TilemapRef
                : _boardManager.Player0TilemapRef;

            Vector3 worldPosCounter = correctMap.GetCellCenterWorld(pos);

            _uiManager.UpdateTileCounterEndGame(loserIndex, pos, guessedCount, actualCount, worldPosCounter, _boardManager.GridSize, correctMap);
        }
    }

}