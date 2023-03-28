using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractiveTrigger : MonoBehaviour
{
    public string targetSceneName;
    public GameObject promptUI;
    public AudioClip sfx;
    public AudioClip lockedSfx;
    public int requiredCollectibles;

    private CombinedPlayerController playerMovement;
    private FadeController fadeController;
    private AudioSource audioSource;
    private bool playerInRange = false;
    private GameObject player;
    private CollectibleManager collectibleManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<CombinedPlayerController>();
        fadeController = FindObjectOfType<FadeController>();
        audioSource = GetComponent<AudioSource>();
        collectibleManager = FindObjectOfType<CollectibleManager>();
        promptUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (collectibleManager.CollectibleCount >= requiredCollectibles)
            {
                if (sfx != null)
                {
                    audioSource.PlayOneShot(sfx);
                }

                StartCoroutine(LoadTargetScene());
            }
            else
            {
                if (lockedSfx != null)
                {
                    audioSource.PlayOneShot(lockedSfx);
                }
            }
        }
    }

    IEnumerator LoadTargetScene()
    {
        playerMovement.enabled = false;
        SetPlayerMeshesActive(false);

        fadeController.StartFadeOut(targetSceneName);
        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(targetSceneName);
    }

    private void SetPlayerMeshesActive(bool isActive)
    {
        MeshRenderer[] meshRenderers = player.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.enabled = isActive;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptUI.SetActive(false);
        }
    }
}