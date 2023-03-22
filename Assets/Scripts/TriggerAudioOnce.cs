using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AudioSource))]
public class TriggerAudioOnce : MonoBehaviour
{
    [Header("Volume Dimming Settings")]
    [Tooltip("Percentage of volume reduction for other audio sources.")]
    public float dimPercentage = 90f;
    [Tooltip("Time in seconds to ramp back the volume of other audio sources.")]
    public float rampBackTime = 3f;

    [Header("Audio Delay Settings")]
    [Tooltip("The delay in seconds before the audio starts playing.")]
    public float audioStartDelay = 1f;

    [Header("Unique ID")]
    [Tooltip("Unique identifier for each audio trigger.")]
    [SerializeField] private string uniqueID;

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
            if (PlayerPrefs.GetInt(uniqueID, 0) == 0)
            {
                audioQueue.Enqueue(audioSource);
                audioPlayed = true;
                PlayerPrefs.SetInt(uniqueID, 1);
                PlayerPrefs.Save();
                if (!isPlaying)
                {
                    StartCoroutine(PlayAudioQueue());
                }
            }
        }
    }

    private IEnumerator PlayAudioQueue()
    {
        isPlaying = true;
        while (audioQueue.Count > 0)
        {
            AudioSource currentAudio = audioQueue.Dequeue();
            yield return new WaitForSeconds(audioStartDelay); // Wait for the delay time
            currentAudio.Play();
            StartCoroutine(DimOtherAudioWithDelay(currentAudio)); // Call the new method here

            yield return new WaitForSeconds(currentAudio.clip.length + 2f); // Wait for the clip to finish playing and add 2 seconds delay
        }
        isPlaying = false;
    }

    private IEnumerator DimOtherAudio(AudioSource currentAudio)
    {
        foreach (AudioSource src in otherAudioSources)
        {
            if (src != currentAudio)
            {
                StartCoroutine(DimAudioSource(src));
            }
        }

        yield return new WaitForSeconds(currentAudio.clip.length);

        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        if (collectibleManager != null && !collectibleManager.HasRequiredPickupAmountReached())
        {
            foreach (AudioSource src in otherAudioSources)
            {
                if (src != currentAudio)
                {
                    StartCoroutine(RampBackAudioSource(src));
                }
            }
        }
        else
        {
            StartCoroutine(DimAudioSource(currentAudio));
        }
    }

    private IEnumerator DimOtherAudioWithDelay(AudioSource currentAudio)
    {
        // Add a small delay to let the scene's music start playing
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DimOtherAudio(currentAudio));
    }

    private IEnumerator DimAudioSource(AudioSource src)
    {
        float elapsedTime = 0f;
        float currentVolume = src.volume;
        float targetVolume = originalVolumes[src] * (1f - dimPercentage / 100f);

        while (elapsedTime < rampBackTime)
        {
            if (src == null)
            {
                break;
            }

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
            if (src == null)
            {
                break;
            }

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