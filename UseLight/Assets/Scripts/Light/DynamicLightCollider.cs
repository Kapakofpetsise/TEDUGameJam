using UnityEngine;
using System.Collections.Generic;

public class DynamicLightCollider : MonoBehaviour
{
    [Header("Spotlight Cone Settings")]
    [Tooltip("Total angle (in degrees) of the spotlight cone.")]
    public float spotlightAngle = 60f;
    [Tooltip("Number of rays used to form the cone.")]
    public int numberOfRays = 10;
    
    [Header("Raycast Settings")]
    [Tooltip("How far each ray segment can travel.")]
    public float maxDistance = 10f;
    [Tooltip("Maximum number of reflections allowed per ray.")]
    public int maxReflections = 2;
    [Tooltip("Layers that include mirrors, lenses, and obstacles.")]
    public LayerMask obstacleMask;
    
    [Header("Lens Settings")]
    [Tooltip("Offset from the lens hit point (along the hit normal) to determine the exit point.")]
    public float lensExitOffset = 0.5f;
    
    [Header("Prefabs for LineRenderers")]
    [Tooltip("Prefab for each left-side beam (wide beam with a LineRenderer).")]
    public GameObject leftRayPrefab;
    [Tooltip("Prefab for the single lens laser (thin beam with a LineRenderer).")]
    public GameObject lensLaserPrefab;
    
    // Pool of LineRenderers for the left-side rays.
    private List<LineRenderer> leftRayPool = new List<LineRenderer>();
    
    // The single LineRenderer for the lens laser.
    private LineRenderer lensLineRenderer;
    
    // Variables for lens hit information.
    private bool lensTriggered;
    private Transform lensTransformHit;
    private Vector2 lensHitPoint;
    private Vector2 lensHitNormal;
    
    void Start()
    {
        // Create a pool of left-ray LineRenderers.
        for (int i = 0; i < numberOfRays; i++)
        {
            GameObject lrObj = Instantiate(leftRayPrefab, transform);
            lrObj.name = "LeftRay_" + i;
            LineRenderer lr = lrObj.GetComponent<LineRenderer>();
            if (lr == null)
            {
                Debug.LogError("leftRayPrefab must have a LineRenderer component!");
            }
            leftRayPool.Add(lr);
        }
        
        // Instantiate the lens laser LineRenderer.
        if (lensLaserPrefab != null)
        {
            GameObject lensObj = Instantiate(lensLaserPrefab, transform);
            lensObj.name = "LensLaser";
            lensLineRenderer = lensObj.GetComponent<LineRenderer>();
            if (lensLineRenderer == null)
            {
                Debug.LogError("lensLaserPrefab must have a LineRenderer component!");
            }
            lensLineRenderer.enabled = false; // Hide by default.
        }
        else
        {
            Debug.LogWarning("No lensLaserPrefab assigned. The lens won't shoot a laser.");
        }
    }
    
    void Update()
    {
        // Reset lens trigger information at the start of each frame.
        lensTriggered = false;
        lensTransformHit = null;
        lensHitPoint = Vector2.zero;
        lensHitNormal = Vector2.zero;
        
        // Cast all left-side rays.
        CastAllLeftRays();
        
        // If any ray hit a lens, process the outgoing laser beam.
        if (lensTriggered && lensLineRenderer != null)
        {
            lensLineRenderer.enabled = true;
            // Compute the exit point on the other side of the lens.
            Vector2 lensOrigin = lensHitPoint - lensHitNormal * lensExitOffset;
            // Define the outgoing beam's direction.
            // For a fixed horizontal right beam, use Vector2.right.
            // Alternatively, you can use the lens's local right via lensTransformHit.right.
            Vector2 laserDirection = Vector2.right;
            
            // Cast the lens beam with reflection.
            List<Vector3> laserPoints = CastRayPostLens(lensOrigin, laserDirection, maxReflections);
            lensLineRenderer.positionCount = laserPoints.Count;
            lensLineRenderer.SetPositions(laserPoints.ToArray());
        }
        else
        {
            if (lensLineRenderer != null)
                lensLineRenderer.enabled = false;
        }
    }
    
    /// <summary>
    /// Casts multiple rays in a cone and updates the left-side beam pool.
    /// </summary>
    private void CastAllLeftRays()
    {
        float halfAngle = spotlightAngle / 2f;
        // Base angle from transform.right.
        float baseAngle = Mathf.Atan2(transform.right.y, transform.right.x) * Mathf.Rad2Deg;
        
        for (int i = 0; i < numberOfRays; i++)
        {
            float t = (numberOfRays <= 1) ? 0f : i / (float)(numberOfRays - 1);
            float currentAngle = baseAngle - halfAngle + t * spotlightAngle;
            Vector2 direction = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            );
            
            // Get the ray path including reflections.
            List<Vector3> rayPoints = CastRayWithReflection(transform.position, direction, maxReflections);
            LineRenderer lr = leftRayPool[i];
            lr.positionCount = rayPoints.Count;
            lr.SetPositions(rayPoints.ToArray());
        }
    }
    
    /// <summary>
    /// Casts a ray from startPos in direction, reflecting off mirrors.
    /// If it hits a lens (tag "Lens") and no lens has been triggered this frame,
    /// stores the lens hit information and stops the ray.
    /// Returns a list of points for drawing the ray.
    /// </summary>
    private List<Vector3> CastRayWithReflection(Vector2 startPos, Vector2 direction, int reflectionsLeft)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(startPos);
        
        Vector2 origin = startPos;
        Vector2 dir = direction;
        
        for (int r = 0; r <= reflectionsLeft; r++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, obstacleMask);
            if (hit.collider != null)
            {
                points.Add(hit.point);
                
                if (hit.collider.CompareTag("Mirror"))
                {
                    // Reflect the ray.
                    dir = Vector2.Reflect(dir, hit.normal);
                    origin = hit.point + dir * 0.01f;
                }
                else if (hit.collider.CompareTag("Lens"))
                {
                    // Store lens hit info (only once per frame).
                    if (!lensTriggered)
                    {
                        lensTriggered = true;
                        lensTransformHit = hit.collider.transform;
                        lensHitPoint = hit.point;
                        lensHitNormal = hit.normal;
                    }
                    // Stop further raycasting once a lens is hit.
                    break;
                }
                else
                {
                    // Hit a non-mirror, non-lens obstacle.
                    break;
                }
            }
            else
            {
                points.Add(origin + dir * maxDistance);
                break;
            }
        }
        
        return points;
    }
    
    /// <summary>
    /// Casts a ray from the lens's exit point that reflects off mirrors.
    /// Returns a list of points for drawing the outgoing lens laser beam.
    /// </summary>
    private List<Vector3> CastRayPostLens(Vector2 startPos, Vector2 direction, int reflectionsRemaining)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(startPos);
        
        Vector2 origin = startPos;
        Vector2 dir = direction;
        
        for (int i = 0; i <= reflectionsRemaining; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, maxDistance, obstacleMask);
            if (hit.collider != null)
            {
                points.Add(hit.point);
                
                if (hit.collider.CompareTag("Mirror"))
                {
                    dir = Vector2.Reflect(dir, hit.normal);
                    origin = hit.point + dir * 0.01f;
                }
                else
                {
                    // Stop at the first non-mirror obstacle.
                    break;
                }
            }
            else
            {
                points.Add(origin + dir * maxDistance);
                break;
            }
        }
        
        return points;
    }
}
