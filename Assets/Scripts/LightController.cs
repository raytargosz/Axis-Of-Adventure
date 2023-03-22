using System.Collections;
using UnityEngine;
using DistantLands.Cozy;

public class LightController : MonoBehaviour
{
    [System.Serializable]
    public struct LightTimeRange
    {
        public float lowerBound;
        public float upperBound;
    }

    public CozyWeather cozyWeather;
    public Light[] lights;
    public LightTimeRange[] lightTimeRanges;
    public float rampTime = 3f;

    private bool areLightsOn = false;

    private void Update()
    {
        float currentTicks = cozyWeather.currentTicks;

        bool shouldBeOn = false;
        foreach (LightTimeRange range in lightTimeRanges)
        {
            if (currentTicks >= range.lowerBound && currentTicks <= range.upperBound)
            {
                shouldBeOn = true;
                break;
            }
        }

        if (!areLightsOn && shouldBeOn)
        {
            StartCoroutine(RampLightsIntensity(true));
            areLightsOn = true;
        }
        else if (areLightsOn && !shouldBeOn)
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

        foreach (Light light in lights)
        {
            FlickeringLight flickeringLight = light.GetComponent<FlickeringLight>();
            if (flickeringLight != null)
            {
                flickeringLight.IsFlickering = rampUp;
            }
        }

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