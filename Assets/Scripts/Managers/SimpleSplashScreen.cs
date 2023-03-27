using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SimpleSplashScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;
    [SerializeField] private CanvasGroup mainCanvasGroup;

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float mainCanvasFadeInDuration = 1f;
    [SerializeField] private float delayBetweenFadeOutAndIn = 1f;

    [Header("SFX")]
    [SerializeField] private AudioClip mouseClickSFX;
    [SerializeField] private AudioClip keyPressSFX;
    [SerializeField] private AudioClip mainCanvasFadeInSFX;
    [Range(0, 1)][SerializeField] private float sfxVolume = 1f;

    private AudioSource audioSource;
    private bool splashScreenActive = true;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = false; // Ensure the AudioSource is not muted
        mainCanvasGroup.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (splashScreenActive && (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            splashScreenActive = false;

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                audioSource.PlayOneShot(mouseClickSFX, sfxVolume);
            }
            else
            {
                audioSource.PlayOneShot(keyPressSFX, sfxVolume);
            }

            StartCoroutine(FadeOutSplashScreenAndEnableMainCanvas());
        }
    }

    private IEnumerator FadeOutSplashScreenAndEnableMainCanvas()
    {
        yield return StartCoroutine(FadeOutSplashScreen());
        yield return new WaitForSeconds(delayBetweenFadeOutAndIn);
        EnableMainCanvas();
    }

    private void EnableMainCanvas()
    {
        mainCanvasGroup.gameObject.SetActive(true);
        mainCanvasGroup.alpha = 0;
        mainCanvasGroup.interactable = false;
        mainCanvasGroup.blocksRaycasts = false;

        // Play the main canvas fade in SFX
        audioSource.PlayOneShot(mainCanvasFadeInSFX, sfxVolume);

        StartCoroutine(FadeInMainCanvas());
    }

    private IEnumerator FadeOutSplashScreen()
    {
        float startTime = Time.time;
        float progress;

        while ((Time.time - startTime) < fadeDuration)
        {
            progress = (Time.time - startTime) / fadeDuration;
            titleCanvasGroup.alpha = Mathf.Lerp(1, 0, progress);
            pressAnyKeyText.alpha = Mathf.Lerp(1, 0, progress);
            yield return null;
        }

        titleCanvasGroup.alpha = 0;
        pressAnyKeyText.alpha = 0;
    }

    private IEnumerator FadeInMainCanvas()
    {
        float elapsedTime = 0f;
        while (elapsedTime < mainCanvasFadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / mainCanvasFadeInDuration;
            mainCanvasGroup.alpha = Mathf.Lerp(0, 1, progress);
            yield return null;
        }

        // Make sure the final alpha value is set to 1
        mainCanvasGroup.alpha = 1;

        // Make the main canvas interactable after the fade-in is complete
        mainCanvasGroup.interactable = true;
        mainCanvasGroup.blocksRaycasts = true;
    }
}