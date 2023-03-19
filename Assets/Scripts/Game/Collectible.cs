using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip specialCollectibleSound;
    [SerializeField] private AudioClip disappearSound;
    [SerializeField] private float spinTime = 4f;
    [SerializeField] private GameObject particleEffectPrefab;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float fadeOutDuration = 1f;

    private AudioSource audioSource;
    private bool isCollected;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered: " + other.gameObject.name);

        if (other.CompareTag("Player") && !isCollected)
        {
            Debug.Log("Collectible triggered by player");

            isCollected = true;

            CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
            collectibleManager.IncrementCollectibleCount();
            int collectibleCounter = CollectibleManager.CollectibleCount;

            if (collectibleCounter % 5 == 0 && specialCollectibleSound != null)
            {
                audioSource.PlayOneShot(specialCollectibleSound);
            }
            else if (collectibleSound != null)
            {
                audioSource.PlayOneShot(collectibleSound);
            }

            StartCoroutine(SpinAndDisappear(spinTime));

            if (collectibleCounter >= collectibleManager.RequiredPickupAmount)
            {
                StartCoroutine(FadeOutAndLoadNextScene());
            }
        }
    }

    private IEnumerator SpinAndDisappear(float duration)
    {
        GameObject particleEffectInstance = null;

        if (particleEffectPrefab != null)
        {
            particleEffectInstance = Instantiate(particleEffectPrefab, transform.position, Quaternion.identity);
        }

        float elapsedTime = 0f;
        float initialYPosition = transform.position.y;
        float spinSpeedMultiplier = 10f; // Adjust this value to control the spin speed
        float upwardSpeed = 1f; // Adjust this value to control the upward movement speed

        while (elapsedTime < duration)
        {
            transform.Rotate(0, 360 * Time.deltaTime * (elapsedTime / duration) * spinSpeedMultiplier, 0);
            float newYPosition = initialYPosition + (elapsedTime / duration) * upwardSpeed;
            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);

            if (particleEffectInstance != null)
            {
                particleEffectInstance.transform.position = transform.position;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (disappearSound != null)
        {
            audioSource.PlayOneShot(disappearSound);
        }

        if (particleEffectInstance != null)
        {
            Destroy(particleEffectInstance);
        }
        Destroy(gameObject, disappearSound != null ? disappearSound.length : 0f);
    }

    private IEnumerator FadeOutAndLoadNextScene()
    {
        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        CanvasGroup canvasGroup = collectibleManager.CollectibleCounterText.transform.parent.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        SceneManager.LoadScene(nextSceneName);
    }

}
