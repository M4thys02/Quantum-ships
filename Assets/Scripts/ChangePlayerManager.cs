using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChangePlayerManager : MonoBehaviour {
    public Tilemap player0Tilemap;
    public Tilemap player1Tilemap;
    [SerializeField] private TileBase tileBase;

    private int defaultGridSize = 3;
    [SerializeField] private int gridSize;
    [SerializeField] private int defaultHeight;
    private int oneTileSizes = 64;
    private float gridScale;
    [SerializeField] private GridLabelGenerator _labelgenerator;

    private int currentPlayer = 0;
    public int CurrentPlayer => currentPlayer;

    // FOR PLAYER ATTACK:
    [SerializeField] private GameObject attackSquarePrefab;
    [SerializeField] private GameObject guessedSquarePrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int defaultSquares = 10;
    [SerializeField] private int maxAttackSquares;

    private Dictionary<Vector3Int, List<GameObject>> player0Squares = new Dictionary<Vector3Int, List<GameObject>>();
    private Dictionary<Vector3Int, List<GameObject>> player1Squares = new Dictionary<Vector3Int, List<GameObject>>();


    private void Awake() {
        gridSize = (int)PlayerPrefs.GetFloat("GridSlider", defaultGridSize);
        gridScale = (float)defaultHeight / (oneTileSizes * gridSize);
        GenerateTilemap(player0Tilemap);
        GenerateTilemap(player1Tilemap);

        _labelgenerator.setUpLabels(gridSize, gridScale, true); //TODO: fix label generation for gridSize 10 etc.

        UpdateTilemapsVisibility();

        maxAttackSquares = (int)PlayerPrefs.GetFloat("SquareSlider", defaultSquares);
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

        if (!squaresDict.TryGetValue(cellPos, out var list)) {
            list = new List<GameObject>();
            squaresDict[cellPos] = list;
        }

        if (add) {
            if (list.Count >= maxAttackSquares)
                return;

            Vector3 spawnPos = tilemap.GetCellCenterWorld(cellPos);
            GameObject square = Instantiate(attackSquarePrefab, spawnPos, Quaternion.identity);
            square.transform.localScale = Vector3.one * gridScale;
            list.Add(square);
        }
        else {
            if (list.Count == 0)
                return;

            GameObject square = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            Destroy(square);
        }
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
        UpdateTilemapsVisibility();
    }

    public void UpdateTilemapsVisibility() {
        bool isPlayer0 = currentPlayer == 0;

        player0Tilemap.gameObject.SetActive(isPlayer0);
        player1Tilemap.gameObject.SetActive(!isPlayer0);

        foreach (var kv in player0Squares) {
            foreach (var square in kv.Value) {
                square.SetActive(isPlayer0);
            }
        }

        foreach (var kv in player1Squares) {
            foreach (var square in kv.Value) {
                square.SetActive(!isPlayer0);
            }
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

    public void CreateGuessedSquares(Vector3Int tilePos3D, int expectedCount) { //TODO: clicking on guessed tile doesn't add red attack square
        Dictionary<Vector3Int, List<GameObject>>  playerSquares = GetActiveSquaresDict();
        if (playerSquares.ContainsKey(tilePos3D)) {
            foreach (var square in playerSquares[tilePos3D]) {
                Destroy(square);
            }
            playerSquares[tilePos3D].Clear();
        }

        for (int i = 0; i < expectedCount; i++) {
            Vector3 spawnPos = playerSquares.ContainsKey(tilePos3D) && playerSquares[tilePos3D].Count > 0
                ? playerSquares[tilePos3D][0].transform.position
                : GetActiveTilemap().GetCellCenterWorld(tilePos3D);

            GameObject guessed = Instantiate(guessedSquarePrefab, spawnPos, Quaternion.identity);
            guessed.transform.localScale = Vector3.one * gridScale;
        }
    }
}
