using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SimpleSplashScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI pressAnyKeyText;

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private float mainCanvasFadeInDuration = 2f;

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
    }

    private void Update()
    {
        if (splashScreenActive && (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            StartCoroutine(FadeOutSplashScreen());

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                audioSource.PlayOneShot(mouseClickSFX, sfxVolume);
            }
            else
            {
                audioSource.PlayOneShot(keyPressSFX, sfxVolume);
            }
        }
    }

    private IEnumerator FadeOutSplashScreen()
    {
        splashScreenActive = false;

        float startTime = Time.time;
        float progress;
        Color textColor;

        while ((Time.time - startTime) < fadeDuration)
        {
            progress = (Time.time - startTime) / fadeDuration;

            textColor = titleText.color;
            textColor.a = Mathf.Lerp(1, 0, progress);
            titleText.color = textColor;

            textColor = pressAnyKeyText.color;
            textColor.a = Mathf.Lerp(1, 0, progress);
            pressAnyKeyText.color = textColor;

            yield return null;
        }

        textColor = titleText.color;
        textColor.a = 0;
        titleText.color = textColor;

        textColor = pressAnyKeyText.color;
        textColor.a = 0;
        pressAnyKeyText.color = textColor;

        // Call your method to fade in the main canvas and play the mainCanvasFadeInSFX here
        StartCoroutine(FadeInMainCanvas());
    }

    private IEnumerator FadeInMainCanvas()
    {
        // Play the main canvas fade in SFX
        audioSource.PlayOneShot(mainCanvasFadeInSFX, sfxVolume);

        // Get the CanvasGroup component of the main canvas
        CanvasGroup mainCanvasGroup = mainCanvas.GetComponent<CanvasGroup>();

        // Set the initial alpha value to 0
        mainCanvasGroup.alpha = 0;

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
    }
}
