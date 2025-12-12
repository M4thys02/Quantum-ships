using System.Collections.Generic;
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
    private int gridSize { get; set; }
    private float gridScale { get; set; }

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

    private float GetFontSizeForGrid(int size) {
        return FontSizeByGrid.TryGetValue(size, out float sizeValue) ? sizeValue : defaultFontSize;
    }
    private void CreateLabelForTile(Vector3Int pos, int row, int column) {
        var label = Instantiate(labelPrefab, transform);
        label.text = $"{(char)('A' + column)}{row + 1}";
        label.fontSize = GetFontSizeForGrid(gridSize);

        Vector3 worldPos = tilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0) * gridScale;
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

    public void setUpLabels(int currentSize, float currentGridScale) {
        gridSize = currentSize;
        gridScale = currentGridScale;

        GenerateLabels();
    }
}
