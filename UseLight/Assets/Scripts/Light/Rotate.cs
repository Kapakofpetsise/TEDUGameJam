using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float minAngle = 0f;  // Minimum rotation angle
    public float maxAngle = 270f;   // Maximum rotation angle
    public float speed = 10f;      // Speed of rotation

    private float t = 0f;

    void Update()
    {
        t += Time.deltaTime * speed / 100f; // Normalize speed
        float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.PingPong(t, 1));
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}