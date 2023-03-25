using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioOnce : MonoBehaviour
{
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private float delayInSeconds = 1.0f;
    [Range(0, 1)][SerializeField] private float volumeReductionPercentage = 0.5f;

    private AudioSource audioSource;
    private List<AudioSource> otherAudioSources;
    private Dictionary<AudioSource, float> originalVolumes;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = audioClip;

        // Find all the AudioSources in the scene
        otherAudioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
        originalVolumes = new Dictionary<AudioSource, float>();

        // Remove the current AudioSource from the list and store original volumes
        for (int i = otherAudioSources.Count - 1; i >= 0; i--)
        {
            if (otherAudioSources[i] == audioSource)
            {
                otherAudioSources.RemoveAt(i);
            }
            else
            {
                originalVolumes[otherAudioSources[i]] = otherAudioSources[i].volume;
            }
        }

        if (!PlayerPrefs.HasKey("AudioPlayed"))
        {
            StartCoroutine(PlayAudioWithDelay(delayInSeconds));
        }
    }

    private IEnumerator PlayAudioWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reduce the volume of other AudioSources
        foreach (AudioSource otherAudioSource in otherAudioSources)
        {
            otherAudioSource.volume *= (1 - volumeReductionPercentage);
        }

        audioSource.Play();

        // Wait for the audio clip to finish
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        // Restore the volume of other AudioSources over a period of three seconds
        StartCoroutine(RestoreVolumes(3f));

        PlayerPrefs.SetInt("AudioPlayed", 1);
    }

    private IEnumerator RestoreVolumes(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            foreach (AudioSource otherAudioSource in otherAudioSources)
            {
                float targetVolume = originalVolumes[otherAudioSource];
                otherAudioSource.volume = Mathf.Lerp(otherAudioSource.volume, targetVolume, timer / duration);
            }

            yield return null;
        }
    }
}