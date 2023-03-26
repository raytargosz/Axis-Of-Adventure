using UnityEngine;
using DistantLands.Cozy;
using System.Collections;

public class TickController : MonoBehaviour
{
    [Tooltip("Number of collectibles to collect before increasing tick count")]
    [SerializeField] private int collectiblesRequired = 5;

    [Tooltip("Amount to increase tick count after required number of collectibles collected")]
    [SerializeField] private float tickIncrease = 10f;

    [Tooltip("Duration in seconds to fast forward to the new time")]
    [SerializeField] private float fastForwardDuration = 1f;

    [Tooltip("Sound effect to play while fast-forwarding")]
    [SerializeField] private AudioClip fastForwardSFX;

    private AudioSource audioSource;
    private int collectiblesCollected = 0;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = fastForwardSFX;
        audioSource.loop = true;

        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        if (collectibleManager != null)
        {
            collectibleManager.OnCollectiblePicked += IncrementCollectiblesCollected;
        }
    }

    private void IncrementCollectiblesCollected()
    {
        collectiblesCollected++;
        if (collectiblesCollected % collectiblesRequired == 0)
        {
            StartCoroutine(IncreaseTickCount());
        }
    }

    private IEnumerator IncreaseTickCount()
    {
        CozyWeather time = FindObjectOfType<CozyWeather>();
        if (time != null)
        {
            float startTime = Time.time;
            float startTicks = time.currentTicks;
            float endTicks = startTicks + tickIncrease;

            audioSource.Play();

            while (Time.time < startTime + fastForwardDuration)
            {
                float t = (Time.time - startTime) / fastForwardDuration;
                time.currentTicks = Mathf.Lerp(startTicks, endTicks, t);
                yield return null;
            }

            time.currentTicks = endTicks;
            audioSource.Stop();
        }
    }

    private void OnDestroy()
    {
        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        if (collectibleManager != null)
        {
            collectibleManager.OnCollectiblePicked -= IncrementCollectiblesCollected;
        }
    }
}
