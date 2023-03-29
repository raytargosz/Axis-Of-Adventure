using UnityEngine;
using TMPro;
using System.Collections;

public class FadeInHoldAndShrink : MonoBehaviour
{
    [Header("UI Element")]
    [Tooltip("TextMeshPro UI Element")]
    public TextMeshProUGUI textElement;

    [Header("Animation Settings")]
    [Tooltip("Fade in duration")]
    public float fadeInDuration = 1f;
    [Tooltip("Hold duration")]
    public float holdDuration = 2f;
    [Tooltip("Shrink duration")]
    public float shrinkDuration = 1f;
    [Tooltip("Target scale")]
    public Vector3 targetScale = new Vector3(0.5f, 0.5f, 0.5f);
    [Tooltip("Target screen position")]
    public Vector2 targetScreenPosition = new Vector2(100, 100);

    private bool playerInRange = false;
    private bool triggerUsed = false;

    private void Start()
    {
        textElement.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !triggerUsed)
        {
            StartCoroutine(AnimateTextElement());
            playerInRange = false;
            triggerUsed = true;
        }
    }

    private IEnumerator AnimateTextElement()
    {
        textElement.gameObject.SetActive(true);
        RectTransform rectTransform = textElement.GetComponent<RectTransform>();
        Vector3 initialPosition = rectTransform.anchoredPosition;
        Vector3 initialScale = rectTransform.localScale;

        // Fade in
        float fadeInElapsed = 0f;
        Color textColor = textElement.color;
        textColor.a = 0f;
        textElement.color = textColor;

        while (fadeInElapsed < fadeInDuration)
        {
            fadeInElapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(0f, 1f, fadeInElapsed / fadeInDuration);
            textElement.color = textColor;
            yield return null;
        }

        textColor.a = 1f;
        textElement.color = textColor;

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Shrink and move to target position
        float shrinkElapsed = 0f;
        while (shrinkElapsed < shrinkDuration)
        {
            shrinkElapsed += Time.deltaTime;
            float t = shrinkElapsed / shrinkDuration;

            rectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetScreenPosition, t);
            rectTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);

            yield return null;
        }

        rectTransform.anchoredPosition = targetScreenPosition;
        rectTransform.localScale = targetScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !triggerUsed)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !triggerUsed)
        {
            playerInRange = false;
        }
    }
}