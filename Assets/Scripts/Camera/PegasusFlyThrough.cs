using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PegasusFlyThrough : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Tag to identify the target object that enters the trigger")]
    public string targetTag = "TargetCamera";

    [Header("Canvas Image Settings")]
    [Tooltip("Canvas Image to fade in when the camera enters the trigger")]
    public Image canvasImage;
    [Tooltip("Fade in speed for the canvas image (higher value is faster)")]
    public float fadeInSpeed = 1f;

    [Header("Activation Settings")]
    [Tooltip("GameObjects to enable when the fade in is complete")]
    public GameObject[] objectsToEnable;

    [Tooltip("GameObjects to disable when the fade in is complete")]
    public GameObject[] objectsToDisable;

    private bool fadeInCompleted = false;

    void OnTriggerEnter(Collider other)
    {
        if (!fadeInCompleted && other.CompareTag(targetTag))
        {
            StartCoroutine(FadeInCanvasImage());
        }
    }

    private IEnumerator FadeInCanvasImage()
    {
        Color currentColor = canvasImage.color;
        float alpha = currentColor.a;

        while (alpha < 1)
        {
            alpha += fadeInSpeed * Time.deltaTime;
            currentColor.a = alpha;
            canvasImage.color = currentColor;
            yield return null;
        }

        fadeInCompleted = true;
        HandleActivation();
    }

    private void HandleActivation()
    {
        foreach (GameObject objectToEnable in objectsToEnable)
        {
            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }
        }

        foreach (GameObject objectToDisable in objectsToDisable)
        {
            if (objectToDisable != null)
            {
                objectToDisable.SetActive(false);
            }
        }
    }
}
