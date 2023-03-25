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
        InitializeComponents();
    }

    private void InitializeComponents()
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

    private void OnTriggerEnter(Collider other)
    {
        if (!audioPlayed && other.CompareTag("Player") && PlayerPrefs.GetInt(uniqueID, 0) == 0)
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

    private IEnumerator PlayAudioQueue()
    {
        isPlaying = true;
        while (audioQueue.Count > 0)
        {
            AudioSource currentAudio = audioQueue.Dequeue();
            yield return new WaitForSeconds(audioStartDelay);
            currentAudio.Play();
            StartCoroutine(DimOtherAudioWithDelay(currentAudio));
            yield return new WaitForSeconds(currentAudio.clip.length + 2f);
        }
        isPlaying = false;
    }

    private IEnumerator DimOtherAudioWithDelay(AudioSource currentAudio)
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(DimOtherAudio(currentAudio));
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

        foreach (AudioSource src in otherAudioSources)
        {
            if (src != currentAudio)
            {
                StartCoroutine(RampBackAudioSource(src));
            }
        }
    }

    private IEnumerator DimAudioSource(AudioSource src)
    {
        yield return ChangeAudioSourceVolume(src, originalVolumes[src] * (1f - dimPercentage / 100f), rampBackTime);
    }

    private IEnumerator RampBackAudioSource(AudioSource src)
    {
        yield return ChangeAudioSourceVolume(src, originalVolumes[src], rampBackTime);
    }

    private IEnumerator ChangeAudioSourceVolume(AudioSource src, float targetVolume, float duration)
    {
        if (src == null) yield break;

        float elapsedTime = 0f;
        float currentVolume = src.volume;

        while (elapsedTime < duration)
        {
            src.volume = Mathf.Lerp(currentVolume, targetVolume, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
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