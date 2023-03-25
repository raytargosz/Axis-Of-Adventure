using UnityEngine;

public class Weathervane : MonoBehaviour
{
    public float minRotationSpeed = 10f;
    public float maxRotationSpeed = 30f;
    public float changeDirectionInterval = 1f;

    private float rotationSpeed;
    private float timeSinceLastDirectionChange;
    private Transform weathervaneTransform;

    void Start()
    {
        weathervaneTransform = transform;
        timeSinceLastDirectionChange = 0f;
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.value < 0.5f ? -1f : 1f);
    }

    void Update()
    {
        timeSinceLastDirectionChange += Time.deltaTime;

        if (timeSinceLastDirectionChange >= changeDirectionInterval)
        {
            rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed) * (Random.value < 0.5f ? -1f : 1f);
            timeSinceLastDirectionChange = 0f;
        }

        float rotation = rotationSpeed * Time.deltaTime;
        weathervaneTransform.Rotate(0f, rotation, 0f, Space.World);
    }
}