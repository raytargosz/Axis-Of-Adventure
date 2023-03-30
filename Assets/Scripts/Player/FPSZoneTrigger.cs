using UnityEngine;

public class FPSZoneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] assetsToEnable;
    [SerializeField] private GameObject[] assetsToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleAssets(assetsToEnable, true);
            ToggleAssets(assetsToDisable, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleAssets(assetsToEnable, false);
            ToggleAssets(assetsToDisable, true);
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