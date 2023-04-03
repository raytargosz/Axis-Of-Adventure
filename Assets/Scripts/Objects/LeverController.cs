using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeverController : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("UI text for interaction prompt")]
    public GameObject interactUI;
    [Tooltip("UI text for activated lever")]
    public GameObject activatedUIText;
    [Tooltip("Fade in duration")]
    public float fadeInDuration = 1f;
    [Tooltip("Fade out duration")]
    public float fadeOutDuration = 1f;
    [Tooltip("Hold duration")]
    public float holdDuration = 4f;
    [Tooltip("Asset fade out duration")]
    public float assetFadeOutDuration = 1f;

    [Header("Sound Effects")]
    [Tooltip("Array of sound effects to play")]
    public AudioClip[] sfx;
    [Tooltip("Array of delays between sound effects")]
    public float[] sfxDelays;

    [Header("Assets")]
    [Tooltip("Array of assets to disable")]
    public GameObject[] assetsToDisable;
    [Tooltip("Array of assets to enable")]
    public GameObject[] assetsToEnable;
    [Tooltip("Array of assets to fade out and destroy")]
    public GameObject[] assetsToFadeOutAndDestroy;
    [Tooltip("Particle system for the FX")]
    public ParticleSystem fx;

    private bool playerInRange = false;
    private AudioSource audioSource;
    private bool leverActivated = false;

    public bool IsLeverActivated()
    {
        return leverActivated;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        interactUI.SetActive(false);
        activatedUIText.SetActive(false);
        fx.Stop();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F) && !leverActivated)
        {
            ActivateLever();
        }
    }

    public void ActivatePower()
    {
        powerActivated = true;
        powerActivationUI.SetActive(false);
    }

    private void ActivateLever()
    {
        leverActivated = true;
        interactUI.SetActive(false);

        // Play sound effects with delays
        StartCoroutine(PlaySFXWithDelays());

        // Disable and enable assets
        DisableAssets();
        EnableAssets();

        // Fade in, hold, and fade out activated UI text
        StartCoroutine(FadeAndHoldActivatedUIText());

        // Fade out and destroy assets
        foreach (GameObject asset in assetsToFadeOutAndDestroy)
        {
            StartCoroutine(FadeOutAndDestroy(asset, assetFadeOutDuration));
        }
        // Turn on FX
        fx.Play();
    }

    private IEnumerator PlaySFXWithDelays()
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            audioSource.PlayOneShot(sfx[i]);
            yield return new WaitForSeconds(sfxDelays[i]);
        }
    }

    private void DisableAssets()
    {
        foreach (GameObject asset in assetsToDisable)
        {
            asset.SetActive(false);
        }
    }

    private void EnableAssets()
    {
        foreach (GameObject asset in assetsToEnable)
        {
            asset.SetActive(true);
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

    private IEnumerator FadeAndHoldActivatedUIText()
    {
        // Fade in
        float fadeInElapsed = 0f;
        Color textColor = activatedUIText.GetComponent<Text>().color;
        textColor.a = 0f;
        activatedUIText.SetActive(true);

        while (fadeInElapsed < fadeInDuration)
        {
            fadeInElapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(0f, 1f, fadeInElapsed / fadeInDuration);
            activatedUIText.GetComponent<Text>().color = textColor;
            yield return null;
        }

        textColor.a = 1f;
        activatedUIText.GetComponent<Text>().color = textColor;

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        float fadeOutElapsed = 0f;
        while (fadeOutElapsed < fadeOutDuration)
        {
            fadeOutElapsed += Time.deltaTime;
            textColor.a = Mathf.Lerp(1f, 0f, fadeOutElapsed / fadeOutDuration);
            activatedUIText.GetComponent<Text>().color = textColor;
            yield return null;
        }

        textColor.a = 0f;
        activatedUIText.GetComponent<Text>().color = textColor;
        activatedUIText.SetActive(false);
    }

    private IEnumerator FadeOutAndDestroy(GameObject asset, float duration)
    {
        float elapsedTime = 0f;
        Material material = asset.GetComponent<Renderer>().material;
        Color startColor = material.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / duration);
            material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(asset);
    }
}