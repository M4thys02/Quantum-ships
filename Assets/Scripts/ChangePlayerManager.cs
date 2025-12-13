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

    private void Awake() {
        gridSize = (int)PlayerPrefs.GetFloat("GridSlider", defaultGridSize);
        gridScale = (float)defaultHeight / (oneTileSizes * gridSize);
        GenerateTilemap(player0Tilemap);
        GenerateTilemap(player1Tilemap);

        _labelgenerator.setUpLabels(gridSize, gridScale, true); //TODO: fix label generation for gridSize 10 etc.

        UpdateTilemapsVisibility();
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
        player0Tilemap.gameObject.SetActive(currentPlayer == 0);
        player1Tilemap.gameObject.SetActive(currentPlayer == 1);
    }

    public Tilemap GetActiveTilemap() {
        return currentPlayer == 0 ? player0Tilemap : player1Tilemap;
    }
}
