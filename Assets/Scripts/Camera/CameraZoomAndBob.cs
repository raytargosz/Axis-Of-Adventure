using System.Collections;
using UnityEngine;

public class CameraZoomAndBob : MonoBehaviour
{
    [Tooltip("The camera to apply the FOV and rotation change")]
    public Camera targetCamera;

    [Tooltip("The new FOV range when the player is in the zone")]
    public Vector2 fovRange = new Vector2(30f, 60f);

    [Tooltip("The Z rotation range when the player is in the zone")]
    public Vector2 zRotationRange = new Vector2(-10f, 10f);

    [Tooltip("The duration of a complete cycle (in seconds)")]
    public float cycleDuration = 1f;

    private float defaultFOV;
    private Quaternion defaultRotation;

    private void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogError("CameraZoneTrigger: No camera assigned.");
            enabled = false;
            return;
        }

        defaultFOV = targetCamera.fieldOfView;
        defaultRotation = targetCamera.transform.rotation;
    }

    public void StartCameraEffect()
    {
        StartCoroutine(ZoomAndRotate());
    }

    public void StopCameraEffect()
    {
        StopAllCoroutines();
        targetCamera.fieldOfView = defaultFOV;
        targetCamera.transform.rotation = defaultRotation;
    }

    private IEnumerator ZoomAndRotate()
    {
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime;

            // Calculate progress and bounce value
            float progress = (timer % cycleDuration) / cycleDuration;
            float bounceProgress = Mathf.Sin(progress * Mathf.PI * 2);

            // Update FOV
            targetCamera.fieldOfView = Mathf.Lerp(fovRange.x, fovRange.y, bounceProgress);

            // Update Z rotation
            float targetZRotation = Mathf.Lerp(zRotationRange.x, zRotationRange.y, bounceProgress);
            Quaternion targetRotation = Quaternion.Euler(defaultRotation.eulerAngles.x, defaultRotation.eulerAngles.y, targetZRotation);
            targetCamera.transform.rotation = targetRotation;

            yield return null;
        }
    }
}
