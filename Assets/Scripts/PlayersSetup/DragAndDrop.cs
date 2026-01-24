using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]
public class DragAndDrop : MonoBehaviour {
    [Header("Scaling")]
    [SerializeField] private float pickupScale = 3f;      // size in storage
    [SerializeField] private float placedScale = 1f;      // size when placed
    [SerializeField] private float draggingScale = 1.2f;  // size while dragging
    [SerializeField] private float scaleLerpSpeed = 8f;

    private Transform followTarget;
    private List<DragAndDrop> followers = new List<DragAndDrop>();
    private Tilemap tilemap;
    private bool isDragging;
    private bool isPlaced;
    public bool IsPlaced => isPlaced;
    private Vector3 originalPosition;
    private float targetScale;

    public event Action<Vector3Int> OnPlaced;
    public event Action<Vector3Int> OnReturned;
    public event Action<DragAndDrop, Vector3Int> OnShiftDragStarted;
    public Vector3Int currentTile { get; private set; }

    public void Initialize(Tilemap map, float gridScale) {
        tilemap = map;
        placedScale = gridScale;
        targetScale = pickupScale;
        originalPosition = transform.position;
        currentTile = new Vector3Int(-1, -1, -1);
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

    public void StartFollowing(Transform target) {
        followTarget = target;
        isDragging = true;
        SetPlacedState(false);
        SetScaleInstant(draggingScale);
    }

    public void AddFollower(DragAndDrop follower) {
        followers.Add(follower);
    }

    public void DropAt(Vector3 position) {
        isDragging = false;
        followTarget = null;
        PlaceObject(position);
    }

    private void PlaceObject(Vector3 worldPos) {
        Vector3Int cellPos = tilemap.WorldToCell(worldPos);
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
    }

    private void OnMouseDown() {
        if (UIControlState.IsOpen) return;

        Vector3Int startTile = currentTile;

        SetPlacedState(false);
        isDragging = true;
        targetScale = draggingScale;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            OnShiftDragStarted?.Invoke(this, startTile); // Sends info to manager, we take from THIS tile
        }
    }

    private void OnMouseUp() {
        if (followers.Count > 0) {
            foreach (var f in followers) {
                f.DropAt(transform.position);
            }
            followers.Clear();
        }
        DropAt(transform.position);
        //Debug.Log($"Current tile is: {currentTile}");
    }

    private void Update() {
        if (isDragging) {
            if (followTarget != null) { // Follows leader square if is follower
                transform.position = followTarget.position;
            }
            else {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorld.z = 0;
                transform.position = mouseWorld;
            }
        }

        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleLerpSpeed);
    }
}

