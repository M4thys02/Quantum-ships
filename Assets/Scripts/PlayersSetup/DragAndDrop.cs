using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]
public class DragAndDrop : MonoBehaviour {
    [Header("Scaling")]
    [SerializeField] private float pickupScale = 3f;      // size in storage
    [SerializeField] private float placedScale = 1f;      // size when placed
    [SerializeField] private float draggingScale = 1.2f;  // size while dragging
    [SerializeField] private float scaleLerpSpeed = 8f;

    private Tilemap tilemap;
    private bool isDragging;
    private bool isPlaced;
    private Vector3 originalPosition;
    private float targetScale;

    public event Action<Vector3Int> OnPlaced;
    public event Action<Vector3Int> OnReturned;
    public Vector3Int currentTile { get; private set; }

    public void Initialize(Tilemap map, float gridScale) {
        tilemap = map;
        placedScale = gridScale;
        targetScale = pickupScale;
        originalPosition = transform.position;
    }

    public void SetScaleInstant(float s) {
        targetScale = s;
        transform.localScale = Vector3.one * s;
    }

    public void SetCurrentTile(Vector3Int tile) {
        currentTile = tile;
    }

    public void SetPlacedState(bool placed) {
        if (placed == isPlaced) 
            return;

        isPlaced = placed;
        targetScale = placed ? placedScale : pickupScale;

        if (placed) {
            OnPlaced?.Invoke(currentTile);
        }
        else {
            OnReturned?.Invoke(currentTile);
        }
    }

    private void OnMouseDown() {
        SetPlacedState(false);
        isDragging = true;
        targetScale = draggingScale;
    }

    private void OnMouseUp() {
        isDragging = false;

        Vector3Int cellPos = tilemap.WorldToCell(transform.position);
        Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);

        if (tilemap.HasTile(cellPos)) {
            currentTile = cellPos;
            transform.position = cellCenter;
            SetPlacedState(true);
        }
        else {
            currentTile = new Vector3Int(-1, -1, -1);
            transform.position = originalPosition;
            targetScale = pickupScale;
            SetPlacedState(false);
        }

        //Debug.Log($"Current tile is: {currentTile}");
    }

    private void Update() {
        if (isDragging) {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            transform.position = mouseWorld;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleLerpSpeed);
    }
}

