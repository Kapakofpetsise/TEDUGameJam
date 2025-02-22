using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {
    [SerializeField] private List<PlayerMovement> players; // List of all playable characters
    private int currentPlayerIndex = 0; // Index of the active player
    private Camera mainCamera;

    private void Start() {
        mainCamera = Camera.main; // Get the main camera
        ActivatePlayer(currentPlayerIndex);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            SwitchToNextPlayer();
        }

        // Smoothly move camera to the active player
        if (players.Count > 0) {
            Vector3 targetPosition = players[currentPlayerIndex].transform.position;
            targetPosition.z = -10; // Keep the camera at a fixed depth
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * 5f);
        }
    }

    private void SwitchToNextPlayer() {
        // Disable the current player
        players[currentPlayerIndex].SetActiveState(false);

        // Move to the next player in the list
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

        // Enable the new player
        ActivatePlayer(currentPlayerIndex);
    }

    private void ActivatePlayer(int index) {
        players[index].SetActiveState(true);
    }
}
