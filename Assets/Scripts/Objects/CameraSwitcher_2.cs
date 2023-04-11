using UnityEngine;

public class CameraSwitcher_2 : MonoBehaviour
{
    [Tooltip("The main player camera")]
    public Camera mainCamera;
    [Tooltip("The secondary stationary camera")]
    public Camera secondaryCamera;
    [Tooltip("Layer mask for the player")]
    public LayerMask playerLayer;

    private void Start()
    {
        // Ensure the secondary camera is initially disabled
        secondaryCamera.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Player entered trigger");
            // Disable the main camera and enable the secondary camera
            mainCamera.enabled = false;
            secondaryCamera.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is the player
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            Debug.Log("Player exited trigger");
            // Re-enable the main camera and disable the secondary camera
            mainCamera.enabled = true;
            secondaryCamera.enabled = false;

            // Disable the trigger collider
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
