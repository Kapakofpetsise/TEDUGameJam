using UnityEngine;
using UnityEngine.Events;

public class LightDetection : MonoBehaviour
{
    public Transform redLightTransform;  
    public UnityEvent inRedLight;  

    public Transform greenLightTransform;  
    public UnityEvent inGreenLight;  
    public UnityEvent inYellowLight;  // green and red light
    public UnityEvent notInLight;  
    
    public CircleCollider2D targetCollider;  // The target object with a CircleCollider2D
    
    public LayerMask ignoreLayer;

    public int numberOfRays = 8;  // Number of rays to cast around the circle's perimeter

    bool isInRedLight = false;
    bool isInGreenLight = false;


    
    void Update()
    {
        isInGreenLight = IsInLight(greenLightTransform);
        isInRedLight = IsInLight(redLightTransform);

        if(isInRedLight && isInGreenLight){
            inYellowLight?.Invoke();
        }else if(isInRedLight){
            inRedLight?.Invoke();
        }else if(isInGreenLight){
            inGreenLight?.Invoke();
        }else{
            notInLight?.Invoke();
        }
    }

    bool IsInLight(Transform point){

        // Get the world position of the circle's center and radius
        Vector2 center = targetCollider.transform.position;
        float radius = targetCollider.radius;

        // Calculate and cast rays to points on the circle's perimeter
        for (int i = 0; i < numberOfRays; i++)
        {
            // Calculate the angle for the current point
            float angle = i * (360f / numberOfRays);

            // Calculate the position on the circle's perimeter
            Vector2 targetPoint = center + new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle) * radius, Mathf.Sin(Mathf.Deg2Rad * angle) * radius);

            // Cast ray to this target point
            if(CastRayToPoint(point.position, targetPoint)){
                return true;
            };
        }
        return false;
    }

    bool CastRayToPoint(Vector2 startPoint, Vector2 targetPoint)
    {
        // Calculate direction from startPoint to targetPoint
        Vector2 direction = (targetPoint - startPoint).normalized;

        // Cast the ray
        RaycastHit2D hit = Physics2D.Raycast(startPoint, direction, Mathf.Infinity, ~ignoreLayer);

        // Debugging: Draw the ray path in the Scene view
        Debug.DrawLine(startPoint, targetPoint, Color.red);

        if (hit.collider == targetCollider)
        {
            Debug.Log("Hit object: " + hit.collider.name);
            return true;
        }
        else
        {
            return false;
        }
    }
}