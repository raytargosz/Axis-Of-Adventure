using UnityEngine;

// Controls the behavior of the light beam and its interactions with mirrors and the target.
public class LightBeamController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Max distance the light beam can travel.")]
    public float maxBeamDistance = 100f;

    [Tooltip("Layer mask for mirrors and targets.")]
    public LayerMask mirrorTargetLayer;

    [Space(10)]
    [Header("Light Beam Effects")]
    [Tooltip("Prefab for the light beam effect.")]
    public GameObject lightBeamEffectPrefab;

    [Tooltip("Width of the light beam effect.")]
    public float lightBeamWidth = 0.1f;

    [Space(10)]
    [Header("Debug")]
    [Tooltip("Enable debug mode to show raycasts.")]
    public bool debugMode = false;

    private LineRenderer lineRenderer;

    private void Start()
    {
        InitializeLineRenderer();
        UpdateLightBeam();
    }

    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.startWidth = lightBeamWidth;
        lineRenderer.endWidth = lightBeamWidth;
        lineRenderer.material = lightBeamEffectPrefab.GetComponent<Renderer>().sharedMaterial;
    }

    // Update the light beam's path whenever necessary (e.g., when a mirror is moved or rotated).
    public void UpdateLightBeam()
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentDirection = transform.forward;

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPosition);

        int reflectionCount = 0;

        while (reflectionCount < 100) // Limit the number of reflections to avoid infinite loops.
        {
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, currentDirection, out hit, maxBeamDistance, mirrorTargetLayer))
            {
                reflectionCount++;
                currentPosition = hit.point;
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(reflectionCount, currentPosition);

                // Check if the hit object has a specific tag (e.g., "Mirror" or "Target").
                if (hit.collider.CompareTag("Mirror")) // Hit object is a mirror, reflect the light beam.
                {
                    currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                }
                else if (hit.collider.CompareTag("Target")) // Hit object is a target, stop updating the light beam.
                {
                    // Implement target activation logic here.
                    break;
                }
                else // Hit object is neither a mirror nor a target, stop updating the light beam.
                {
                    break;
                }
            }
            else
            {
                // No hit, extend the light beam to the maximum distance.
                lineRenderer.positionCount += 1;
                lineRenderer.SetPosition(reflectionCount + 1, currentPosition + currentDirection * maxBeamDistance);
                break;
            }

            if (debugMode)
            {
                Debug.DrawRay(currentPosition, currentDirection * maxBeamDistance, Color.red);
            }
        }
    }
}
