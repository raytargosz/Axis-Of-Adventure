using UnityEngine;
using System.Collections;

public class FPSZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] assetsToEnable;
    [SerializeField] private GameObject[] assetsToDisable;
    [SerializeField] private CombinedPlayerController playerController;
    [SerializeField] private float fpsCameraYOffset = 1f; 

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
            StartCoroutine(ToggleAssetsWithDelay(assetsToEnable, false, playerController.cameraSwitchTime));
            StartCoroutine(ToggleAssetsWithDelay(assetsToDisable, true, playerController.cameraSwitchTime));

            if (playerController != null)
            {
                playerController.firstPersonCamera.transform.localPosition = initialCameraPosition;
            }
        }
    }

    private IEnumerator ToggleAssetsWithDelay(GameObject[] assets, bool enable, float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleAssets(assets, enable);
    }


    private void ToggleAssets(GameObject[] assets, bool enable)
    {
        foreach (GameObject asset in assets)
        {
            asset.SetActive(enable);
        }
    }
}