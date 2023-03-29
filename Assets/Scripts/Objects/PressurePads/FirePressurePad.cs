using System.Collections;
using UnityEngine;

public class FirePressurePad : MonoBehaviour
{
    public GameObject[] fxArray;
    public AudioClip stepOnSFX;
    public AudioClip fxSFX;
    public AudioSource audioSource;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    public float activationInterval = 5f;

    private bool playerSteppedOn = false;
    private bool fxActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerSteppedOn)
        {
            playerSteppedOn = true;
            StartCoroutine(ActivateFXAtRandomIntervals());
            PlaySFX(stepOnSFX);
        }
    }

    private IEnumerator ActivateFXAtRandomIntervals()
    {
        fxActivated = true;
        float startTime = Time.time;
        while (Time.time - startTime < activationInterval)
        {
            int randomIndex = Random.Range(0, fxArray.Length);
            fxArray[randomIndex].SetActive(true);
            PlaySFX(fxSFX);
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        }

        // Ensure all FX are enabled at the end of the activation interval
        foreach (GameObject fx in fxArray)
        {
            fx.SetActive(true);
        }
    }

    private void PlaySFX(AudioClip sfx)
    {
        if (audioSource != null && sfx != null)
        {
            audioSource.PlayOneShot(sfx, sfxVolume);
        }
    }
}