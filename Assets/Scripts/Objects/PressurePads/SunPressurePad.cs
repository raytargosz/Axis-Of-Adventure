using UnityEngine;
using DistantLands.Cozy;

public class SunPressurePad : MonoBehaviour
{
    public CozyWeather weatherController;
    public float maxTickCount = 1000;
    public float ticksToFastForward = 100;
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
        float tickIncrement = tickCountSpeed * Time.deltaTime;
        weatherController.currentTicks = Mathf.Min(weatherController.currentTicks + tickIncrement, maxTickCount);
    }

    private void PlaySFX(AudioClip sfx)
    {
        if (audioSource != null && sfx != null)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}