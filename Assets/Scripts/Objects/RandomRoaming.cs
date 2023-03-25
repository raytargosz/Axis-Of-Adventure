using UnityEngine;

public class RandomRoaming : MonoBehaviour
{
    [Header("Roaming Settings")]
    [Tooltip("Roaming speed")]
    public float speed = 1f;
    [Tooltip("Roaming radius")]
    public float radius = 5f;
    [Tooltip("Rotation speed")]
    public float rotationSpeed = 30f;
    [Tooltip("Allow roaming on the X-axis")]
    public bool moveOnX = true;
    [Tooltip("Allow roaming on the Y-axis")]
    public bool moveOnY = false;
    [Tooltip("Allow roaming on the Z-axis")]
    public bool moveOnZ = true;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float movementProgress;
    private float rotationProgress;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        SetNewTargetPosition();
        SetNewTargetRotation();
    }

    private void Update()
    {
        movementProgress += Time.deltaTime * speed / Vector3.Distance(startPosition, targetPosition);
        rotationProgress += Time.deltaTime * rotationSpeed;

        if (movementProgress >= 1f)
        {
            SetNewTargetPosition();
        }

        if (rotationProgress >= 1f)
        {
            SetNewTargetRotation();
        }

        transform.position = Vector3.Lerp(startPosition, targetPosition, movementProgress);
        transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationProgress);
    }

    private void SetNewTargetPosition()
    {
        float x = moveOnX ? startPosition.x + Random.Range(-radius, radius) : startPosition.x;
        float y = moveOnY ? startPosition.y + Random.Range(-radius, radius) : startPosition.y;
        float z = moveOnZ ? startPosition.z + Random.Range(-radius, radius) : startPosition.z;
        targetPosition = new Vector3(x, y, z);
        startPosition = transform.position;
        movementProgress = 0f;
    }

    private void SetNewTargetRotation()
    {
        startRotation = transform.rotation;
        targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, Random.Range(-180f, 180f), 0));
        rotationProgress = 0f;
    }
}