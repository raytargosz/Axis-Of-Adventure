using UnityEngine;
using System.Collections;

public class PressurePadController : MonoBehaviour
{
    [Header("Pressure Pad Settings")]
    [Tooltip("The amount the pressure pad should drop in the Y-axis")]
    public float yOffset = 1f;
    [Tooltip("Duration of the pressure pad animation")]
    public float animationDuration = 0.5f;

    [Header("Sound Effects")]
    [Tooltip("Sound effect to play when stepping on the pressure pad")]
    public AudioClip stepOnSFX;
    [Tooltip("Sound effect to play when stepping off the pressure pad")]
    public AudioClip stepOffSFX;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private AudioSource audioSource;
    private bool isPlayerOnPad = false;
    private bool isAnimating = false;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + new Vector3(0, yOffset, 0);
        audioSource = GetComponent<AudioSource>();
    }

    private IEnumerator AnimatePressurePad(Vector3 target, AudioClip sfx)
    {
        isAnimating = true;
        float elapsedTime = 0f;

        Vector3 startPosition = transform.position;
        audioSource.PlayOneShot(sfx);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            transform.position = Vector3.Lerp(startPosition, target, t);
            yield return null;
        }

        transform.position = target;
        isAnimating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerOnPad)
        {
            isPlayerOnPad = true;
            if (!isAnimating)
            {
                StartCoroutine(AnimatePressurePad(targetPosition, stepOnSFX));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerOnPad)
        {
            isPlayerOnPad = false;
            if (!isAnimating)
            {
                StartCoroutine(AnimatePressurePad(initialPosition, stepOffSFX));
            }
        }
    }
}