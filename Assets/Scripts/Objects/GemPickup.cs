using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;

public class GemPickup : MonoBehaviour
{
    [Tooltip("Pickup value of the gem.")]
    public int pickupValue = 1;

    [Tooltip("Speed at which the gem moves upwards.")]
    public float moveUpSpeed = 1f;

    [Tooltip("Speed at which the gem spins.")]
    public float spinSpeed = 180f;

    [Tooltip("Duration of the pickup animation.")]
    public float animationDuration = 1f;

    [Tooltip("Audio clip to play when gem is picked up.")]
    public AudioClip pickupSFX;

    [Tooltip("Particle effect to play when the gem is picked up.")]
    public GameObject pickupVFX;

    [Tooltip("Reference to the sphere collider that should be disabled on pickup.")]
    public Collider sphereCollider;

    private bool isPickedUp = false;

    private GemCounter gemCounter;

    private void Start()
    {
        GameObject gemCounterObject = GameObject.Find("GemCounter");
        if (gemCounterObject != null)
        {
            gemCounter = gemCounterObject.GetComponent<GemCounter>();
        }
        else
        {
            Debug.LogError("GemCounter object not found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Replace "Player" with the tag of your player object.
        if (other.CompareTag("Player") && !isPickedUp)
        {
            // Set isPickedUp to true to prevent repeated OnTriggerEnter calls
            isPickedUp = true;

            // Call UpdateGemCounter method when the player picks up a gem.
            gemCounter.UpdateGemCounter(pickupValue);

            StartCoroutine(PickupAnimation());

            // Instantiate VFX and destroy it after animationDuration
            GameObject vfxInstance = Instantiate(this.pickupVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, animationDuration);

            AudioSource.PlayClipAtPoint(pickupSFX, transform.position);

            // Disable the sphere collider.
            sphereCollider.enabled = false;
        }
    }

    private IEnumerator PickupAnimation()
    {
        float timer = 0f;
        Vector3 startPosition = transform.position;

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float moveUp = Mathf.Lerp(0, moveUpSpeed, timer / animationDuration);
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
            transform.position = new Vector3(startPosition.x, startPosition.y + moveUp, startPosition.z);
            yield return null;
        }

        Destroy(gameObject);
    }
}
