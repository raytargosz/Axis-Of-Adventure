using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class VignetteEffectController : MonoBehaviour
{
    public VolumeProfile volumeProfile;
    public Vignette vignette;
    public float darkZoneIntensity = 0.5f;
    public GameObject darkZonePrompt;

    private void Start()
    {
        volumeProfile.TryGet(out vignette);
    }

    public void EnterDarkZone()
    {
        vignette.intensity.Override(darkZoneIntensity);
        darkZonePrompt.SetActive(true);
    }

    public void ExitDarkZone()
    {
        vignette.intensity.Override(0);
        darkZonePrompt.SetActive(false);
    }
}