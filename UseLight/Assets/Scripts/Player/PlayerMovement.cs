using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private float grabRange = 1f;
    [SerializeField] private LayerMask grabLayer;


    private Vector2 _movement;
    private Rigidbody2D _rb;
    private Rigidbody2D grabbedObject;
    private Collider2D detectedObject;
    private bool isActive = false;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (!isActive) return; // Prevents inactive characters from rotating or moving

        _movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        if (_movement != Vector2.zero) {
            _rb.linearVelocity = _movement * _moveSpeed;
        }
        else {
            _rb.linearVelocity = Vector2.zero;
        }

        // Only rotate if the character is active
        Vector2 lookDirection = InputManager.LookDirection - (Vector2)transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        _rb.rotation = Mathf.LerpAngle(_rb.rotation, angle, _rotationSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.E)) {
            if (grabbedObject == null) {
                TryGrabObject();
            }
            else {
                DropObject();
            }
        }

        // Move grabbed object with the player
        if (grabbedObject != null) {
            grabbedObject.MovePosition(grabPoint.position);
        }
    }

    private void TryGrabObject() {
        if (detectedObject != null) {
            grabbedObject = detectedObject.attachedRigidbody;

            if (grabbedObject != null) {
                grabbedObject.isKinematic = true; 
                grabbedObject.transform.parent = grabPoint; 
            }
        }
    }

    private void DropObject() {
        if (grabbedObject != null) {
            grabbedObject.isKinematic = false;
            grabbedObject.transform.parent = null;
            grabbedObject = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if ((grabLayer.value & (1 << other.gameObject.layer)) > 0) {
            detectedObject = other;
            Debug.Log("Detected object: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other == detectedObject) {
            detectedObject = null;
            Debug.Log("Lost detection of object: " + other.gameObject.name);
        }
    }

    public void SetActiveState(bool state) {
        isActive = state;
        _rb.linearVelocity = Vector2.zero; // Stop movement when switching away

        if (!isActive) {
            _rb.angularVelocity = 0; // Stop rotation when inactive
        }
    }
}
