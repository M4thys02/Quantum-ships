using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public LevelLoader _levelLoader;
    [SerializeField] private GameObject _blockPanel;
    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame(string sceneName) {
        _blockPanel.SetActive(true);
        _levelLoader.loadGame(sceneName);
    }

    public void quitApp() {
        Application.Quit();
    }
}
