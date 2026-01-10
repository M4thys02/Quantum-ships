using System;
using System.Collections.Generic;
using UnityEngine;

public class TokenManager : MonoBehaviour {
    [SerializeField] private GameObject _attackSquarePrefab;
    [SerializeField] private GameObject _guessedSquarePrefab;

    // Data: [PlayerIndex][Coordinates] -> List of objects
    private Dictionary<Vector3Int, List<GameObject>>[] _playerSquares;
    private int _currentPlacedCount = 0;
    private int _maxSquaresPerTurn;

    private TurnManager _turnManager;
    private BoardManager _boardManager;
    private UIManager _uiManager;

    public void Initialize(TurnManager tm, BoardManager bm, UIManager ui) {
        _turnManager = tm;
        _boardManager = bm;
        _uiManager = ui;

        // Načtení limitu čtverečků z PlayerPrefs (stejně jako v původním kódu)
        _maxSquaresPerTurn = (int)PlayerPrefs.GetFloat("SquareSlider", 10);

        _playerSquares = new Dictionary<Vector3Int, List<GameObject>>[2];
        _playerSquares[0] = new Dictionary<Vector3Int, List<GameObject>>();
        _playerSquares[1] = new Dictionary<Vector3Int, List<GameObject>>();

        // DŮLEŽITÉ: Při změně tahu schováme čtverečky jednoho hráče a ukážeme druhého
        _turnManager.OnTurnChanged += (prev, curr) => {
            _currentPlacedCount = 0;
            UpdateVisuals(curr);
        };
    }

    public void OnTileInteract(Vector3Int cellPos, bool isAdding) {
        int player = _turnManager.CurrentPlayer;

        if (!_playerSquares[player].ContainsKey(cellPos))
            _playerSquares[player][cellPos] = new List<GameObject>();

        var list = _playerSquares[player][cellPos];

        if (IsTileResolved(list)) return;

        if (isAdding) {
            if (_currentPlacedCount >= _maxSquaresPerTurn) return;
            AddSquare(cellPos, list);
            _currentPlacedCount++;
        }
        else {
            if (list.Count == 0) return;
            RemoveSquare(list);
            _currentPlacedCount--;
        }

        Vector3 worldPos = _boardManager.GetActiveTilemap().GetCellCenterWorld(cellPos);
        _uiManager.UpdateTileCounter(player, cellPos, list.Count, worldPos, _boardManager.GridSize);
    }

    private void AddSquare(Vector3Int pos, List<GameObject> list) {
        Vector3 worldPos = _boardManager.GetActiveTilemap().GetCellCenterWorld(pos);
        GameObject sq = Instantiate(_attackSquarePrefab, worldPos, Quaternion.identity, transform); // Spawne čtvereček jako potomka tohoto Manageru
        sq.transform.localScale = Vector3.one * _boardManager.GridScale;

        list.Add(sq);
    }

    private void RemoveSquare(List<GameObject> list) {
        GameObject sq = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        Destroy(sq);
    }

    public void MarkTileAsGuessed(Vector3Int pos, int count) {
        int player = _turnManager.CurrentPlayer;
        var dict = _playerSquares[player];

        // Smazání útočných čtverečků
        if (dict.TryGetValue(pos, out var list)) {
            foreach (var sq in list) Destroy(sq);
            list.Clear();
        }
        else {
            dict[pos] = new List<GameObject>();
            list = dict[pos];
        }

        // Vytvoření "uhodnutých" čtverečků
        Vector3 worldPos = _boardManager.GetActiveTilemap().GetCellCenterWorld(pos);
        for (int i = 0; i < count; i++) {
            GameObject g = Instantiate(_guessedSquarePrefab, worldPos, Quaternion.identity, transform);
            g.transform.localScale = Vector3.one * _boardManager.GridScale;
            list.Add(g);
        }
    }

    // Metoda pro přepínání viditelnosti mezi tahy (aby hráč neviděl útoky soupeře)
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

    public Dictionary<Vector3Int, List<GameObject>> GetActiveSquares() {
        return _playerSquares[_turnManager.CurrentPlayer];
    }

    private bool IsTileResolved(List<GameObject> list) {
        // Kontrola podle Tagu (musíš mít v Unity nastavený Tag "GuessedSquare" u prefabu)
        return list.Count > 0 && list[0].CompareTag("GuessedSquare");
    }
}