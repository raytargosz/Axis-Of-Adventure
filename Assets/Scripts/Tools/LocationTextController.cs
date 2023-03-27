using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

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

    public float overallTextFadeInDuration = 3f;

    private bool hasTriggered = false;

    void Start()
    {
        // Set initial alpha values for locationText and locationImage
        SetTextAlpha(0f);
        SetImageAlpha(0f);
    }

    private void SetTextAlpha(float alpha)
    {
        for (int i = 0; i < locationText.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = locationText.textInfo.characterInfo[i];
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Color32[] vertexColors = locationText.textInfo.meshInfo[materialIndex].colors32;
            Color32 c = vertexColors[vertexIndex];

            for (int j = 0; j < 4; j++)
            {
                vertexColors[vertexIndex + j] = new Color32(c.r, c.g, c.b, (byte)(alpha * 255));
            }

            locationText.textInfo.meshInfo[materialIndex].colors32 = vertexColors;
        }

        locationText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void SetImageAlpha(float alpha)
    {
        locationImage.color = new Color(locationImage.color.r, locationImage.color.g, locationImage.color.b, alpha);
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
        StartCoroutine(FadeInText());
        StartCoroutine(FadeInImageCoroutine());

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        StartCoroutine(FadeOutText());
        StartCoroutine(FadeOutImageCoroutine());
    }

    private IEnumerator FadeInText()
    {
        int totalCharacters = locationText.textInfo.characterCount;
        float timeBetweenFades = overallTextFadeInDuration / totalCharacters;
        List<int> indices = Enumerable.Range(0, totalCharacters).ToList();
        indices = indices.OrderBy(x => Random.Range(0, totalCharacters)).ToList();

        Color32[] newVertexColors;
        foreach (int index in indices)
        {
            float elapsedTime = 0f;
            float singleCharacterFadeDuration = timeBetweenFades;
            TMP_CharacterInfo charInfo = locationText.textInfo.characterInfo[index];
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] vertexColors = locationText.textInfo.meshInfo[materialIndex].colors32;
            newVertexColors = (Color32[])vertexColors.Clone();

            while (elapsedTime < singleCharacterFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / singleCharacterFadeDuration);
                Color32 c = newVertexColors[vertexIndex];

                for (int i = 0; i < 4; i++)
                {
                    newVertexColors[vertexIndex + i] = new Color32(c.r, c.g, c.b, (byte)(alpha * 255));
                }

                locationText.textInfo.meshInfo[materialIndex].colors32 = newVertexColors;
                locationText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }
        }
    }

    private IEnumerator FadeOutText()
    {
        int totalCharacters = locationText.textInfo.characterCount;
        float timeBetweenFades = overallTextFadeInDuration / totalCharacters;
        List<int> indices = Enumerable.Range(0, totalCharacters).ToList();
        indices = indices.OrderBy(x => Random.Range(0, totalCharacters)).ToList();

        Color32[] newVertexColors;
        foreach (int index in indices)
        {
            float elapsedTime = 0f;
            float singleCharacterFadeDuration = timeBetweenFades;
            TMP_CharacterInfo charInfo = locationText.textInfo.characterInfo[index];
            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] vertexColors = locationText.textInfo.meshInfo[materialIndex].colors32;
            newVertexColors = (Color32[])vertexColors.Clone();

            while (elapsedTime < singleCharacterFadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / singleCharacterFadeDuration);
                Color32 c = newVertexColors[vertexIndex];

                for (int i = 0; i < 4; i++)
                {
                    newVertexColors[vertexIndex + i] = new Color32(c.r, c.g, c.b, (byte)(alpha * 255));
                }

                locationText.textInfo.meshInfo[materialIndex].colors32 = newVertexColors;
                locationText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                yield return null;
            }
        }
    }

    private IEnumerator FadeInImageCoroutine()
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

    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    private IEnumerator FadeOutImageCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < textFadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(imageMaxAlpha, 0, elapsedTime / textFadeOutDuration);
            locationImage.color = new Color(locationImage.color.r, locationImage.color.g, locationImage.color.b, alpha);
            yield return null;
        }
        ResetTrigger();
    }
}