using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyA") || other.CompareTag("EnemyB")) {
            other.GetComponent<Enemy>().ExposeToLight(true);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("EnemyA") || other.CompareTag("EnemyB")) {
            other.GetComponent<Enemy>().ExposeToLight(false);
        }
    }
}
