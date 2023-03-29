using System.Collections;
using UnityEngine;

public class CreaturesPressurePad : MonoBehaviour
{
    public GameObject[] creaturePrefabs;
    public AudioClip[] sfxArray;
    public AudioSource audioSource;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    private bool playerSteppedOn = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerSteppedOn)
        {
            playerSteppedOn = true;
            EnableCreaturesAndPlaySFX();
        }
    }

    private void EnableCreaturesAndPlaySFX()
    {
        // Enable all creature prefabs
        foreach (GameObject creature in creaturePrefabs)
        {
            creature.SetActive(true);
        }

        // Play all sound effects
        StartCoroutine(PlaySFXArray());
    }

    private IEnumerator PlaySFXArray()
    {
        foreach (AudioClip sfx in sfxArray)
        {
            if (audioSource != null && sfx != null)
            {
                audioSource.PlayOneShot(sfx, sfxVolume);
                yield return new WaitForSeconds(sfx.length);
            }
        }
    }
}