using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class VignetteEffectController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;
    public Vignette vignette;
    public float darkZoneIntensity = 0.5f;
    public GameObject darkZonePrompt;

    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out vignette);
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
