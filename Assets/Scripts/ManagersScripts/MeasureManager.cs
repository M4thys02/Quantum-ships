using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeasureManager : MonoBehaviour {

    // [player][tile] → count of measurements
    public Dictionary<Vector2Int, int>[] Measurements = {
        new Dictionary<Vector2Int, int>(),
        new Dictionary<Vector2Int, int>()
    };

    public void AddMeasurement(Vector2Int tile, int player) {
        if (!Measurements[player].ContainsKey(tile))
            Measurements[player][tile] = 0;

        Measurements[player][tile]++;
    }

    // Alphabetical order - (A1, A2, B1, B2, ...)
    public Dictionary<Vector2Int, int> GetPlayerMeasurements(int player) {
        return Measurements[player]
        .OrderBy(kv => kv.Key.x)   // A, B, C...
        .ThenBy(kv => kv.Key.y)    // 1, 2, 3...
        .ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}
