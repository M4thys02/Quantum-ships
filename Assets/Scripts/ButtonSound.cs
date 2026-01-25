using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour {
    [Header("Leave empty for default tone")]
    public AudioClip customClip;
    private void Start() {
        GetComponent<Button>().onClick.AddListener(PlaySound);
    }

    private void PlaySound() {
        if (GlobalAudioManager.Instance != null) {
            if (customClip != null) {
                GlobalAudioManager.Instance.ButtonClick(customClip);
            }
            else {
                GlobalAudioManager.Instance.PlayButtonClickSound();
            }
        }
    }

}
