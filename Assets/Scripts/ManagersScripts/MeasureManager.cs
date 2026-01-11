using System.Collections.Generic;
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

    public Dictionary<Vector2Int, int> GetPlayerMeasurements(int player) {
        return Measurements[player];
    }
}
