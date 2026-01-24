using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class ProbabilitySquaresManager : MonoBehaviour {
    [SerializeField] private GameObject _probabilitySquare;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TextMeshPro squareLabelPrefab;
    [SerializeField] private float defaultFontSize = 4f;
    [SerializeField] private int defaultProbSquaresCount = 10;
    [SerializeField] private int maxToMove = 10;

    private TextMeshPro _activeLabel;
    public int RemainingSquares { get; set; }
    private float scale { get; set; }

    private void spawnProbSquares(int count, float gridScale) {
        for (int i = 0; i < count; i++) {
            var squareObj = Instantiate(_probabilitySquare, transform);
            var drag = squareObj.GetComponent<DragAndDrop>();

            drag.Initialize(tilemap, gridScale);
            drag.OnPlaced += HandleSquarePlaced;
            drag.OnReturned += HandleSquareReturned;
            drag.OnMassPlaced += HandleMassPlaced;
        }
    }

    private void spawnLabel() {
        _activeLabel = Instantiate(squareLabelPrefab, transform);
        _activeLabel.GetComponent<MeshRenderer>().sortingOrder = RemainingSquares + 1;
        _activeLabel.fontSize = defaultFontSize;
        _activeLabel.text = GetLabelText();
    }

    public void setUpProbSquares(int newProbabilityCount, int gridSize, float gridScale) {
        scale = gridScale;
        RemainingSquares = Mathf.Max(newProbabilityCount, defaultProbSquaresCount);
        spawnProbSquares(RemainingSquares, gridScale);
        spawnLabel();
    }

    private void HandleSquarePlaced(Vector3Int placedTile) {
        RemainingSquares = Mathf.Max(RemainingSquares - 1, 0);
        UpdateLabel();
        PlayersSetUps.AddSquare(new Vector2Int(placedTile.x, placedTile.y));
    }

    private void HandleSquareReturned(Vector3Int placedTile) {
        RemainingSquares++;
        UpdateLabel();
        PlayersSetUps.RemoveSquare(new Vector2Int(placedTile.x, placedTile.y));
    }

    private void HandleMassPlaced(Vector3Int targetTile) {
        Vector3 targetWorldPos = tilemap.GetCellCenterWorld(targetTile);
        int movedCount = 0;

        foreach (Transform child in transform) {
            if (child.CompareTag("TextLabel")) continue;

            var drag = child.GetComponent<DragAndDrop>();

            // If square exists, square is not placed and limit was not reached
            if (drag != null && !drag.IsPlaced && movedCount < (maxToMove - 1)) {
                drag.SetCurrentTile(targetTile);
                drag.transform.position = targetWorldPos;
                drag.SetPlacedState(true);
                drag.SetScaleInstant(scale);

                movedCount++;
            }
        }
    }

    private void UpdateLabel() {
        if (_activeLabel != null)
            _activeLabel.text = GetLabelText();
    }

    private string GetLabelText() {
        return $"Remaining\nsquares:\n{RemainingSquares}";
    }

    public void DistributeProbabilitySquares() {
        foreach (Transform child in transform) {
            if (child.tag != "TextLabel") {
                GameObject square = child.gameObject;
                Vector3 randomPosition = GetRandomTileCenter();
                Vector3Int cellPos = tilemap.WorldToCell(randomPosition);
                square.transform.position = randomPosition;

                var drag = square.GetComponent<DragAndDrop>();
                if (drag != null) {
                    // We need to tell the logic first: "This square is NOT placed"!
                    drag.SetPlacedState(false);
                    // ------------------
                    drag.SetCurrentTile(cellPos);
                    drag.SetPlacedState(true);
                    drag.SetScaleInstant(scale);
                }
            }
        }

        RemainingSquares = 0;
        UpdateLabel();
    }

    Vector3 GetRandomTileCenter() {
        BoundsInt bounds = tilemap.cellBounds;

        var validCells = new System.Collections.Generic.List<Vector3Int>();
        foreach (var pos in bounds.allPositionsWithin) {
            if (tilemap.HasTile(pos))
                validCells.Add(pos);
        }

        if (validCells.Count == 0)
            return Vector3.zero;

        int index = Random.Range(0, validCells.Count);
        Vector3Int randomCell = validCells[index];
        return tilemap.GetCellCenterWorld(randomCell);
    }
}
