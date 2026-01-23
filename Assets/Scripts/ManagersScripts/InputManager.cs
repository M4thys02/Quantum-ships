using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputManager : MonoBehaviour {
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private BoardManager _boardManager;

    [Header("Hold settings")]
    [SerializeField] private float holdDelay = 0.1f;
    [SerializeField] private float repeatRate = 0.25f;

    public event Action<Vector3Int> OnLeftClick;
    public event Action<Vector3Int> OnRightClick;

    private bool _holding;
    private bool _rightButton;
    private float _timer;
    private Vector3Int _heldCell;

    private void Update() {
        if (UIControlState.IsOpen || AYSScreenControl.IsOpen) {
            return;
        }

        if (Input.GetMouseButtonDown(0))
            StartHold(false);

        if (Input.GetMouseButtonDown(1))
            StartHold(true);

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            StopHold();

        if (_holding)
            UpdateHold();
    }

    private void StartHold(bool rightButton) {
        Tilemap activeMap = _boardManager.GetActiveTilemap();
        if (activeMap == null) return;

        Vector3Int cell = GetCellUnderMouse(activeMap);
        if (!activeMap.HasTile(cell)) return;

        _holding = true;
        _rightButton = rightButton;
        _heldCell = cell;
        _timer = 0f;

        FireEvent(cell);
    }

    private void UpdateHold() {
        Tilemap activeMap = _boardManager.GetActiveTilemap();
        if (activeMap == null) {
            StopHold();
            return;
        }

        Vector3Int currentCell = GetCellUnderMouse(activeMap);

        if (currentCell != _heldCell) {
            StopHold();
            return;
        }

        _timer += Time.deltaTime;

        if (_timer < holdDelay)
            return;

        if (_timer >= holdDelay + repeatRate) {
            _timer = holdDelay;
            FireEvent(_heldCell);
        }
    }

    private void StopHold() {
        _holding = false;
    }

    private void FireEvent(Vector3Int cell) {
        if (_rightButton)
            OnRightClick?.Invoke(cell);
        else
            OnLeftClick?.Invoke(cell);
    }

    private Vector3Int GetCellUnderMouse(Tilemap map) {
        Vector3 worldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        return map.WorldToCell(worldPos);
    }
}
