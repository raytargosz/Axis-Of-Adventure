using UnityEngine;

public class CameraControlZone : MonoBehaviour
{
    [Header("Camera Settings")]
    public Vector3 newOffset;
    public float newRotationSpeed = 100f;
    public float newSmoothingSpeed = 0.125f;
    public float newRotationAngle = 45f;
    public float newVerticalAngle = 30f;
    public float newMinDistance = 5f;
    public float newMaxDistance = 15f;
    public float newMinFov = 30f;
    public float newMaxFov = 60f;
    public float newFovSpeed = 10f;
    public float transitionDuration = 1f;

    private IsometricCameraController isoCamController;
    private bool isInsideZone = false;

    private void Start()
    {
        isoCamController = FindObjectOfType<IsometricCameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInsideZone)
        {
            isInsideZone = true;
            isoCamController.UpdateCameraSettings(newOffset, newRotationSpeed, newSmoothingSpeed, newRotationAngle, newVerticalAngle, newMinDistance, newMaxDistance, newMinFov, newMaxFov, newFovSpeed, transitionDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInsideZone)
        {
            isInsideZone = false;
            isoCamController.ResetCameraSettings(transitionDuration);
        }
    }
}