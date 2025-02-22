using UnityEngine;

public class ExitDoor : MonoBehaviour {
    [SerializeField] private string characterTag;
    [SerializeField] private string nextLevelName; 

    private bool characterReached = false;

    public bool HasCharacterReached() => characterReached;
    public string GetNextLevelName() => nextLevelName;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(characterTag)) 
        {
            characterReached = true;
            Debug.Log(other.name + " reached their exit (" + gameObject.name + ")");
            GameManager.Instance.CheckLevelCompletion();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(characterTag)) 
        {
            characterReached = false;
            Debug.Log(other.name + " left the exit.");
        }
    }
}
