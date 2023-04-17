using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using TMPro;

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

    [Header("UI Settings")]
    [Tooltip("UI TMP element to display when the gem is picked up.")]
    public TextMeshProUGUI pickupUIText;

    [Tooltip("Duration for the UI text to fade in.")]
    public float uiFadeInDuration = 1f;

    [Tooltip("Duration for the UI text to fade out.")]
    public float uiFadeOutDuration = 2f;

    [Header("Toggle Assets")]
    [Tooltip("List of GameObjects to enable/disable upon pickup.")]
    public List<GameObject> toggleObjects;

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

        // Hide the UI text initially
        if (pickupUIText != null)
        {
            Color originalColor = pickupUIText.color;
            pickupUIText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickedUp)
        {
            isPickedUp = true;

            gemCounter.UpdateGemCounter(pickupValue);

            StartCoroutine(PickupAnimation());

            GameObject vfxInstance = Instantiate(this.pickupVFX, transform.position, Quaternion.identity);
            Destroy(vfxInstance, animationDuration);

            AudioSource.PlayClipAtPoint(pickupSFX, transform.position);

            sphereCollider.enabled = false;

            ShowPickupUI();

            ToggleAssets();
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
    private void ShowPickupUI()
    {
        if (pickupUIText != null)
        {
            pickupUIText.gameObject.SetActive(true);
            // Replace the placeholder with the pickup value
            pickupUIText.text = string.Format(pickupUIText.text, "Somewhere A Door Has Been Unlocked");
            StartCoroutine(FadeInAndOutUIText());
        }
    }

    private IEnumerator FadeInAndOutUIText()
    {
        // Fade in
        pickupUIText.CrossFadeAlpha(1, uiFadeInDuration, false);
        yield return new WaitForSeconds(uiFadeInDuration);

        // Wait before fading out
        yield return new WaitForSeconds(uiFadeOutDuration);

        // Fade out
        pickupUIText.CrossFadeAlpha(0, uiFadeOutDuration, false);
        yield return new WaitForSeconds(uiFadeOutDuration);

        // Disable the UI text object
        pickupUIText.gameObject.SetActive(false);
    }

    private void ToggleAssets()
    {
        foreach (GameObject obj in toggleObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }
}
