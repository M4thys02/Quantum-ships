using UnityEngine;
using UnityEngine.SceneManagement;

public class AYSScreenControl : MonoBehaviour
{
    [SerializeField] private GameObject _areYouSureUI;

    public static bool IsOpen { get; private set; }

    private void OnEnable() {
        IsOpen = true;
    }
    private void OnDisable() {
        IsOpen = false;
    }

    public void EnableAYSUI() {
        _areYouSureUI.SetActive(true);
    }

    public void DisableAYSUI() {
        _areYouSureUI.SetActive(false);
    }

    public void GoToMainMenu() {
        PlayersSetUps.Cleanup();
        SceneManager.LoadScene("MainMenu");
    }
}
