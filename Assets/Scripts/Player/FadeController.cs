using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    public string targetScene;

    private bool fadingIn = true;
    private bool fadingOut = false;
    private bool insideTrigger = false;


    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.enabled = true;
            fadeImage.color = new Color(0, 0, 0, 1);
        }
    }

    private void Update()
    {
        if (fadingIn)
        {
            StartCoroutine(FadeIn());
            fadingIn = false;
        }
        else if (fadingOut)
        {
            StartCoroutine(FadeOut());
            fadingOut = false;
        }
    }


    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        Color currentColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            currentColor.a = alpha;
            fadeImage.color = currentColor;
            yield return null;
        }

        currentColor.a = 0f;
        fadeImage.color = currentColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            insideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            insideTrigger = false;
        }
    }

    public IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color currentColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            currentColor.a = alpha;
            fadeImage.color = currentColor;
            yield return null;
        }

        currentColor.a = 1f;
        fadeImage.color = currentColor;

        // Load the target scene after the fade-out is complete
        SceneManager.LoadScene(targetScene);
    }

    public void StartFadeOut(string targetScene)
    {
        this.targetScene = targetScene;
        fadingOut = true;
    }
}