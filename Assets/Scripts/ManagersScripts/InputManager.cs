using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour {
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private BoardManager _boardManager;

    public event Action<Vector3Int> OnLeftClick;
    public event Action<Vector3Int> OnRightClick;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) HandleClick(OnLeftClick);
        if (Input.GetMouseButtonDown(1)) HandleClick(OnRightClick);
    }

    private void HandleClick(Action<Vector3Int> clickEvent) {
        Tilemap activeMap = _boardManager.GetActiveTilemap();
        if (activeMap == null) return;

        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        Vector3Int cellPos = activeMap.WorldToCell(worldPos);

        if (activeMap.HasTile(cellPos)) {
            clickEvent?.Invoke(cellPos);
        }
    }
}