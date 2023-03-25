using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RotateObject : MonoBehaviour
{
    [Tooltip("Axis of rotation (e.g. Vector3.up for Y-axis rotation)")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Tooltip("Rotation speed in degrees per second")]
    [SerializeField] private float rotationSpeed = 30f;

    [Tooltip("Audio clip to be played while the object is rotating")]
    [SerializeField] private AudioClip rotationSound;

    [Tooltip("Minimum volume of the rotation sound")]
    [SerializeField, Range(0f, 1f)] private float minVolume = 0.5f;

    [Tooltip("Maximum volume of the rotation sound")]
    [SerializeField, Range(0f, 1f)] private float maxVolume = 1f;

    [Tooltip("Minimum distance from the object for the sound volume calculation")]
    [SerializeField] private float minDistance = 1f;

    [Tooltip("Maximum distance from the object for the sound volume calculation")]
    [SerializeField] private float maxDistance = 5f;

    private AudioSource audioSource;
    private float rolloffFactor;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = rotationSound;
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;
        audioSource.volume = minVolume;

        rolloffFactor = (maxVolume - minVolume) / (maxDistance - minDistance);

        audioSource.Play();
    }

    private void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            audioSource.volume = Mathf.Clamp(minVolume + rolloffFactor * (distance - minDistance), minVolume, maxVolume);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.volume = minVolume;
        }
    }
}