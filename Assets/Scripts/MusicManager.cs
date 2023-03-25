using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    public float[] trackWeights; // Add this array to store the weights for each track
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;
    public float fixedTrackVolume = 1f;

    private AudioSource audioSource;
    private List<AudioClip> playlist;

    void Start()
    {
        if (musicTracks.Length != trackWeights.Length)
        {
            Debug.LogError("Music tracks and track weights arrays must have the same length.");
            return;
        }

        audioSource = GetComponent<AudioSource>();
        playlist = new List<AudioClip>(musicTracks);
        StartCoroutine(PlayShuffledMusic());
    }

    private IEnumerator PlayShuffledMusic()
    {
        while (true)
        {
            if (playlist.Count == 0)
            {
                playlist.AddRange(musicTracks);
            }

            int randomIndex = GetRandomWeightedIndex(trackWeights);
            AudioClip selectedClip = playlist[randomIndex];
            playlist.RemoveAt(randomIndex);

            audioSource.clip = selectedClip;
            audioSource.Play();

            StartCoroutine(FadeIn(fadeInTime));

            yield return new WaitForSeconds(selectedClip.length - fadeOutTime);

            StartCoroutine(FadeOut(fadeOutTime));
            yield return new WaitForSeconds(fadeOutTime);
        }
    }

    private int GetRandomWeightedIndex(float[] weights)
    {
        float totalWeight = 0;
        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float currentWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            currentWeight += weights[i];
            if (randomValue < currentWeight)
            {
                return i;
            }
        }

        return weights.Length - 1;
    }

    private IEnumerator FadeIn(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, fixedTrackVolume, elapsedTime / time);
            yield return null;
        }
    }

    private IEnumerator FadeOut(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(fixedTrackVolume, 0f, elapsedTime / time);
            yield return null;
        }
    }
}