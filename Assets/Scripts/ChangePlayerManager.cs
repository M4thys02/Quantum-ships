using UnityEngine;
using UnityEngine.Tilemaps;

public class ChangePlayerManager : MonoBehaviour {
    public Tilemap player0Tilemap;
    public Tilemap player1Tilemap;

    private int currentPlayer = 0;

    public int CurrentPlayer => currentPlayer;

    public void ChangePlayer() {
        currentPlayer = (currentPlayer == 0) ? 1 : 0;
        UpdateTilemapsVisibility();
    }

    public void UpdateTilemapsVisibility() {
        player0Tilemap.gameObject.SetActive(currentPlayer == 0);
        player1Tilemap.gameObject.SetActive(currentPlayer == 1);
    }

    public Tilemap GetActiveTilemap() {
        return currentPlayer == 0 ? player0Tilemap : player1Tilemap;
    }
}
