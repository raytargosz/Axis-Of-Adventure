using UnityEngine;

public class BroomSweeping : MonoBehaviour
{
    public float swingSpeed = 1f;
    public float swingAmplitude = 1f;
    public float randomMoveSpeed = 1f;
    public float changeTargetDelay = 1f;
    public Vector3 moveRange = new Vector3(1f, 0f, 1f);
    public Transform swingPivot;
    public float minRotationSpeed = 30f;
    public float maxRotationSpeed = 60f;
    public float changeRotationSpeedDelay = 2f;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float timeSinceLastTargetChange;
    private float elapsedTime;
    private float currentRotationSpeed;
    private float timeSinceLastRotationSpeedChange;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = GetRandomTargetPosition();
        timeSinceLastTargetChange = 0f;
        elapsedTime = 0f;
        currentRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
        timeSinceLastRotationSpeedChange = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        timeSinceLastTargetChange += Time.deltaTime;
        timeSinceLastRotationSpeedChange += Time.deltaTime;

        if (timeSinceLastTargetChange >= changeTargetDelay)
        {
            targetPosition = GetRandomTargetPosition();
            timeSinceLastTargetChange = 0f;
        }

        if (timeSinceLastRotationSpeedChange >= changeRotationSpeedDelay)
        {
            currentRotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
            timeSinceLastRotationSpeedChange = 0f;
        }

        float swingAngle = swingAmplitude * Mathf.Sin(elapsedTime * swingSpeed);
        swingPivot.localRotation = Quaternion.Euler(swingAngle, 0f, swingAngle);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * randomMoveSpeed);

        // Update Y rotation
        transform.Rotate(0, currentRotationSpeed * Time.deltaTime, 0);
    }

    private Vector3 GetRandomTargetPosition()
    {
        return initialPosition + new Vector3(
            Random.Range(-moveRange.x, moveRange.x),
            0f,
            Random.Range(-moveRange.z, moveRange.z)
        );
    }
}