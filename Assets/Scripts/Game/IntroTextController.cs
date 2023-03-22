using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroTextController : MonoBehaviour
{
    public TextMeshProUGUI[] textLines;
    public float fadeInTime = 2f;
    public float holdTime = 2f;
    public float fadeOutTime = 2f;
    public float preFadeInDelay = 1f;
    public float postFadeOutDelay = 1f;
    public float timeBetweenTextLines = 1f;
    public string nextSceneName = "NextScene";
    public AudioClip fadeInSound;
    public AudioClip fadeOutSound;
    public AudioSource audioSource;
    public AudioSource backgroundAudioSource;
    public float fadeInSoundVolume = 1f;
    public float fadeOutSoundVolume = 1f;
    public float backgroundSoundVolume = 0.5f;

    private AudioSource[] sceneAudioSources;
    private Dictionary<AudioSource, float> originalVolumes;

    private void Start()
    {
        sceneAudioSources = FindObjectsOfType<AudioSource>();
        originalVolumes = new Dictionary<AudioSource, float>();

        foreach (AudioSource source in sceneAudioSources)
        {
            originalVolumes[source] = source.volume;
        }

        backgroundAudioSource.volume = backgroundSoundVolume;
        StartCoroutine(FadeInOut());

        // Hide the cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftControl))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator FadeInOut()
    {
        yield return new WaitForSeconds(preFadeInDelay);

        bool introSoundPlayed = false;
        for (int i = 0; i < textLines.Length; i++)
        {
            TextMeshProUGUI textMeshPro = textLines[i];
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0);

            if (audioSource != null && fadeInSound != null && !introSoundPlayed)
            {
                audioSource.PlayOneShot(fadeInSound, fadeInSoundVolume);
                introSoundPlayed = true;
            }

            float startTime = Time.time;
            while (Time.time < startTime + fadeInTime)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    break;
                }

                float t = (Time.time - startTime) / fadeInTime;
                textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, t);
                yield return null;
            }

            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1);
            startTime = Time.time;
            while (Time.time < startTime + holdTime)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    break;
                }
                yield return null;
            }

            if (i == textLines.Length - 1 && audioSource != null && fadeOutSound != null)
            {
                audioSource.PlayOneShot(fadeOutSound, fadeOutSoundVolume);
            }

            startTime = Time.time;
            while (Time.time < startTime + fadeOutTime)
            {
                float t = 1 - (Time.time - startTime) / fadeOutTime;
                textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, t);
                yield return null;
            }

            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0);
            yield return new WaitForSeconds(timeBetweenTextLines);

        if (i == textLines.Length - 1)
            {
                // Fade out all audio sources in the scene, except the one playing the fade out sound
                startTime = Time.time;
                while (Time.time < startTime + fadeOutTime)
                {
                    float t = 1 - (Time.time - startTime) / fadeOutTime;
                    foreach (AudioSource source in sceneAudioSources)
                    {
                        // Check if the audio source is not the one playing the fade out sound
                        if (source != null && source != audioSource)
                        {
                            source.volume = originalVolumes[source] * t; 
                        }
                    }
                    yield return null;
                }
            }
        }

        yield return new WaitForSeconds(postFadeOutDelay);
        SceneManager.LoadScene(nextSceneName);
    }
}