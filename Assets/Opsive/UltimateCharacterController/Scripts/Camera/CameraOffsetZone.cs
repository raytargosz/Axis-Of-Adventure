using System.Collections;
using UnityEngine;
using Opsive.UltimateCharacterController.Camera;

public class CameraOffsetZone : MonoBehaviour
{
    [Tooltip("The new camera anchor offset values inside the zone.")]
    public Vector3 anchorOffsetInsideZone;

    [Tooltip("The camera anchor offset values outside the zone.")]
    public Vector3 anchorOffsetOutsideZone;

    [Tooltip("The transition duration when entering or exiting the zone.")]
    public float transitionDuration = 1f;

    private CameraController cameraController;
    private bool isInsideZone;

    private void Start()
    {
        var mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        if (mainCamera != null)
        {
            cameraController = mainCamera.GetComponent<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("CameraController component not found on the main camera.");
            }
        }
        else
        {
            Debug.LogError("Main camera not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideZone = true;
            StartCoroutine(UpdateCameraAnchorOffset(anchorOffsetInsideZone));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideZone = false;
            StartCoroutine(UpdateCameraAnchorOffset(anchorOffsetOutsideZone));
        }
    }

    private IEnumerator UpdateCameraAnchorOffset(Vector3 targetAnchorOffset)
    {
        if (cameraController == null)
        {
            yield break;
        }

        float timer = 0f;
        Vector3 initialAnchorOffset = cameraController.AnchorOffset;

        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            cameraController.AnchorOffset = Vector3.Lerp(initialAnchorOffset, targetAnchorOffset, timer / transitionDuration);
            yield return null;
        }

        cameraController.AnchorOffset = targetAnchorOffset;
    }
}