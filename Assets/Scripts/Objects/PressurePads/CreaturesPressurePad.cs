using System.Collections;
using UnityEngine;

public class CreaturesPressurePad : MonoBehaviour
{
    public GameObject[] creaturePrefabs;
    public AudioClip[] sfxArray;
    public AudioSource audioSource;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    public float fadeInDuration = 1f;

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
        // Enable and fade in all creature prefabs
        foreach (GameObject creature in creaturePrefabs)
        {
            creature.SetActive(true);
            StartCoroutine(FadeInCreature(creature));
        }

        // Play all sound effects
        StartCoroutine(PlaySFXArray());
    }

    private IEnumerator FadeInCreature(GameObject creature)
    {
        Renderer renderer = creature.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Renderer component not found on creature.");
            yield break;
        }

        Color initialColor = renderer.material.color;
        initialColor.a = 0f;
        renderer.material.color = initialColor;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            initialColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            renderer.material.color = initialColor;
            yield return null;
        }

        initialColor.a = 1f;
        renderer.material.color = initialColor;
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