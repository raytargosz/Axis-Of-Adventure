using UnityEngine;

public class FPSZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] assetsToEnable;
    [SerializeField] private GameObject[] assetsToDisable;
    [SerializeField] private CombinedPlayerController playerController;
    [SerializeField] private float fpsCameraYOffset = 1f; // Add this line for the Y-axis offset

    private Vector3 initialCameraPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleAssets(assetsToEnable, true);
            ToggleAssets(assetsToDisable, false);

            if (playerController != null)
            {
                initialCameraPosition = playerController.firstPersonCamera.transform.localPosition;
                Vector3 newCameraPosition = initialCameraPosition;
                newCameraPosition.y += fpsCameraYOffset;
                playerController.firstPersonCamera.transform.localPosition = newCameraPosition;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleAssets(assetsToEnable, false);
            ToggleAssets(assetsToDisable, true);

            if (playerController != null)
            {
                playerController.firstPersonCamera.transform.localPosition = initialCameraPosition;
            }
        }
    }

    private void ToggleAssets(GameObject[] assets, bool enable)
    {
        foreach (GameObject asset in assets)
        {
            asset.SetActive(enable);
        }
    }
}