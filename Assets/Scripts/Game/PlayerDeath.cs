using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PlayerDeath : MonoBehaviour
{
    [Tooltip("Death sound effect")]
    [SerializeField] private AudioClip deathSound;

    [Tooltip("Canvas group for the panel")]
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Tooltip("Canvas group for the text")]
    [SerializeField] private CanvasGroup textCanvasGroup;

    [Tooltip("You Died text component")]
    [SerializeField] private TMP_Text youDiedText;

    [Tooltip("Audio fade-out duration")]
    [SerializeField] private float audioFadeOutDuration = 1f;

    [Tooltip("Death sequence delay")]
    [SerializeField] private float deathDelay = 5f;

    [Tooltip("Fade duration for UI elements")]
    [SerializeField] private float fadeDuration = 1f;

    private AudioSource audioSource;
    private bool isDead = false;
    private Dictionary<AudioSource, float> initialAudioSourceVolumes;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && Input.GetKey(KeyCode.LeftControl))
        {
            TriggerDeathSequence();
        }
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

    private IEnumerator DeathSequence()
    {
        FadeAudioSourcesOut(audioFadeOutDuration);
        yield return new WaitForSeconds(audioFadeOutDuration);

        audioSource.volume = 0.66f;
        audioSource.PlayOneShot(deathSound);

        yield return StartCoroutine(FadeUIElement(panelCanvasGroup, 0, 1, fadeDuration));
        yield return StartCoroutine(FadeUIElement(textCanvasGroup, 0, 1, fadeDuration));
        yield return new WaitForSeconds(deathDelay);
        yield return StartCoroutine(FadeUIElement(textCanvasGroup, 1, 0, fadeDuration));

        panelCanvasGroup.alpha = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void FadeAudioSourcesOut(float duration)
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        initialAudioSourceVolumes = new Dictionary<AudioSource, float>();

        foreach (AudioSource source in allAudioSources)
        {
            initialAudioSourceVolumes.Add(source, source.volume);
            StartCoroutine(FadeAudioSource(source, source.volume, 0, duration));
        }
    }

    private IEnumerator FadeAudioSource(AudioSource source, float startVolume, float endVolume, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / duration);
            yield return null;
        }
    }

    private IEnumerator FadeUIElement(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeUIElement(panelCanvasGroup, 1, 0, fadeDuration));
        isDead = false; // Reset isDead flag
    }
}