using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public DistantLands.Cozy.CozyController cozyController;
    public Light[] lights;
    public float ticksLowerBound;
    public float ticksUpperBound;
    public float rampTime = 3f;

    private bool areLightsOn = false;

    private void Update()
    {
        float currentTicks = cozyController.currentTicks;

        if (!areLightsOn && currentTicks >= ticksLowerBound && currentTicks <= ticksUpperBound)
        {
            StartCoroutine(RampLightsIntensity(true));
            areLightsOn = true;
        }
        else if (areLightsOn && (currentTicks < ticksLowerBound || currentTicks > ticksUpperBound))
        {
            StartCoroutine(RampLightsIntensity(false));
            areLightsOn = false;
        }
    }

    private IEnumerator RampLightsIntensity(bool rampUp)
    {
        float startTime = Time.time;
        float initialIntensity = lights[0].intensity;
        float targetIntensity = rampUp ? 1 : 0;

        while (Time.time - startTime < rampTime)
        {
            float progress = (Time.time - startTime) / rampTime;
            float newIntensity = Mathf.Lerp(initialIntensity, targetIntensity, progress);

            foreach (Light light in lights)
            {
                light.intensity = newIntensity;
            }

            yield return null;
        }
    }
}