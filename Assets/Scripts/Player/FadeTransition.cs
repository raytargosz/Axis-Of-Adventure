using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    [Tooltip("Panel used for the fade transition.")]
    public Image fadePanel;

    [Tooltip("Duration of the fade animation.")]
    public float fadeDuration = 1f;

    [Tooltip("Scene name to transition to.")]
    public string targetSceneName;

    [Tooltip("Objects to enable when player enters the trigger.")]
    public GameObject[] objectsToEnable;

    private bool isInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isInTrigger)
        {
            isInTrigger = true;

            // Enable specified game objects.
            foreach (GameObject obj in objectsToEnable)
            {
                obj.SetActive(true);
            }

            StartCoroutine(FadeAndTransition());
        }
    }

    private IEnumerator FadeAndTransition()
    {
        Color fadeColor = fadePanel.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadePanel.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        // Load the target scene when the fade animation is complete.
        SceneManager.LoadScene(targetSceneName);
    }
}
