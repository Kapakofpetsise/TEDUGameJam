using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PolygonCollider2D))]
public class LightCast : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public float spotlightAngle = 60f; // Total cone angle in degrees
    public int numberOfRays = 20;      // How many rays along the cone

    [Header("Raycast Settings")]
    public float maxDistance = 10f;
    public LayerMask obstacleMask;     // Layers for obstacles (including mirrors, if applicable)

    private PolygonCollider2D polyCollider;

    void Start()
    {
        polyCollider = GetComponent<PolygonCollider2D>();
        if (polyCollider == null)
        {
            Debug.LogError("PolygonCollider2D component not found!");
        }
        // Optional: mark as trigger if you want to use OnTriggerEnter2D.
        polyCollider.isTrigger = true;
    }

    void Update()
    {
        UpdateColliderShape();
    }

    void UpdateColliderShape()
    {
        Vector2 origin = transform.position;
        float halfAngle = spotlightAngle / 2f;
        // Base angle is determined by the GameObject's right direction.
        float baseAngle = Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;

        // Calculate endpoints for each ray along the spotlight cone.
        List<Vector2> endpoints = new List<Vector2>();
        for (int i = 0; i < numberOfRays; i++)
        {
            float t = i / (float)(numberOfRays - 1);
            float currentAngle = baseAngle - halfAngle + t * spotlightAngle;
            Vector2 direction = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, maxDistance, obstacleMask);
            if (hit.collider != null)
            {
                endpoints.Add(hit.point);
            }
            else
            {
                endpoints.Add(origin + direction * maxDistance);
            }
        }

        // Build the polygon: start with the light origin and add the endpoints.
        List<Vector2> polyPoints = new List<Vector2>();
        polyPoints.Add(origin);
        polyPoints.AddRange(endpoints);
        // Optionally, add the origin again to close the shape.
        polyPoints.Add(origin);

        // Convert world space points to local space relative to this GameObject.
        for (int i = 0; i < polyPoints.Count; i++)
        {
            polyPoints[i] = transform.InverseTransformPoint(polyPoints[i]);
        }

        // Update the PolygonCollider2D.
        polyCollider.pathCount = 1;
        polyCollider.SetPath(0, polyPoints.ToArray());
    }
}
