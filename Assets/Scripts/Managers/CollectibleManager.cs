using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class CollectibleManager : MonoBehaviour
{
    [Tooltip("Text to display the collectible count")]
    [SerializeField] private TextMeshProUGUI collectibleCounterText;

    [Tooltip("Required number of collectibles to complete the level")]
    [SerializeField] private int requiredPickupAmount = 20;

    [Tooltip("Image used for the fade effect when transitioning between scenes")]
    [SerializeField] private Image fadeImage;

    [Tooltip("Duration of the fade-in effect when transitioning between scenes")]
    [SerializeField] private float fadeInDuration = 1f;

    [Tooltip("Name of the next scene to load after completing the level")]
    [SerializeField] private string nextSceneName;

    [Tooltip("Sound effect to play when the level is completed")]
    [SerializeField] private AudioClip levelCompleteSFX;

    [Tooltip("Audio source to play the level complete sound effect")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Duration of the audio fade-out effect when the level is completed")]
    [SerializeField] private float audioFadeOutDuration = 1f;

    [Tooltip("Font size for the timer text when the level is completed")]
    [SerializeField] private int enlargedFontSize = 100;

    [Tooltip("Duration of the timer text fade-out effect")]
    [SerializeField] private float timerTextFadeDuration = 4f;

    [Tooltip("Time to wait before transitioning to the next scene")]
    [SerializeField] private float waitBeforeTransition = 4f;

    [Tooltip("Interval for determining if a collectible is special")]
    [SerializeField] private int specialCollectibleInterval = 5;

    private bool levelCompleted = false;

    public int CollectibleCount { get; private set; }
    public int RequiredPickupAmount => requiredPickupAmount;
    public event Action OnCollectiblePicked;

    private void Start()
    {
        CollectibleCount = 0;
        UpdateCollectibleCounterText();
    }

    private void Update()
    {
        CheckForDevCheats();

        if (!levelCompleted && HasRequiredPickupAmountReached())
        {
            levelCompleted = true;
            StartCoroutine(FadeInImageAndLoadNextScene(fadeImage, fadeInDuration));
        }
    }

    public void IncrementCollectibleCount()
    {
        CollectibleCount++;
        UpdateCollectibleCounterText();
        OnCollectiblePicked?.Invoke();
    }

    private void UpdateCollectibleCounterText()
    {
        collectibleCounterText.text = $"{CollectibleCount}/{requiredPickupAmount}";
    }

    public bool HasRequiredPickupAmountReached()
    {
        return CollectibleCount >= RequiredPickupAmount;
    }

    private void CheckForDevCheats()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            {
                for (int i = 0; i < 10; i++)
                {
                    IncrementCollectibleCount();
                }
            }
        }
    }

    public bool IsSpecialCollectible(int collectibleCount)
    {
        // Check if the given collectible count is a multiple of the special collectible interval
        return collectibleCount % specialCollectibleInterval == 0;
    }

    private IEnumerator FadeOutAllAudio(float duration)
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foreach (AudioSource source in audioSources)
            {
                if (source != audioSource)
                {
                    source.volume = Mathf.Lerp(1, 0, elapsedTime / duration);
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (AudioSource source in audioSources)
        {
            source.volume = 0;
        }
    }

    private IEnumerator FadeOutTriggerAudioOnce(float duration)
    {
        TriggerAudioOnce[] triggerAudioOnceComponents = FindObjectsOfType<TriggerAudioOnce>();
        List<AudioSource> audioSources = new List<AudioSource>();

        foreach (TriggerAudioOnce trigger in triggerAudioOnceComponents)
        {
            AudioSource triggerAudioSource = trigger.GetComponent<AudioSource>();
            audioSources.Add(triggerAudioSource);
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foreach (AudioSource source in audioSources)
            {
                source.volume = Mathf.Lerp(1, 0, elapsedTime / duration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foreach (AudioSource source in audioSources)
        {
            source.volume = 0;
            source.Stop();
        }
    }

    private IEnumerator FadeInImageAndLoadNextScene(Image fadeImage, float duration)
    {
        LevelTimer levelTimer = FindObjectOfType<LevelTimer>();
        audioSource.PlayOneShot(levelCompleteSFX);
        StartCoroutine(FadeOutTriggerAudioOnce(audioFadeOutDuration));

        CanvasGroup canvasGroup = fadeImage.GetComponent<CanvasGroup>();
        TextMeshProUGUI timerText = levelTimer?.GetComponentInChildren<TextMeshProUGUI>();

        if (levelTimer != null)
        {
            levelTimer.FinishLevel();
            timerText.fontSize = enlargedFontSize;
            timerText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            timerText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            timerText.rectTransform.anchoredPosition = Vector2.zero;
            timerText.transform.parent.SetSiblingIndex(canvasGroup.transform.parent.GetSiblingIndex() + 1);
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(waitBeforeTransition);

        if (timerText != null)
        {
            elapsedTime = 0f;
            while (elapsedTime < timerTextFadeDuration)
            {
                timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, Mathf.Lerp(1, 0, elapsedTime / timerTextFadeDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 0);
        }

        SceneManager.LoadScene(nextSceneName);
    }
} 
