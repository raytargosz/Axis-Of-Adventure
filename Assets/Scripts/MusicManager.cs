using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    public float fadeInTime = 2f;
    public float fadeOutTime = 2f;
    public float fixedTrackVolume = 1f; // Add this line to set a fixed track volume

    private AudioSource audioSource;
    private List<AudioClip> playlist;

    void Start()
    {
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

            int randomIndex = Random.Range(0, playlist.Count);
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

    private IEnumerator FadeIn(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, fixedTrackVolume, elapsedTime / time); // Apply the fixedTrackVolume here
            yield return null;
        }
    }

    private IEnumerator FadeOut(float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(fixedTrackVolume, 0f, elapsedTime / time); // Apply the fixedTrackVolume here
            yield return null;
        }
    }
}