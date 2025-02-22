using UnityEngine;

public class ExitDoor : MonoBehaviour {
    [SerializeField] private string characterTag;
    [SerializeField] private string nextLevelName; 

    private bool characterReached = false;

    public bool HasCharacterReached() => characterReached;
    public string GetNextLevelName() => nextLevelName;

    private string childName ="Activated"; // Name of the child to activate/deactivate

    private Transform childTransform;

    void Start()
    {
        childTransform = transform.Find(childName);
        if (childTransform == null)
        {
            Debug.LogError("Child not found: " + childName);
        }
        DeactivateChild();
    }

    public void ActivateChild()
    {
        if (childTransform != null)
        {
            childTransform.gameObject.SetActive(true);
        }
    }

    public void DeactivateChild()
    {
        if (childTransform != null)
        {
            childTransform.gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(characterTag)) 
        {
            characterReached = true;
            ActivateChild();
            Debug.Log(other.name + " reached their exit (" + gameObject.name + ")");
            GameManager.Instance.CheckLevelCompletion();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag(characterTag)) 
        {
            characterReached = false;
            DeactivateChild();
            Debug.Log(other.name + " left the exit.");
        }
    }
}
