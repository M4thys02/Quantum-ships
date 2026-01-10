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
    [SerializeField] private TMP_Text _player0MeasureText; // Textové pole pro seznam měření P0
    [SerializeField] private TMP_Text _player1MeasureText; // Textové pole pro seznam měření P1

    [Header("Game Info")]
    [SerializeField] private TMP_Text _probabilityText;

    [Header("Controls")]
    [SerializeField] private Button _attackButton;
    [SerializeField] private Button _measureButton;
    [SerializeField] private Button _nextPlayerButton;

    // Eventy, na které se napojí GameManager
    public event Action OnAttackClicked;
    public event Action OnMeasureClicked;
    public event Action OnNextTurnClicked;

    private void Awake() {
        // Automatické napojení tlačítek na C# eventy
        _attackButton.onClick.AddListener(() => OnAttackClicked?.Invoke());
        _measureButton.onClick.AddListener(() => OnMeasureClicked?.Invoke());
        _nextPlayerButton.onClick.AddListener(() => OnNextTurnClicked?.Invoke());
    }

    /// <summary>
    /// Aktualizuje vizuál podle toho, kdo je na tahu.
    /// </summary>
    public void UpdateTurnUI(int activePlayer) {
        bool isP0 = (activePlayer == 0);

        // Zobrazení nápisu "Player Turn"
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

    /// <summary>
    /// Zapne/Vypne tlačítka akcí (Attack/Measure).
    /// </summary>
    public void SetActionButtonsInteractable(bool interactable) {
        _attackButton.interactable = interactable;
        _measureButton.interactable = interactable;
        // Nebo přes SetActive, pokud je chceš úplně schovat:
        // _attackButton.gameObject.SetActive(interactable);
        // _measureButton.gameObject.SetActive(interactable);
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
}