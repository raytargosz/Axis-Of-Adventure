using System.Collections;
using UnityEngine;
using TMPro; 
using UnityEngine.UI;

public class LocationTextController : MonoBehaviour
{
    public TextMeshProUGUI locationText;
    public float fadeInDuration = 3f;
    public float displayDuration = 6f;
    public float fadeOutDuration = 3f;
    public Image locationImage;
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

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, alpha);
            locationImage.color = new Color(locationImage.color.r, locationImage.color.g, locationImage.color.b, alpha);
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            locationText.color = new Color(locationText.color.r, locationText.color.g, locationText.color.b, alpha);
            locationImage.color = new Color(locationImage.color.r, locationImage.color.g, locationImage.color.b, alpha);
            yield return null;
        }
    }
}
