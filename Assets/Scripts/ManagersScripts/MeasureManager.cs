using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeasureManager : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _player0Measurements;
    [SerializeField] private TextMeshProUGUI _player1Measurements;

    private Dictionary<Vector2Int, int> player0Measurements = new();
    private Dictionary<Vector2Int, int> player1Measurements = new();

    public void CurrentMeasurement(Vector2Int measuredTile, int player) {
        var dict = (player == 0) ? player0Measurements : player1Measurements;
        var text = (player == 0) ? _player0Measurements : _player1Measurements;

        if (!dict.ContainsKey(measuredTile))
            dict[measuredTile] = 0;

        dict[measuredTile]++;

        text.text = BuildMeasurementText(dict);
    }

    private string TileToLabel(Vector2Int tile) {
        char column = (char)('A' + tile.x);
        int row = tile.y + 1;

        return $"{column}{row}";
    }

    private string BuildMeasurementText(Dictionary<Vector2Int, int> dict) {
        System.Text.StringBuilder sb = new();

        foreach (var kvp in dict) {
            string label = TileToLabel(kvp.Key);
            sb.AppendLine($"{label} - {kvp.Value}");
        }

        return sb.ToString();
    }

    public void UpdateMeasurementsVisibility(int player) {
        _player0Measurements.gameObject.SetActive(player == 0);
        _player1Measurements.gameObject.SetActive(player == 1);
    }
}
