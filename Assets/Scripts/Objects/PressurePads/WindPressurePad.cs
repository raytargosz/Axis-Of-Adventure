using UnityEngine;

public class WindPressurePad : MonoBehaviour
{
    public AudioClip[] stepOnSFX;
    public AudioSource audioSource;
    [Range(0f, 1f)]
    public float sfxVolume = 1f;
    public GameObject windFX;

    private bool playerSteppedOn = false;

    private void Start()
    {
        if (windFX != null)
        {
            windFX.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerSteppedOn)
        {
            playerSteppedOn = true;
            PlayRandomSFX();
            EnableWindFX();
        }
    }

    private void PlayRandomSFX()
    {
        if (audioSource != null && stepOnSFX.Length > 0)
        {
            int randomIndex = Random.Range(0, stepOnSFX.Length);
            AudioClip selectedSFX = stepOnSFX[randomIndex];

            if (selectedSFX != null)
            {
                audioSource.PlayOneShot(selectedSFX, sfxVolume);
            }
        }
    }

    private void EnableWindFX()
    {
        if (windFX != null)
        {
            windFX.SetActive(true);
        }
    }
}