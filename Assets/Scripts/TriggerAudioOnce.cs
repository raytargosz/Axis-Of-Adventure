using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AudioSource))]
public class TriggerAudioOnce : MonoBehaviour
{
    [Header("Volume Dimming Settings")]
    public float dimPercentage = 90f;
    public float rampBackTime = 3f;

    private AudioSource audioSource;
    private bool audioPlayed = false;
    private List<AudioSource> otherAudioSources;
    private Dictionary<AudioSource, float> originalVolumes;
    private static Queue<AudioSource> audioQueue = new Queue<AudioSource>();
    private static bool isPlaying = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        otherAudioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        otherAudioSources.Remove(audioSource);
        originalVolumes = new Dictionary<AudioSource, float>();

        foreach (AudioSource src in otherAudioSources)
        {
            originalVolumes[src] = src.volume;
        }
    }

    public Dictionary<AudioSource, float> GetOriginalVolumes()
    {
        return originalVolumes;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!audioPlayed && other.CompareTag("Player"))
        {
            audioQueue.Enqueue(audioSource);
            audioPlayed = true;
            if (!isPlaying)
            {
                StartCoroutine(PlayAudioQueue());
            }
        }
    }

    private IEnumerator PlayAudioQueue()
    {
        isPlaying = true;
        while (audioQueue.Count > 0)
        {
            AudioSource currentAudio = audioQueue.Dequeue();
            currentAudio.Play();
            StartCoroutine(DimOtherAudio(currentAudio));

            yield return new WaitForSeconds(currentAudio.clip.length + 2f); // Wait for the clip to finish playing and add 2 seconds delay
        }
        isPlaying = false;
    }

    private IEnumerator DimOtherAudio(AudioSource currentAudio)
    {
        foreach (AudioSource src in otherAudioSources)
        {
            if (src != currentAudio) // Exclude the currentAudio from being dimmed
            {
                StartCoroutine(DimAudioSource(src));
            }
        }

        yield return new WaitForSeconds(currentAudio.clip.length);

        // Check if the player has collected all items
        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        if (collectibleManager != null && !collectibleManager.HasRequiredPickupAmountReached())
        {
            foreach (AudioSource src in otherAudioSources)
            {
                if (src != currentAudio) // Exclude the currentAudio from being ramped back
                {
                    StartCoroutine(RampBackAudioSource(src));
                }
            }
        }
        else
        {
            // Fade out the current audio source when the player has collected all items
            StartCoroutine(DimAudioSource(currentAudio));
        }
    }

    private IEnumerator DimAudioSource(AudioSource src)
    {
        float elapsedTime = 0f;
        float currentVolume = src.volume;
        float targetVolume = originalVolumes[src] * (1f - dimPercentage / 100f);

        while (elapsedTime < rampBackTime)
        {
            elapsedTime += Time.deltaTime;
            src.volume = Mathf.Lerp(currentVolume, targetVolume, elapsedTime / rampBackTime);
            yield return null;
        }
    }

    private IEnumerator RampBackAudioSource(AudioSource src)
    {
        float elapsedTime = 0f;
        float currentVolume = src.volume;
        float targetVolume = originalVolumes[src];

        while (elapsedTime < rampBackTime)
        {
            elapsedTime += Time.deltaTime;
            src.volume = Mathf.Lerp(currentVolume, targetVolume, elapsedTime / rampBackTime);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();

        foreach (AudioSource src in otherAudioSources)
        {
            if (src != null)
            {
                src.volume = originalVolumes[src];
            }
        }
    }
}