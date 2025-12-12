using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;

    public void loadGame(string sceneName) {
        //Debug.Log("loadGame() called with " + sceneName);
        StartCoroutine(LoadGame(sceneName));
    }

    IEnumerator LoadGame(string newScene) {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(newScene);
    }
}
