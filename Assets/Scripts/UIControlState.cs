using UnityEngine;

public class UIControlState : MonoBehaviour {
    [SerializeField] private GameObject _controlsUI;
    public static bool IsOpen { get; private set; }
    private void OnEnable() {
        IsOpen = true;
    }
    private void OnDisable() {
        IsOpen = false;
    }

    public void EnableControlsUI() {
        _controlsUI.SetActive(true);
    }

    public void DisableControlsUI() {

    }
}