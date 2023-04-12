using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class InteractiveTrigger : MonoBehaviour
{
    [Header("Scene Transition")]
    [Tooltip("Target scene to load when the door is opened.")]
    public string targetSceneName;

    [Header("UI")]
    [Tooltip("UI prompt for door interaction.")]
    public GameObject promptUI;
    [Tooltip("UI element to display door lock status.")]
    public GameObject lockStatusUI;

    [Header("Audio")]
    [Tooltip("Sound effect to play when door is unlocked.")]
    public AudioClip unlockSFX;
    [Tooltip("Sound effect to play when door is locked.")]
    public AudioClip lockedSFX;

    [Header("Fade Settings")]
    [Tooltip("Image to fade in and out.")]
    public Image fadeImage;
    [Tooltip("Fade duration in seconds.")]
    public float fadeDuration = 2.0f;

    [Header("Door Lock Settings")]
    [Tooltip("Is the door locked by default?")]
    public bool lockedByDefault = true;

    private AudioSource audioSource;
    private bool playerInRange = false;
    private GameObject player;
    private KeyPickup keyPickup;
    private bool doorUnlocked = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = GetComponent<AudioSource>();
        promptUI.SetActive(false);
        lockStatusUI.SetActive(false);
        fadeImage.canvasRenderer.SetAlpha(0.0f);

        if (!lockedByDefault)
        {
            doorUnlocked = true;
        }
    }

    private void UpdateLockStatusUI()
    {
        lockStatusUI.GetComponent<TextMeshProUGUI>().text = doorUnlocked ? "Door Unlocked" : "Door Locked";
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (doorUnlocked)
            {
                if (unlockSFX != null)
                {
                    audioSource.PlayOneShot(unlockSFX);
                }

                StartCoroutine(LoadTargetScene());
            }
            else
            {
                if (lockedSFX != null)
                {
                    audioSource.PlayOneShot(lockedSFX);
                }
            }
        }
    }

    IEnumerator LoadTargetScene()
    {
        player.SetActive(false);
        FadeIn();
        yield return new WaitForSeconds(fadeDuration);
        SceneManager.LoadScene(targetSceneName);
    }

    private void FadeIn()
    {
        fadeImage.CrossFadeAlpha(1, fadeDuration, false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            keyPickup = other.GetComponent<KeyPickup>();

            if (keyPickup != null && keyPickup.HasKey)
            {
                doorUnlocked = true;
            }

            playerInRange = true;
            promptUI.SetActive(true);
            lockStatusUI.SetActive(true);
            UpdateLockStatusUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptUI.SetActive(false);
            lockStatusUI.SetActive(false);
        }
    }
}
