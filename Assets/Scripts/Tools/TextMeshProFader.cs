using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextMeshProFader : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> textObjects;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float delayBetweenTexts = 1f;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float audioFadeDuration = 1f;
    [SerializeField] private float startDelay = 2f;

    private void Start()
    {
        StartCoroutine(StartTextFadeSequence());

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


    private IEnumerator StartTextFadeSequence()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(FadeInOutTextSequence());
    }


    private IEnumerator FadeInOutTextSequence()
    {
        foreach (TMP_Text text in textObjects)
        {
            yield return FadeText(text, 0, 1, fadeInDuration);
            yield return new WaitForSeconds(displayDuration);
            yield return FadeText(text, 1, 0, fadeOutDuration);

            if (text != textObjects[textObjects.Count - 1])
            {
                yield return new WaitForSeconds(delayBetweenTexts);
            }
        }

        yield return FadeOutAllAudio(audioFadeDuration);
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeText(TMP_Text text, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        Color startColor = text.color;
        startColor.a = startAlpha;
        Color endColor = text.color;
        endColor.a = endAlpha;

        while (elapsedTime < duration)
        {
            text.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.color = endColor;
    }

    private IEnumerator FadeOutAllAudio(float duration)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        float elapsedTime = 0f;
        float[] initialVolumes = new float[audioSources.Length];

        for (int i = 0; i < audioSources.Length; i++)
        {
            initialVolumes[i] = audioSources[i].volume;
        }

        while (elapsedTime < duration)
        {
            for (int i = 0; i < audioSources.Length; i++)
            {
                audioSources[i].volume = Mathf.Lerp(initialVolumes[i], 0, elapsedTime / duration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}