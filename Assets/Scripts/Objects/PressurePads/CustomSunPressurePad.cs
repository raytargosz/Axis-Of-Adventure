using UnityEngine;
using DistantLands.Cozy;

public class CustomSunPressurePad : MonoBehaviour
{
    public CozyWeather weatherController;
    public float minTickCount = 80;
    public float maxTickCount = 200;
    public float interval = 20;
    public float tickCountSpeed = 1f;
    public AudioClip stepOnSFX;
    public AudioClip stepOffSFX;
    public AudioSource audioSource;

    private bool playerOnPad = false;

    private void Update()
    {
        if (playerOnPad && weatherController != null)
        {
            FastForwardTicks();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPad = true;
            PlaySFX(stepOnSFX);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPad = false;
            PlaySFX(stepOffSFX);
        }
    }

    private void FastForwardTicks()
    {
        if (weatherController.currentTicks < minTickCount) return;

        float targetTicks = Mathf.Min(weatherController.currentTicks + interval, maxTickCount);
        float tickIncrement = tickCountSpeed * Time.deltaTime;

        if (weatherController.currentTicks < targetTicks)
        {
            weatherController.currentTicks = Mathf.Min(weatherController.currentTicks + tickIncrement, targetTicks);
        }
    }

    private void PlaySFX(AudioClip sfx)
    {
        if (audioSource != null && sfx != null)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}