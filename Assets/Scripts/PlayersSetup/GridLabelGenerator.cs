using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridLabelGenerator : MonoBehaviour {
    [Header("References")]
    public TextMeshPro labelPrefab;
    public Tilemap tilemap;

    [Header("Grid Settings")]
    [SerializeField] private float defaultFontSize = 4f;
    [SerializeField] private int maxRowsBeforeNewOffset = 9;
    [SerializeField] private int maxGridSizeBeforeNewOffset = 5;
    private int gridSize { get; set; }
    private float gridScale { get; set; }
    private bool mainGame { get; set; }

    private static readonly Dictionary<int, float> FontSizeByGrid = new() {
        { 3, 20f },
        { 4, 15f },
        { 5, 13f },
        { 6, 11f },
        { 7, 9f },
        { 8, 8f },
        { 9, 7f },
        { 10, 4.75f }
    };

    private Dictionary<int, float> FontSizeInGame = new() {
        { 3, 5f },
        { 4, 5f },
        { 5, 4.5f },
        { 6, 4f },
        { 7, 3f },
        { 8, 2.5f },
        { 9, 2f },
        { 10, 2f }
    };
    private void CreateLabelForTile(Vector3Int pos, int row, int column) {
        var label = Instantiate(labelPrefab, transform);
        label.text = $"{(char)('A' + column)}{row + 1}";
        Vector3 worldPos = new Vector3(0, 0, 0);

        if (!mainGame) {
            label.fontSize = FontSizeByGrid.TryGetValue(gridSize, out float sizeValue) ? sizeValue : defaultFontSize;
            worldPos = tilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0) * gridScale;
        }
        else {
            label.fontSize = FontSizeInGame.TryGetValue(gridSize, out float sizeValue) ? sizeValue : defaultFontSize;
            float x_offset = 0f;
            if (row > maxRowsBeforeNewOffset - 1 || gridSize >= maxGridSizeBeforeNewOffset) {
                x_offset = 0.3f;
            }
            else {
                x_offset = 0.2f;
            }
            worldPos = tilemap.CellToWorld(pos) + new Vector3(x_offset, 0.85f, 0) * gridScale;
        }

        label.transform.position = worldPos;
    }
    private void GenerateLabels() {
        int row = 0;
        int column = 0;
        BoundsInt bounds = tilemap.cellBounds;
        foreach (var pos in bounds.allPositionsWithin) {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            CreateLabelForTile(pos, row, column);

            if (column != 0 && column % (gridSize - 1) == 0) {
                ++row;
                column = 0;
            }
            else {
                ++column;
            }
        }
    }

    public void setUpLabels(int currentSize, float currentGridScale, bool isGame) {
        gridSize = currentSize;
        gridScale = currentGridScale;
        mainGame = isGame;

        GenerateLabels();
    }
}
