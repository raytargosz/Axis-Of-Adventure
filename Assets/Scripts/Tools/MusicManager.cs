/*
 * This script is for managing and playing background music in a game. It allows you to 
 * set weights for each track to control the probability of a track being played. 
 * Higher weights result in a higher probability of being played.
 *
 * To use this script:
 * 1. Attach this script to a GameObject with an AudioSource component.
 * 2. Assign the music tracks you want to play to the `musicTracks` array.
 * 3. Set the corresponding weights for each track in the `trackWeights` array.
 * 4. Ensure that the lengths of the `musicTracks` and `trackWeights` arrays are the same.
 * 5. Adjust the fadeInTime, fadeOutTime, and fixedTrackVolume as needed.
 *
 * The script will handle playing the music tracks in a shuffled order, with each track's 
 * probability of being played determined by its weight relative to the total weight of all tracks.
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Tooltip("Array of music tracks to be played.")]
    public AudioClip[] musicTracks;

    [Tooltip("Array of weights for each track. Higher weights result in a higher probability of being played.")]
    public float[] trackWeights;

    [Tooltip("Time in seconds for the music to fade in.")]
    public float fadeInTime = 2f;

    [Tooltip("Time in seconds for the music to fade out.")]
    public float fadeOutTime = 2f;

    [Tooltip("Fixed volume for each track.")]
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