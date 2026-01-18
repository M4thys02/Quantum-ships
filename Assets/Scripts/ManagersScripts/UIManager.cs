using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Turn Indicators")]
    [SerializeField] private GameObject _gameTitle;
    [SerializeField] private TMP_Text _player0TurnText;
    [SerializeField] private TMP_Text _player1TurnText;

    [Header("Measurement Panels")]
    [SerializeField] private GameObject _measurementBoard;
    [SerializeField] private GameObject _player0Measures;
    [SerializeField] private GameObject _player1Measures;
    [SerializeField] private TMP_Text _player0MeasureText;
    [SerializeField] private TMP_Text _player1MeasureText;

    [Header("Game Info")]
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private TMP_Text _probabilityText;
    [SerializeField] private TMP_Text _whichPlayerWinText;

    [Header("Controls")]
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _measureButton;
    [SerializeField] private Button _nextPlayerButton;

    [Header("Font Settings")]
    [SerializeField] private float _defaultFontSize = 20f;
    private float _currentFontSize;
    // Slovník pro mapování: GridSize -> FontSize
    private readonly Dictionary<int, float> FontSizeByGrid = new Dictionary<int, float>() {
        { 3, 24f },
        { 4, 23f },
        { 5, 22f },
        { 6, 21f },
        { 7, 20f },
        { 8, 20f },
        { 9, 20f },
        { 10, 18f }
    };

    [Header("World Text Settings")]
    [SerializeField] private TileCounter _tileCounterPrefab;
    private Dictionary<Vector3Int, TileCounter>[] _playerCounters = {
        new Dictionary<Vector3Int, TileCounter>(),
        new Dictionary<Vector3Int, TileCounter>()
    };

    // Events for GameManager
    public event Action OnAttackClicked;
    public event Action OnMeasureClicked;
    public event Action OnNextTurnClicked;

    private void Awake() {
        _attackButton.onClick.AddListener(() => OnAttackClicked?.Invoke());
        _measureButton.onClick.AddListener(() => OnMeasureClicked?.Invoke());
        _nextPlayerButton.onClick.AddListener(() => OnNextTurnClicked?.Invoke());

        _currentFontSize = _defaultFontSize;
        InitializeUI((int)PlayerPrefs.GetFloat("GridSlider", 3));
    }

    public void InitializeUI(int gridSize) {
        if (!FontSizeByGrid.TryGetValue(gridSize, out _currentFontSize)) {
            _currentFontSize = _defaultFontSize;
        }

        if (_player0Measures != null) {
            TMP_Text[] p0Texts = _player0Measures.GetComponentsInChildren<TMP_Text>(true);
            foreach (var txt in p0Texts) {
                txt.fontSize = _currentFontSize;
            }
        }

        if (_player1Measures != null) {
            TMP_Text[] p1Texts = _player1Measures.GetComponentsInChildren<TMP_Text>(true);
            foreach (var txt in p1Texts) {
                txt.fontSize = _currentFontSize;
            }
        }
    }

    // Update UIText which player is on turn
    public void UpdateTurnUI(int activePlayer) {
        bool isP0 = (activePlayer == 0);

        _player0TurnText?.gameObject.SetActive(isP0);
        _player1TurnText?.gameObject.SetActive(!isP0);

        _player0Measures?.gameObject.SetActive(isP0);
        _player1Measures?.gameObject.SetActive(!isP0);
    }

    // Shows probability of 1 square
    public void UpdateProbability(float percentage) {
        if (_probabilityText != null) _probabilityText.text = $"= {percentage:F2} %";
    }

    // Gets dictionary with measurement data, formates them, and then write them in UI
    public void UpdateMeasurementList(int playerIndex, Dictionary<Vector2Int, int> measurements) {
        TMP_Text targetText = (playerIndex == 0) ? _player0MeasureText : _player1MeasureText;
        if (targetText == null) return;

        // Font se nastavuje v InitializeUI, ale pro jistotu ho můžeme držet i zde
        targetText.fontSize = _currentFontSize;
        targetText.text = BuildMeasurementString(measurements);
    }


    // Turns OFF/ON buttons (Attack & Measure)
    public void SetActionButtonsInteractable(bool interactable) {
        _attackButton.interactable = interactable;
        _measureButton.interactable = interactable;
    }

    public void ToggleNextTurnButton(bool active) {
        _nextPlayerButton.interactable = active;
    }

    // Methods for counters numbers
    public void UpdateTileCounter(int playerIndex, Vector3Int cellPos, int count, Vector3 worldPos, int gridSize, Tilemap targetMap) {
        var counters = _playerCounters[playerIndex];

        if (count <= 0) {
            if (counters.TryGetValue(cellPos, out var existing)) {
                Destroy(existing.gameObject);
                counters.Remove(cellPos);
            }
            return;
        }

        if (!counters.TryGetValue(cellPos, out var counterScript)) {
            counterScript = Instantiate(_tileCounterPrefab, worldPos, Quaternion.identity, targetMap.transform); // ← parent = tilemapa
            counters[cellPos] = counterScript;
        }

        counterScript.UpdateVisuals(count, gridSize);
    }

    public void ToggleCountersVisibility(int activePlayer) {
        for (int i = 0; i < 2; i++) {
            bool isVisible = (i == activePlayer);
            foreach (var counter in _playerCounters[i].Values) {
                if (counter != null) counter.gameObject.SetActive(isVisible);
            }
        }
    }

    private void ToggleBothCountersVisibility(bool isVisible) {
        for (int i = 0; i < 2; i++) {
            foreach (var counter in _playerCounters[i].Values) {
                if (counter != null) counter.gameObject.SetActive(isVisible);
            }
        }
    }

    public void PlayerWinGame(int winPlayer) {
        ToggleNextTurnButton(false);
        _gameTitle.SetActive(false);
        _measurementBoard.SetActive(false);
        ShowWhoWin(winPlayer);
        _winScreen.SetActive(true);
        ToggleBothCountersVisibility(true);
    }

    public void UpdateTileCounterEndGame(int playerIndex, Vector3Int cellPos, int guessed, int actual, Vector3 worldPos, int gridSize, Tilemap targetMap) {
        if (guessed > 0 && guessed == actual) return;

        var counters = _playerCounters[playerIndex];

        if (guessed == 0 && actual == 0) return;

        if (!counters.TryGetValue(cellPos, out var counterScript)) {
            counterScript = Instantiate(_tileCounterPrefab, worldPos, Quaternion.identity, targetMap.transform);
            counters[cellPos] = counterScript;
        }

        counterScript.SetText(guessed, actual, gridSize);
    }

    // --- Helping methods for text formating ---
    private string BuildMeasurementString(Dictionary<Vector2Int, int> dict) {
        StringBuilder sb = new StringBuilder();

        foreach (var kvp in dict) {
            string label = TileToLabel(kvp.Key);
            sb.AppendLine($"{label} - {kvp.Value}");
        }

        return sb.ToString();
    }

    private string TileToLabel(Vector2Int tile) {
        // Assuming: tile.x = 0 -> 'A', tile.y = 0 -> '1'
        char column = (char)('A' + tile.x);
        int row = tile.y + 1;
        return $"{column}{row}";
    }

    private void ShowWhoWin(int player) {
        string roman = player == 0 ? "I" : "II";
        _whichPlayerWinText.text = $"Player {roman} won";
    }
}