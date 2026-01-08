using System.Collections.Generic;
using UnityEngine;

public class PlayersSetUps : MonoBehaviour {
    private static PlayersSetUps instance;
    private static int currentPlayer { get; set; }
    static Dictionary<Vector2Int, int> player0Tiles = new();
    static Dictionary<Vector2Int, int> player1Tiles = new();

    public static void Cleanup() {
        if (instance != null) {
            Destroy(instance.gameObject); // remove object
            instance = null;              // reset so new one can spawn
        }
        player0Tiles.Clear();
        player1Tiles.Clear();

        ChangePlayer();
    }

    public static void AddSquare(Vector2Int tilePosition) {
        var dict = (currentPlayer == 0) ? player0Tiles : player1Tiles;

        if (!dict.ContainsKey(tilePosition))
            dict[tilePosition] = 0;

        dict[tilePosition]++;
    }

    public static void RemoveSquare(Vector2Int tilePosition) {
        var dict = (currentPlayer == 0) ? player0Tiles : player1Tiles;

        if (!dict.ContainsKey(tilePosition))
            return;

        dict[tilePosition]--;

        if (dict[tilePosition] <= 0)
            dict.Remove(tilePosition);
    }

    public static void ChangePlayer() {
        currentPlayer ^= 1;
    }

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Vector2Int GetWeightedRandomTileForPlayer(int player) {
        Dictionary<Vector2Int, int> dict = (player == 0) ? player0Tiles : player1Tiles;

        if (dict.Count == 0) {
            Debug.LogWarning($"No tiles available for player {player}.");
            return Vector2Int.zero;
        }

        int totalWeight = 0;

        foreach (var kvp in dict) {
            totalWeight += kvp.Value;
        }

        int randomValue = Random.Range(0, totalWeight);

        foreach (var kvp in dict) {
            randomValue -= kvp.Value;

            if (randomValue < 0)
                return kvp.Key;
        }

        // Fallback, logically unreachable
        return Vector2Int.zero;
    }

    public static Dictionary<Vector2Int, int> GetKeyValuePairs(int player) {
        return player == 1 ? player0Tiles : player1Tiles;
    }

    //public static void ShowDictionaries() {
    //    Debug.Log("=== Player 0 Tiles ===");
    //    if (player0Tiles.Count == 0) {
    //        Debug.Log("No tiles for player 0.");
    //    }
    //    else {
    //        foreach (var kvp in player0Tiles) {
    //            Debug.Log($"Position: {kvp.Key}, Count: {kvp.Value}");
    //        }
    //    }

    //    Debug.Log("=== Player 1 Tiles ===");
    //    if (player1Tiles.Count == 0) {
    //        Debug.Log("No tiles for player 1.");
    //    }
    //    else {
    //        foreach (var kvp in player1Tiles) {
    //            Debug.Log($"Position: {kvp.Key}, Count: {kvp.Value}");
    //        }
    //    }
    //}
}

