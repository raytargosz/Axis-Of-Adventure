using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LocationTextController : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public float textFadeInDuration = 3f;
    public float displayDuration = 6f;
    public float textFadeOutDuration = 3f;
    public Image locationImage;
    public float imageFadeInDuration = 1.5f;
    public float imageMaxAlpha = 0.5f;
    public AudioClip locationSFX;
    public AudioSource audioSource;

    [Range(0, 1)]
    [Tooltip("Volume of the location SFX")]
    public float locationSFXVolume = 1f;

    private bool hasTriggered = false;

    public void TriggerLocationText(string locationName)
    {
        StartCoroutine(ShowLocationText(locationName));
    }

    private IEnumerator ShowLocationText(string locationName)
    {
        if (hasTriggered) yield break;

        hasTriggered = true;
        locationText.text = locationName;

        // Play SFX
        audioSource.PlayOneShot(locationSFX);

        // Fade in text and image
        StartCoroutine(FadeInText());
        StartCoroutine(FadeInImage());

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        float elapsedTime = 0f;
        while (elapsedTime < textFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / textFadeOutDuration);
            locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, alpha);
            locationImage.color = new Color(locationImage.color.r, locationImage.color.g, locationImage.color.b, alpha * imageMaxAlpha);
            yield return null;
        }
    }

    private IEnumerator FadeInText()
    {
        float elapsedTime = 0f;
        while (elapsedTime < textFadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / textFadeInDuration);
            locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeInImage()
    {
        float elapsedTime = 0f;
        while (elapsedTime < imageFadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, imageMaxAlpha, elapsedTime / imageFadeInDuration);
            locationImage.color = new Color(locationImage.color.r, locationImage.color.g, locationImage.color.b, alpha);
            yield return null;
        }
    }
}
