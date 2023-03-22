using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TimedAudioController : MonoBehaviour
{
    [Header("Timed Delay Settings")]
    public float startDelay = 0f;

    [Header("Fade In Settings")]
    public bool fadeIn = false;
    public float fadeInTime = 1f;

    private AudioSource audioSource;
    private float originalVolume;
    private float fadeInTimer = 0f;
    private bool audioStarted = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        originalVolume = audioSource.volume;
        audioSource.volume = fadeIn ? 0f : originalVolume;
        Invoke("StartAudio", startDelay);
    }

    void Update()
    {
        if (audioStarted && fadeIn)
        {
            fadeInTimer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, originalVolume, fadeInTimer / fadeInTime);
            if (audioSource.volume >= originalVolume)
            {
                audioSource.volume = originalVolume;
                fadeIn = false;
            }
        }
    }

    void StartAudio()
    {
        audioSource.Play();
        audioStarted = true;
    }
}