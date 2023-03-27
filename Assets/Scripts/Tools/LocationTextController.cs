using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocationTextController : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI locationText;
    public Image locationImage;

    [Header("Timings")]
    public float textFadeInDuration = 3f;
    public float displayDuration = 6f;
    public float textFadeOutDuration = 3f;
    public float imageFadeInDuration = 1.5f;

    [Header("Image Settings")]
    [Tooltip("Maximum alpha value for the location image")]
    public float imageMaxAlpha = 0.5f;

    [Header("Audio")]
    public AudioClip locationSFX;
    public AudioSource audioSource;
    [Range(0, 1)]
    [Tooltip("Volume of the location SFX")]
    public float locationSFXVolume = 1f;

    private bool hasTriggered = false;

    void Start()
    {
        // Initialize location text and image alpha values to 0
        SetTextAlpha(0f);
        SetImageAlpha(0f);
    }

    public void TriggerLocationText(string locationName)
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(ShowLocationText(locationName));
        }
    }

    private IEnumerator ShowLocationText(string locationName)
    {
        locationText.text = locationName;

        // Play SFX
        audioSource.PlayOneShot(locationSFX);

        // Fade in text and image
        StartCoroutine(FadeTextAlpha(0f, 1f, textFadeInDuration));
        StartCoroutine(FadeImageAlpha(0f, imageMaxAlpha, imageFadeInDuration));

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out text and image
        StartCoroutine(FadeTextAlpha(1f, 0f, textFadeOutDuration));
        StartCoroutine(FadeImageAlpha(imageMaxAlpha, 0f, textFadeOutDuration));

        yield return new WaitForSeconds(textFadeOutDuration);

        // Reset trigger
        hasTriggered = false;
    }

    private IEnumerator FadeTextAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetTextAlpha(alpha);
            yield return null;
        }
    }

    private IEnumerator FadeImageAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            SetImageAlpha(alpha);
            yield return null;
        }
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = locationText.color;
        locationText.color = new Color(color.r, color.g, color.b, alpha);
    }

    private void SetImageAlpha(float alpha)
    {
        Color color = locationImage.color;
        locationImage.color = new Color(color.r, color.g, color.b, alpha);
    }
}