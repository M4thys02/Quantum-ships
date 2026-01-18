using UnityEngine;
using UnityEngine.SceneManagement;

public class GuideScreen : MonoBehaviour {
    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
