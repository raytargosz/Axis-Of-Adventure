using UnityEngine;

public class DarkZone : MonoBehaviour
{
    public VignetteEffectController vignetteController;
    public Light playerLight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !IsPlayerLightActive())
        {
            vignetteController.EnterDarkZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vignetteController.ExitDarkZone();
        }
    }

    private bool IsPlayerLightActive()
    {
        return playerLight != null && playerLight.enabled;
    }
}
