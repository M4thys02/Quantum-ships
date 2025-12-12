using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
    private ChangePlayerManager _changePlayerManager;

    private void Awake() {
        _changePlayerManager = GetComponentInChildren<ChangePlayerManager>();

        _changePlayerManager.UpdateTilemapsVisibility();
    }
    public void GameFinished() {
        PlayersSetUps.Cleanup();             // destroy persistent object
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangePlayer() {
        _changePlayerManager.ChangePlayer();
    }
}
