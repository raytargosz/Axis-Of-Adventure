using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [Tooltip("Indicates if the target is currently activated by the light beam")]
    [SerializeField] private bool isActivated = false;

    [Header("Target Settings")]
    [Tooltip("Layer of the light beam")]
    [SerializeField] private LayerMask lightBeamLayer;
    [Tooltip("Duration of the target's activation animation")]
    [SerializeField] private float activationDuration = 1f;

    [Header("Visual & Audio Feedback")]
    [Tooltip("Sound effect to play when the target is activated")]
    [SerializeField] private AudioClip activationSound;
    [Tooltip("Audio source used to play sound effects")]
    [SerializeField] private AudioSource audioSource;

    [Space]
    [Tooltip("Game object or visual effect to enable when the target is activated")]
    [SerializeField] private GameObject activationEffect;

    private bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the light beam
        if (((1 << other.gameObject.layer) & lightBeamLayer) != 0 && !isActive)
        {
            ActivateTarget();
        }
    }

    private void ActivateTarget()
    {
        // Set the target to active
        isActive = true;

        // Play activation sound effect
        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        // Enable visual effect
        if (activationEffect != null)
        {
            activationEffect.SetActive(true);
        }

        // Execute any additional logic when the target is activated
        StartCoroutine(OnTargetActivated());
    }

    private IEnumerator OnTargetActivated()
    {
        // Wait for the activation animation to finish (if any)
        yield return new WaitForSeconds(activationDuration);

        // Trigger any events, animations or actions here (e.g., opening a door, revealing a hidden passage, etc.)
        // For example, you can use UnityEvent to create a custom event that can be assigned in the Inspector
    }

    // You can call this method from the LightBeamController when the light beam hits the target
    public void Activate()
    {
        isActivated = true;
        // You can also include additional logic here, such as playing a sound effect or changing the target's appearance
    }

    // You can call this method from the LightBeamController when the light beam stops hitting the target
    public void Deactivate()
    {
        isActivated = false;
        // You can also include additional logic here, such as reverting the target's appearance
    }

    public bool IsActivated()
    {
        return isActivated;
    }
}
