using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 Movement;
    public static Vector2 LookDirection;

    public PlayerInput _playerInput;
    public InputAction _moveAction;
    public InputAction _lookAction;

    private void Awake() {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
    }

    private void Update() {
        Movement = _moveAction.ReadValue<Vector2>();
        
        Vector2 mousePosition = _lookAction.ReadValue<Vector2>();
        LookDirection = Camera.main.ScreenToWorldPoint(mousePosition);
    }

}
