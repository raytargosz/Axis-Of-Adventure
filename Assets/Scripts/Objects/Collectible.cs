using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip specialCollectibleSound;
    [SerializeField] private AudioClip disappearSound;
    [SerializeField] private float spinTime = 4f;
    [SerializeField] private GameObject particleEffectPrefab;
    [SerializeField] private string nextSceneName;
    [SerializeField] private Vector3 spinDirection = Vector3.up;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private GameObject image;
    [SerializeField] private AudioClip levelCompleteSFX;

    [Header("Transition")]
    public CombinedPlayerController playerController;
    public float fadeInDuration = 1f;

    private CollectibleManager collectibleManager;
    private AudioSource audioSource;
    private bool isCollected;

    private void Start()
    {
        collectibleManager = FindObjectOfType<CollectibleManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger entered: " + other.gameObject.name);

        if (other.CompareTag("Player") && !isCollected)
        {
            Debug.Log("Collectible triggered by player");

            isCollected = true;

            CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
            collectibleManager.IncrementCollectibleCount();

            if (collectibleManager.IsSpecialCollectible(CollectibleManager.CollectibleCount) && specialCollectibleSound != null)
            {
                audioSource.PlayOneShot(specialCollectibleSound, sfxVolume);
            }
            else if (collectibleSound != null)
            {
                audioSource.PlayOneShot(collectibleSound, sfxVolume);
            }

            StartCoroutine(SpinAndDisappear(spinTime));

            if (collectibleManager.HasRequiredPickupAmountReached())
            {
                StartCoroutine(FadeInImageAndLoadNextScene(image, fadeInDuration));
            }
        }
    }

    IEnumerator SpinAndDisappear(float duration)
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
            transform.Rotate(spinDirection * 360 * Time.deltaTime * (elapsedTime / duration) * spinSpeedMultiplier);
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
            audioSource.PlayOneShot(disappearSound, sfxVolume);
        }

        if (particleEffectInstance != null)
        {
            Destroy(particleEffectInstance);
        }
        Destroy(gameObject, disappearSound != null ? disappearSound.length : 0f);
    }

    IEnumerator FadeOutAudioSources(AudioSource[] audioSources, float duration)
    {
        float elapsedTime = 0f;

        Dictionary<AudioSource, float> initialVolumes = audioSources.ToDictionary(x => x, x => x.volume);

        while (elapsedTime < duration)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.volume = Mathf.Lerp(initialVolumes[audioSource], 0, elapsedTime / duration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

        IEnumerator FadeInImageAndLoadNextScene(GameObject image, float duration)
        {
            if (levelCompleteSFX != null && audioSource != null)
            {
                audioSource.PlayOneShot(levelCompleteSFX);
            }

            playerController.enabled = false; // Disable the player controller

            // Get all audio sources in the scene
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();

            // Fade out all audio sources except level complete SFX
            StartCoroutine(FadeOutAudioSources(audioSources.Where(a => a != audioSource).ToArray(), duration));

            // Fade in image
            CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                float elapsedTime = 0f;
                while (elapsedTime < duration)
                {
                    canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            // Load the next scene
            SceneManager.LoadScene(nextSceneName);
        }


        private IEnumerator FadeOutAllAudioExceptLevelCompleteSFX(AudioSource[] audioSources, float duration)
        {
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
                if (audioSources[i] != audioSource || !audioSource.isPlaying || audioSource.clip != levelCompleteSFX)
                {
                    audioSources[i].volume = Mathf.Lerp(initialVolumes[i], 0, elapsedTime / duration);
                }
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Set all audio sources to 0 volume, except for the level complete SFX
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (audioSources[i] != audioSource || !audioSource.isPlaying || audioSource.clip != levelCompleteSFX)
            {
                audioSources[i].volume = 0;
            }
        }
    }
}