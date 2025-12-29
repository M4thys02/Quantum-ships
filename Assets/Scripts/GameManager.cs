using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
    private ChangePlayerManager _changePlayerManager;
    private MeasureManager _measureManager;

    private void Awake() {
        _changePlayerManager = GetComponentInChildren<ChangePlayerManager>();
        _measureManager = GetComponentInChildren<MeasureManager>();
    }
    public void GameFinished() {
        PlayersSetUps.Cleanup();             // destroy persistent object
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangePlayer() {
        _changePlayerManager.ChangePlayer();
        _measureManager.UpdateMeasurementsVisibility(_changePlayerManager.GetActivatePlayer());
    }

    public void PlayerAttack() {

    }

    public void PlayerMeasure() {
        int currentPlayer = _changePlayerManager.GetActivatePlayer();
        Vector2Int measuredTile = PlayersSetUps.GetWeightedRandomTileForPlayer(currentPlayer);
        _measureManager.CurrentMeasurement(measuredTile, currentPlayer);
    }
}
