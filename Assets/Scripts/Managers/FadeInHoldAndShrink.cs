using UnityEngine;
using TMPro;
using System.Collections;

public class FadeInHoldAndShrink : MonoBehaviour
{
    [Header("UI Element")]
    [Tooltip("TextMeshPro UI Element")]
    public TextMeshProUGUI textElement;
    [Tooltip("Press V to View Riddle Text Element")]
    public TextMeshProUGUI pressVTextElement;

    [Header("Animation Settings")]
    [Tooltip("Fade in duration")]
    public float fadeInDuration = 1f;
    [Tooltip("Hold duration")]
    public float holdDuration = 2f;
    [Tooltip("Fade out duration")]
    public float fadeOutDuration = 1f;
    [Tooltip("Hold duration for Press V Text")]
    public float pressVHoldDuration = 3f;

    private bool playerInRange = false;
    private bool triggerUsed = false;

    private void Start()
    {
        textElement.gameObject.SetActive(false);
        pressVTextElement.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && !triggerUsed)
        {
            StartCoroutine(AnimateTextElement());
            playerInRange = false;
            triggerUsed = true;
        }

        if (triggerUsed && Input.GetKey(KeyCode.V))
        {
            Color textColor = textElement.color;
            textColor.a = 1f;
            textElement.color = textColor;
            textElement.gameObject.SetActive(true);
        }
        else if (triggerUsed && Input.GetKeyUp(KeyCode.V))
        {
            Color textColor = textElement.color;
            textColor.a = 0f;
            textElement.color = textColor;
            textElement.gameObject.SetActive(false);
        }
    }

    private IEnumerator AnimateTextElement()
    {
        textElement.gameObject.SetActive(true);

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

        // Fade out
        float fadeOutElapsed = 0f;
        while (fadeOutElapsed < fadeOutDuration)
        {
            fadeOutElapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, fadeOutElapsed / fadeOutDuration);
            textElement.color = textColor;
            yield return null;
        }

        textColor.a = 0f;
        textElement.color = textColor;
        textElement.gameObject.SetActive(false);

        // Show "Press V To View Riddle" text
        pressVTextElement.gameObject.SetActive(true);
        Color pressVTextColor = pressVTextElement.color;
        pressVTextColor.a = 1f;
        pressVTextElement.color = pressVTextColor;
        yield return new WaitForSeconds(pressVHoldDuration);

        // Fade out "Press V To View Riddle" text
        float pressVFadeOutElapsed = 0f;
        while (pressVFadeOutElapsed < fadeInDuration)
        {
            pressVFadeOutElapsed += Time.deltaTime;
            pressVTextColor.a = Mathf.Lerp(1f, 0f, pressVFadeOutElapsed / fadeInDuration);
            pressVTextElement.color = pressVTextColor;
            yield return null;
        }

        pressVTextColor.a = 0f;
        pressVTextElement.color = pressVTextColor;
        pressVTextElement.gameObject.SetActive(false);
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