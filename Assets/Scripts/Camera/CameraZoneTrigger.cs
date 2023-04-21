using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    [Tooltip("The player's tag")]
    public string playerTag = "Player";

    [Tooltip("The CameraZoomAndBob script to enable/disable")]
    public CameraZoomAndBob cameraZoomAndBob;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            cameraZoomAndBob.StartCameraEffect();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            cameraZoomAndBob.StopCameraEffect();
        }
    }
}
