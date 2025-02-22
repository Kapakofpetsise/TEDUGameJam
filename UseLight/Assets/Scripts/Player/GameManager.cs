using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;

    private List<ExitDoor> exitDoors;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        exitDoors = new List<ExitDoor>(FindObjectsOfType<ExitDoor>());
    }

    public void CheckLevelCompletion() {
        foreach (ExitDoor exit in exitDoors) {
            if (!exit.HasCharacterReached())
                return;
        }

        Debug.Log("All characters reached their exits! Loading next level...");
        LoadNextLevel();
    }

    private void LoadNextLevel() {
        string nextLevel = null;

       
        foreach (ExitDoor exit in exitDoors) {
            string exitLevel = exit.GetNextLevelName();
            if (!string.IsNullOrEmpty(exitLevel)) {
                nextLevel = exitLevel;
                break; 
            }
        }

        if (!string.IsNullOrEmpty(nextLevel)) {
            Debug.Log("Loading level: " + nextLevel);
            SceneManager.LoadScene(nextLevel);
        }
        else {
            Debug.LogWarning(" No next level.");
        }
    }
}
