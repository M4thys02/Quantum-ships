using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalAudioManager : MonoBehaviour
{
    public static GlobalAudioManager Instance { get; private set; }
    [Header("UI / Button Sounds")]
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip measureButtonSound;
    [SerializeField] private AudioClip attackButtonSound;
    [SerializeField] private AudioClip changePlayerButtonSound;

    [Header("Other sound clips")]
    [SerializeField] private AudioClip squarePlaceSound;
    [SerializeField] private AudioClip winSound;

    [Header("Audio Source for UI / SFX")]
    [SerializeField] private AudioSource audioSource;

    [Header("Default Volume")]
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void ButtonClick(AudioClip clip) {
        if (clip != null && audioSource != null) {
            audioSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayButtonClickSound() {
        if (buttonClickClip != null && audioSource != null) {
            audioSource.PlayOneShot(buttonClickClip, volume);
        }
    }

    public void PlayWinSound() {
        audioSource.PlayOneShot(winSound, volume);
    }

    public void PlayPopSound() {
        audioSource.PlayOneShot(squarePlaceSound, volume);
    }

    public void PlayButtonSound(int buttonType) {
        switch (buttonType) {
            case 0: audioSource.PlayOneShot(attackButtonSound, volume); break;
            case 1: audioSource.PlayOneShot(measureButtonSound, volume); break;
            case 2: audioSource.PlayOneShot(changePlayerButtonSound, volume); break;
        }
    }
}
