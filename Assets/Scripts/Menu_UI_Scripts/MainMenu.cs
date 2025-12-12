using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public LevelLoader _levelLoader;
    public void GoToScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    public void StartGame(string sceneName) {
        _levelLoader.loadGame(sceneName);
    }

    public void quitApp() {
        Application.Quit();
        //Debug.Log("Application has quit");
    }
}
