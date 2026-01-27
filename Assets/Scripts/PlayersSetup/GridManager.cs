using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using System.Collections.Generic;

[System.Serializable]
public struct TileSet {
    public TileBase center;

    [Header("Corners")]
    public TileBase upRight;
    public TileBase upLeft;
    public TileBase downRight;
    public TileBase downLeft;

    [Header("Edges")]
    public TileBase up;
    public TileBase down;
    public TileBase left;
    public TileBase right;
}

public class GridManager : MonoBehaviour {
    private int _gridResolution;
    public int gridResolution {
        get => _gridResolution;
        set => _gridResolution = value;
    }
    public float currentScale;

    public GridLabelGenerator _labelGenerator;
    [SerializeField] private Tilemap targetTilemap;
    [SerializeField] private TileSet tileSet;

    enum TileType { Center, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight }
    private TileBase GetTileForPosition(Vector3Int cell) {
        bool left = cell.x == 0;
        bool right = cell.x == _gridResolution - 1;
        bool down = cell.y == 0;
        bool up = cell.y == _gridResolution - 1;

        if (up && left) return tileSet.upLeft;
        if (up && right) return tileSet.upRight;
        if (down && left) return tileSet.downLeft;
        if (down && right) return tileSet.downRight;
        if (up) return tileSet.up;
        if (down) return tileSet.down;
        if (left) return tileSet.left;
        if (right) return tileSet.right;
        return tileSet.center;
    }

    private void PlaceTile(Vector3Int cell) {
        targetTilemap.SetTile(cell, GetTileForPosition(cell));
    }

    private void generateGrid() {
        targetTilemap.ClearAllTiles();
        for (int x = 0; x < _gridResolution; ++x) {
            for (int y = 0; y < _gridResolution; ++y) {
                Vector3Int position = new Vector3Int(x, y, 0);
                PlaceTile(position);
            }
        }
        targetTilemap.transform.localScale = Vector3.one * currentScale;
    }

    public void setUpGrid(int newSize, float newScale) {
        gridResolution = newSize;
        currentScale = newScale; //This is much better then dictionary, bcs it is caluclated on-time for current gridResolution

        generateGrid();
        _labelGenerator.setUpLabels(gridResolution, currentScale, false);
    }
}
