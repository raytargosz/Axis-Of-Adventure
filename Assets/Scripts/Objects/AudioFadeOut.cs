using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFadeOut : MonoBehaviour
{
    public float fadeDuration = 2.5f;

    private List<AudioSource> audioSources;

    private void Start()
    {
        audioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
    }

    public void FadeOutAllAudio()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            StartCoroutine(FadeOutAudioSource(audioSource, fadeDuration));
        }
    }

    private IEnumerator FadeOutAudioSource(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }
}