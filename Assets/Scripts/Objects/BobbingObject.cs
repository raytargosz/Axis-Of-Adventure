using UnityEngine;

public class BobbingObject : MonoBehaviour
{
    [Tooltip("Amplitude of the bobbing motion.")]
    [SerializeField] private float bobbingAmplitude = 1f;

    [Tooltip("Frequency of the bobbing motion.")]
    [SerializeField] private float bobbingFrequency = 1f;

    [Tooltip("Frequency of the bobbing motion when sprinting.")]
    [SerializeField] private float sprintingBobbingFrequency = 1.5f;

    [Tooltip("Toggle between local and world space for the bobbing motion.")]
    [SerializeField] private bool useLocalSpace = true;

    [Header("Rotation Settings")]
    [Tooltip("Enable or disable X rotation.")]
    [SerializeField] private bool enableXRotation = false;

    [Tooltip("Speed of the X rotation.")]
    [SerializeField] private float xRotationSpeed = 1f;

    [Tooltip("Enable or disable Y rotation.")]
    [SerializeField] private bool enableYRotation = false;

    [Tooltip("Speed of the Y rotation.")]
    [SerializeField] private float yRotationSpeed = 1f;

    [Tooltip("Enable or disable Z rotation.")]
    [SerializeField] private bool enableZRotation = false;

    [Tooltip("Speed of the Z rotation.")]
    [SerializeField] private float zRotationSpeed = 1f;

    [Header("Player Movement")]
    [Tooltip("Reference to the PlayerMovement script.")]
    [SerializeField] private CombinedPlayerController playerController;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        if (useLocalSpace)
        {
            startPosition = transform.localPosition;
        }
        else
        {
            startPosition = transform.position;
        }

        timeOffset = Random.Range(0f, 2 * Mathf.PI);
    }

    private void FixedUpdate()
    {
        if (playerController != null)
        {
            bool isMoving = playerController.IsMoving();
            UpdateBobbing(isMoving);
        }

        RotateObject();
    }

    private void UpdateBobbing(bool shouldBob)
    {
        if (shouldBob)
        {
            float currentBobbingFrequency = playerController.IsSprinting() ? sprintingBobbingFrequency : bobbingFrequency;
            float speedRatio = playerController.IsSprinting() ? sprintingBobbingFrequency / bobbingFrequency : 1f;

            Vector3 newPosition = startPosition + Vector3.up * bobbingAmplitude * Mathf.Sin((Time.time + timeOffset) * currentBobbingFrequency * speedRatio);

            if (useLocalSpace)
            {
                transform.localPosition = newPosition;
            }
            else
            {
                transform.position = newPosition;
            }
        }
        else
        {
            if (useLocalSpace)
            {
                transform.localPosition = startPosition;
            }
            else
            {
                transform.position = startPosition;
            }
        }
    }

    private void RotateObject()
    {
        float xRotation = enableXRotation ? xRotationSpeed * Time.deltaTime : 0;
        float yRotation = enableYRotation ? yRotationSpeed * Time.deltaTime : 0;
        float zRotation = enableZRotation ? zRotationSpeed * Time.deltaTime : 0;

        transform.Rotate(xRotation, yRotation, zRotation, useLocalSpace ? Space.Self : Space.World);
    }

    public float GetVerticalMovement()
    {
        return transform.position.y - startPosition.y;
    }
}
