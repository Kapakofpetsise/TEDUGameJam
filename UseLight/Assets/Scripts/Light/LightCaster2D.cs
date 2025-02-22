using UnityEngine;

public class LightCaster2D : MonoBehaviour
{
    public int numberOfRays = 360; // Number of rays for the light
    public float maxDistance = 5f; // Max distance the rays will travel
    public int maxReflections = 2; // How many times the light can reflect
    public LayerMask obstacleMask; // LayerMask to detect obstacles

    void Update()
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = i * (360f / numberOfRays); // Spread rays evenly in 360 degrees
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            CastRay(transform.position, direction, maxReflections);
        }
    }

    void CastRay(Vector2 startPos, Vector2 direction, int reflectionsRemaining)
    {
        Vector2 origin = startPos;
        Vector2 dir = direction;
        
        for (int i = 0; i <= reflectionsRemaining; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, obstacleMask);

            if (hit.collider != null)
            {
                Debug.DrawLine(origin, hit.point, Color.red); // Draw the ray
                
                if (hit.collider.CompareTag("Mirror")) // Check if it's a mirror
                {
                    // Compute reflection
                    dir = Vector2.Reflect(dir, hit.normal);
                    origin = hit.point; // Start from reflection point
                }
                else
                {
                    break; // Stop if it hits a non-mirror object
                }
            }
            else
            {
                Debug.DrawLine(origin, origin + dir * maxDistance, Color.yellow);
                break;
            }
        }
    }
}
