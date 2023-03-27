using UnityEngine;

public class BobbingObject : MonoBehaviour
{
    [Tooltip("Amplitude of the bobbing motion.")]
    [SerializeField] private float bobbingAmplitude = 1f;

    [Tooltip("Frequency of the bobbing motion.")]
    [SerializeField] private float bobbingFrequency = 1f;

    [Tooltip("Toggle between local and world space for the bobbing motion.")]
    [SerializeField] private bool useLocalSpace = true;

    [Header("Rotation Settings")]
    [Tooltip("Enable or disable Y rotation.")]
    [SerializeField] private bool enableYRotation = false;

    [Tooltip("Speed of the Y rotation.")]
    [SerializeField] private float yRotationSpeed = 1f;

    private Vector3 startPosition;
    private float timeOffset;

    public float GetVerticalMovement()
    {
        return bobbingAmplitude * bobbingFrequency * Mathf.Cos((Time.time + timeOffset) * bobbingFrequency);
    }

    public float GetVerticalVelocity()
    {
        return bobbingAmplitude * bobbingFrequency * Mathf.Cos((Time.time + timeOffset) * bobbingFrequency);
    }

    private void Start()
    {
        // Store the initial position of the object depending on the useLocalSpace setting
        if (useLocalSpace)
        {
            startPosition = transform.localPosition;
        }
        else
        {
            startPosition = transform.position;
        }

        // Randomize the time offset to create variation in the bobbing motion
        timeOffset = Random.Range(0f, 2 * Mathf.PI);
    }

    private void FixedUpdate()
    {
        // Calculate the new position of the object based on the bobbing parameters
        Vector3 newPosition = startPosition + Vector3.up * bobbingAmplitude * Mathf.Sin((Time.time + timeOffset) * bobbingFrequency);

        // Update the position of the object depending on the useLocalSpace setting
        if (useLocalSpace)
        {
            transform.localPosition = newPosition;
        }
        else
        {
            transform.position = newPosition;
        }

        // Rotate the object around the Y axis if enabled
        if (enableYRotation)
        {
            transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0, useLocalSpace ? Space.Self : Space.World);
        }
    }
}