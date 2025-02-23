using UnityEngine;

public class MusicManager : MonoBehaviour {
    private static MusicManager instance;
    private AudioSource audioSource;

    [SerializeField] private AudioClip backgroundMusic; // Assign in Inspector

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject); // Makes sure this object persists across levels
            audioSource = GetComponent<AudioSource>();

            if (backgroundMusic != null) {
                audioSource.clip = backgroundMusic;
                audioSource.loop = true; // Loop the music
                audioSource.Play();
            }
        }
        else {
            Destroy(gameObject); // Prevents multiple MusicManagers from existing
        }
    }
}
