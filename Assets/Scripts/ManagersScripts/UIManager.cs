using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [Header("Turn Indicators")]
    [SerializeField] private TMP_Text _player0TurnText;
    [SerializeField] private TMP_Text _player1TurnText;

    [Header("Measurement Panels")]
    [SerializeField] private TMP_Text _player0MeasureText;
    [SerializeField] private TMP_Text _player1MeasureText;

    [Header("Game Info")]
    [SerializeField] private TMP_Text _probabilityText;

    [Header("Controls")]
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _measureButton;
    [SerializeField] private Button _nextPlayerButton;

    [Header("World Text Settings")]
    [SerializeField] private TMP_Text _tileCounterPrefab;
    private Dictionary<Vector3Int, TMP_Text>[] _playerCounters = { new Dictionary<Vector3Int, TMP_Text>(), new Dictionary<Vector3Int, TMP_Text>() };

    // Events for GameManager
    public event Action OnAttackClicked;
    public event Action OnMeasureClicked;
    public event Action OnNextTurnClicked;

    private void Awake() {
        _attackButton.onClick.AddListener(() => OnAttackClicked?.Invoke());
        _measureButton.onClick.AddListener(() => OnMeasureClicked?.Invoke());
        _nextPlayerButton.onClick.AddListener(() => OnNextTurnClicked?.Invoke());
    }

    /// <summary>
    /// Aktualizuje vizuál podle toho, kdo je na tahu.
    /// </summary>
    public void UpdateTurnUI(int activePlayer) {
        bool isP0 = (activePlayer == 0);

        if (_player0TurnText) _player0TurnText.gameObject.SetActive(isP0);
        if (_player1TurnText) _player1TurnText.gameObject.SetActive(!isP0);

        // Zobrazení panelů s měřením (hráč vidí jen svoje měření?)
        // Pokud chceš, aby hráč viděl svoje měření, nebo měření soupeře, uprav podmínku zde.
        // Dle původního kódu se zobrazuje panel aktivního hráče.
        if (_player0MeasureText) _player0MeasureText.gameObject.SetActive(isP0);
        if (_player1MeasureText) _player1MeasureText.gameObject.SetActive(!isP0);
    }

    /// <summary>
    /// Vypíše pravděpodobnost jednoho čtverečku.
    /// </summary>
    public void UpdateProbability(float percentage) {
        if (_probabilityText != null)
            _probabilityText.text = $"= {percentage:F2} %";
    }

    /// <summary>
    /// Přijme slovník naměřených dat, zformátuje je na text a vypíše do UI.
    /// </summary>
    public void UpdateMeasurementList(int playerIndex, Dictionary<Vector2Int, int> measurements) {
        TMP_Text targetText = (playerIndex == 0) ? _player0MeasureText : _player1MeasureText;

        if (targetText == null) return;

        targetText.text = BuildMeasurementString(measurements);
    }


    // Turns OFF/ON buttons (Attack & Measure)
    public void SetActionButtonsInteractable(bool interactable) {
        _attackButton.interactable = interactable;
        _measureButton.interactable = interactable;
    }

    public void ToggleNextTurnButton(bool active) {
        _nextPlayerButton.gameObject.SetActive(active);
    }

    // --- Pomocné metody pro formátování textu ---
    private string BuildMeasurementString(Dictionary<Vector2Int, int> dict) {
        StringBuilder sb = new StringBuilder();

        foreach (var kvp in dict) {
            string label = TileToLabel(kvp.Key);
            sb.AppendLine($"{label} - {kvp.Value}");
        }

        return sb.ToString();
    }

    private string TileToLabel(Vector2Int tile) {
        // Předpoklad: tile.x = 0 -> 'A', tile.y = 0 -> '1'
        char column = (char)('A' + tile.x);
        int row = tile.y + 1;
        return $"{column}{row}";
    }

    public void UpdateTileCounter(int playerIndex, Vector3Int cellPos, int count, Vector3 worldPos) {
        var counters = _playerCounters[playerIndex];

        if (count <= 0) {
            if (counters.TryGetValue(cellPos, out var existing)) {
                Destroy(existing.gameObject);
                counters.Remove(cellPos);
            }
            return;
        }

        if (!counters.TryGetValue(cellPos, out var text)) {
            text = Instantiate(_tileCounterPrefab, worldPos, Quaternion.identity);
            counters[cellPos] = text;
        }

        text.text = count.ToString();
    }

    public void ToggleCountersVisibility(int activePlayer) {
        for (int i = 0; i < 2; i++) {
            bool isVisible = (i == activePlayer);
            foreach (var text in _playerCounters[i].Values) {
                if (text != null) text.gameObject.SetActive(isVisible);
            }
        }
    }
}