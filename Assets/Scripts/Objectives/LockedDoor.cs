using UnityEngine;
using UnityEngine.UI;

public class LockedDoor : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("UI text for interaction prompt")]
    public GameObject interactUI;
    [Tooltip("UI text for locked door objective")]
    public GameObject lockedObjectiveUI;

    [Header("Sound Effects")]
    [Tooltip("Locked door sound effect")]
    public AudioClip lockedSFX;

    private AudioSource audioSource;
    private bool playerInRange = false;
    private bool doorUnlocked = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactUI.SetActive(false);
        lockedObjectiveUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !doorUnlocked)
        {
            audioSource.PlayOneShot(lockedSFX);
            lockedObjectiveUI.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }

    public void UnlockDoor()
    {
        doorUnlocked = true;
    }
}