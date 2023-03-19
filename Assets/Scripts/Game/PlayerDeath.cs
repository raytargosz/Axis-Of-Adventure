using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private CanvasGroup textCanvasGroup;
    [SerializeField] private TMP_Text youDiedText;
    [SerializeField] private float audioFadeOutDuration = 1f;
    [SerializeField] private float deathDelay = 5f;
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource audioSource;
    private bool isDead = false;
    private Dictionary<AudioSource, float> initialAudioSourceVolumes;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isDead = false; // Reset isDead flag
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void TriggerDeathSequence()
    {
        if (!isDead)
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    public void HandlePlayerDeath()
    {
        if (!isDead)
        {
            isDead = true;
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        // Fade out other audio sources
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        initialAudioSourceVolumes = new Dictionary<AudioSource, float>();

        foreach (AudioSource source in allAudioSources)
        {
            initialAudioSourceVolumes.Add(source, source.volume);
        }

        float elapsedTime = 0f;
        while (elapsedTime < audioFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float fadeProgress = elapsedTime / audioFadeOutDuration;
            foreach (AudioSource source in allAudioSources)
            {
                source.volume = Mathf.Lerp(initialAudioSourceVolumes[source], 0, fadeProgress);
            }
            yield return null;
        }

        audioSource.volume = 0.66f;
        audioSource.PlayOneShot(deathSound);

        // Fade in panel
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            Debug.Log("Panel fade-in progress: " + panelCanvasGroup.alpha);
            yield return null;
        }

        // Fade in text
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            Debug.Log("Text fade-in progress: " + textCanvasGroup.alpha);
            yield return null;
        }

        yield return new WaitForSeconds(deathDelay);

        // Fade out text
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            Debug.Log("Text fade-out progress: " + textCanvasGroup.alpha);
            yield return null;
        }

        // Set the panel alpha to 1
        panelCanvasGroup.alpha = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadePanelAfterSceneLoad());
    }

    private IEnumerator FadePanelAfterSceneLoad()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            Debug.Log("Panel fade-out progress: " + panelCanvasGroup.alpha);
            yield return null;
        }

        isDead = false; // Reset isDead flag
    }
}