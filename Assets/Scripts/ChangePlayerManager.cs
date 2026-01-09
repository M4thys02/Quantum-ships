using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class ChangePlayerManager : MonoBehaviour {
    public Tilemap player0Tilemap;
    public Tilemap player1Tilemap;
    [SerializeField] private TileBase tileBase;
    [SerializeField] TMP_Text player0Turn;
    [SerializeField] TMP_Text player1Turn;

    private int defaultGridSize = 3;
    private int oneTileSizes = 64;
    private float gridScale;
    [SerializeField] private int gridSize;
    [SerializeField] private int defaultHeight;
    [SerializeField] private GridLabelGenerator _labelgenerator;

    private int currentPlayer = 0;
    public int CurrentPlayer => currentPlayer;

    //PLAYER ATTACK & GUESS:
    [Header("Player attacking and guessing")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject attackSquarePrefab;
    [SerializeField] private GameObject guessedSquarePrefab;
    [SerializeField] private Transform attackSqrtRoot;
    [SerializeField] private Transform guessedSqrtRoot;
    [SerializeField] private int defaultSquares = 10;
    [SerializeField] private int maxSquares;
    [SerializeField] private int currentAttackSquares;
    [SerializeField] private TextMeshPro tileCounterPrefab;
    [SerializeField] private Transform countersRoot;

    private Dictionary<Vector3Int, List<GameObject>> player0Squares = new Dictionary<Vector3Int, List<GameObject>>();
    private Dictionary<Vector3Int, List<GameObject>> player1Squares = new Dictionary<Vector3Int, List<GameObject>>();

    private Dictionary<Vector3Int, TMP_Text> player0Counters = new();
    private Dictionary<Vector3Int, TMP_Text> player1Counters = new();

    private void Awake() {
        gridSize = (int)PlayerPrefs.GetFloat("GridSlider", defaultGridSize);
        gridScale = (float)defaultHeight / (oneTileSizes * gridSize);
        GenerateTilemap(player0Tilemap);
        GenerateTilemap(player1Tilemap);

        _labelgenerator.setUpLabels(gridSize, gridScale, true);

        player1Turn.gameObject.SetActive(false);
        UpdateTilemapsVisibility();

        maxSquares = (int)PlayerPrefs.GetFloat("SquareSlider", defaultSquares);
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            HandleClick(add: true);
        }
        else if (Input.GetMouseButtonDown(0)) {
            HandleClick(add: false);
        }
    }

    private void HandleClick(bool add) {
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        TryHandleTilemap(GetActiveTilemap(), GetActiveSquaresDict(), worldPos, add);
    }

    private void TryHandleTilemap(Tilemap tilemap, Dictionary<Vector3Int, List<GameObject>> squaresDict, Vector3 worldPos, bool add) {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);

        // Ignore clicks outside the tilemap
        if (!tilemap.HasTile(cellPos))
            return;

        if (IsTileAlreadyGuessed(squaresDict, cellPos))
            return;

        if (!squaresDict.TryGetValue(cellPos, out var list)) {
            list = new List<GameObject>();
            squaresDict[cellPos] = list;
        }

        if (add) {
            if (CountAllSquares(GetActiveSquaresDict()) >= maxSquares)
                return;

            Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos);
            GameObject square = Instantiate(attackSquarePrefab, spawnPos, Quaternion.identity, attackSqrtRoot);
            square.transform.localScale = Vector3.one * gridScale;
            list.Add(square);
            currentAttackSquares++;
            UpdateTileCounter(cellPos, list.Count, tilemap, GetActiveCountersDict());
        }
        else {
            if (list.Count == 0)
                return;

            GameObject square = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            Destroy(square);
            currentAttackSquares--;
            UpdateTileCounter(cellPos, list.Count, tilemap, GetActiveCountersDict());
        }
    }

    private void UpdateTileCounter(Vector3Int cellPos, int count, Tilemap tilemap,  Dictionary<Vector3Int, TMP_Text> countersDict) {
        if (count <= 0) {
            if (countersDict.TryGetValue(cellPos, out var oldText)) {
                Destroy(oldText.gameObject);
                countersDict.Remove(cellPos);
            }
            return;
        }

        if (!countersDict.TryGetValue(cellPos, out var text)) {
            Vector3 pos = tilemap.GetCellCenterWorld(cellPos);

            text = Instantiate(tileCounterPrefab, countersRoot);
            text.transform.position = pos;
            countersDict[cellPos] = text;
        }

        float scale = (gridSize >= 6) ? 0.25f : 0.5f;
        if (count >= 100) scale *= 0.5f;
        text.transform.localScale = Vector3.one * scale;

        text.text = count.ToString();
    }

    private void GenerateTilemap(Tilemap currentTilemap) {
        currentTilemap.ClearAllTiles();
        for (int x = 0; x < gridSize; ++x) {
            for (int y = 0; y < gridSize; ++y) {
                Vector3Int position = new Vector3Int(x, y, 0);
                currentTilemap.SetTile(position, tileBase);
            }
        }
        currentTilemap.transform.localScale = Vector3.one * gridScale;
    }

    public void ChangePlayer() {
        currentPlayer = (currentPlayer == 0) ? 1 : 0;
        currentAttackSquares = 0;

        player0Turn.gameObject.SetActive(currentPlayer == 0);
        player1Turn.gameObject.SetActive(currentPlayer == 1);

        UpdateTilemapsVisibility();
    }

    public void UpdateTilemapsVisibility() {
        bool isPlayer0 = currentPlayer == 0;

        player0Tilemap.gameObject.SetActive(!isPlayer0);
        player1Tilemap.gameObject.SetActive(isPlayer0);

        SetSquaresVisibility(player0Squares, isPlayer0);
        SetSquaresVisibility(player1Squares, !isPlayer0);

        SetCountersVisibility(player0Counters, isPlayer0);
        SetCountersVisibility(player1Counters, !isPlayer0);
    }

    private void SetSquaresVisibility(Dictionary<Vector3Int, List<GameObject>> dict, bool visible) {
        foreach (var kv in dict) {
            foreach (var square in kv.Value) {
                if (square != null)
                    square.SetActive(visible);
            }
        }
    }

    private void SetCountersVisibility(Dictionary<Vector3Int, TMP_Text> dict, bool visible) {
        foreach (var kv in dict) {
            if (kv.Value != null)
                kv.Value.gameObject.SetActive(visible);
        }
    }

    public Tilemap GetActiveTilemap() {
        return currentPlayer == 0 ? player0Tilemap : player1Tilemap;
    }

    public Dictionary<Vector3Int, List<GameObject>> GetActiveSquaresDict() {
        return currentPlayer == 0 ? player0Squares : player1Squares;
    }

    public int GetActivatePlayer() {
        return currentPlayer;
    }

    public int GetOpponent() {
        return currentPlayer ^ 1;
    }
    private Dictionary<Vector3Int, TMP_Text> GetActiveCountersDict() {
        return currentPlayer == 0 ? player0Counters : player1Counters;
    }
    public void CreateGuessedSquares(Vector3Int tilePos3D, int amount) { //TODO: not only 10 squares in one tile, but 10 squares only in GLOBAL
        Dictionary<Vector3Int, List<GameObject>> playerSquares = GetActiveSquaresDict();

        // remove old attack squares
        if (playerSquares.TryGetValue(tilePos3D, out var list)) {
            foreach (var square in list)
                Destroy(square);

            list.Clear();
        }
        else {
            playerSquares[tilePos3D] = new List<GameObject>();
            list = playerSquares[tilePos3D];
        }

        Vector3 spawnPos = GetActiveTilemap().GetCellCenterWorld(tilePos3D);

        // create guessed squares INSTEAD of attack squares
        for (int i = 0; i < amount; i++) {
            GameObject guessed = Instantiate(guessedSquarePrefab, spawnPos, Quaternion.identity, guessedSqrtRoot);
            guessed.transform.localScale = Vector3.one * gridScale;
            list.Add(guessed);
        }

        UpdateTileCounter(tilePos3D, amount, GetActiveTilemap(), GetActiveCountersDict());
    }

    bool IsTileAlreadyGuessed(Dictionary<Vector3Int, List<GameObject>> squares, Vector3Int tile) {
        return squares.TryGetValue(tile, out var list) && list.Count > 0 && list[0].CompareTag("GuessedSquare");
    }

    private int CountAllSquares(Dictionary<Vector3Int, List<GameObject>> squaresDict) {
        int total = 0;

        foreach (var kv in squaresDict) {
            foreach (var square in kv.Value) {
                if (square != null && (square.CompareTag("GuessedSquare") || square.CompareTag("AttackSquare"))) {
                    total++;
                }
            }
        }

        return total;
    }
}
