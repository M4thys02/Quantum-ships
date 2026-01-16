using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TokenManager : MonoBehaviour {
    [Header("All squares prefabs")]
    [SerializeField] private GameObject _attackSquarePrefab;
    [SerializeField] private GameObject _guessedSquarePrefab;
    [SerializeField] private GameObject _probabilitySquarePrefab;

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
            GameObject g = Instantiate(_guessedSquarePrefab, worldPos, Quaternion.identity, targetMap.transform);
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

    // === NOVÁ METODA ===
    public void RevealLoserBoard(int loserIndex) {
        // 1. Získáme "Pravdu" (řešení) pro prohrávajícího hráče
        // (Pozor: PlayersSetUps.GetKeyValuePairs vrací to, co má hráč "u sebe", 
        // ale my chceme vidět, jak prohrávající hráč tipoval na SOUPEŘE.
        // Takže musíme porovnat attackery (loser) s řešením (winner's setup)).

        int opponent = (loserIndex == 0) ? 1 : 0;
        var trueSolution = PlayersSetUps.GetKeyValuePairs(opponent);
        var playerGuesses = _playerSquares[loserIndex];

        HashSet<Vector3Int> allPositions = new HashSet<Vector3Int>(); // Množina všech pozic, které musíme řešit (sjednocení tipů a reality)

        foreach (var kvp in playerGuesses) allPositions.Add(kvp.Key);
        foreach (var kvp in trueSolution) allPositions.Add(new Vector3Int(kvp.Key.x, kvp.Key.y, 0));

        Tilemap targetMap = _boardManager.GetActiveTilemap();

        foreach (Vector3Int pos in allPositions) {
            Vector2Int pos2D = new Vector2Int(pos.x, pos.y);

            // Kolik jich tam ve skutečnosti mělo být
            int actualCount = trueSolution.ContainsKey(pos2D) ? trueSolution[pos2D] : 0;

            // Kolik jich hráč tipoval
            int guessedCount = 0;
            if (playerGuesses.TryGetValue(pos, out var list)) {
                guessedCount = list.Count;
            }

            // A) Hráč netipoval nic (0), ale mělo tam něco být (>0) -> Modré čtverečky
            if (guessedCount == 0 && actualCount > 0) {
                if (!_playerSquares[loserIndex].ContainsKey(pos)) {
                    _playerSquares[loserIndex][pos] = new List<GameObject>();
                }

                // Správně určíme pozici na tilemapě prohrávajícího (toho, kdo útočil)
                // BoardManager ukazuje obě, musíme najít world pozici
                // Tady trochu hack: spoléháme, že UI Manager si s tím poradí, 
                // jen potřebujeme worldPos pro instanciaci

                // Zjistíme, která tilemapa patří "protivníkovi" (protože na tu loser útočil)
                // Pokud loser je 0, útočil na tilemapu 1.
                Tilemap attackMap = (loserIndex == 0) ? _boardManager.Player1TilemapRef : _boardManager.Player0TilemapRef;

                // Prozatím použijeme active (protože v GameFinished jsou obě active)
                // Ale musíme trefit tu, na které jsou červené čtverečky losera.
                // Logika hry: Hráč 0 sype červené čtverce na Tilemapu 1 (aby viděl, kam střílí).
                Vector3 worldPos = attackMap.GetCellCenterWorld(pos);

                for (int i = 0; i < actualCount; i++) {
                    GameObject blueSq = Instantiate(_probabilitySquarePrefab, worldPos, Quaternion.identity, attackMap.transform);
                    blueSq.transform.localScale = Vector3.one;
                    _playerSquares[loserIndex][pos].Add(blueSq);
                }
            }

            // B) Aktualizace Textu (UI)
            Tilemap correctMap = (loserIndex == 0) ? _boardManager.Player1TilemapRef : _boardManager.Player0TilemapRef;
            Vector3 wPos = correctMap.GetCellCenterWorld(pos);

            _uiManager.UpdateTileCounterEndGame(loserIndex, pos, guessedCount, actualCount, wPos, _boardManager.GridSize, correctMap);
        }
    }
}