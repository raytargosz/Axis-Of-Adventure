using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeOutImage : MonoBehaviour
{
    public float fadeDuration = 1f;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float startTime = Time.time;
        Color imageColor = image.color;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            image.color = new Color(imageColor.r, imageColor.g, imageColor.b, 1 - t);
            yield return null;
        }
        image.color = new Color(imageColor.r, imageColor.g, imageColor.b, 0);
    }
}
