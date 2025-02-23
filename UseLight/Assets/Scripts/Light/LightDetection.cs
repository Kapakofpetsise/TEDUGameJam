using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class LightDetection : MonoBehaviour
{

    [System.Serializable]
    public struct TransformEventPair
    {
        public Transform lightTransform;
        public UnityEvent inLight;

        public TransformEventPair(Transform transform)
        {
            this.lightTransform = transform;
            this.inLight = new UnityEvent();
        }
    }
    public UnityEvent notInLight;  
    
    public CircleCollider2D targetCollider;  // The target object with a CircleCollider2D
    
    public LayerMask ignoreLayer;

    public int numberOfRays = 8;  // Number of rays to cast around the circle's perimeter

    public List<TransformEventPair> lightPairs = new List<TransformEventPair>();

    bool isLighted = false;

    void Update()
    {
        isLighted = false;
        foreach (TransformEventPair pair in lightPairs)
        {
            if (IsInLight(pair.lightTransform))
            {
                pair.inLight?.Invoke(); // Invoke the event if assigned
                isLighted = true;
            }
        }
        if(!isLighted){notInLight?.Invoke();}
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