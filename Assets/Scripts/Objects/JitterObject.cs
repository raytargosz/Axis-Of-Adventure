using UnityEngine;

public class JitterObject : MonoBehaviour
{
    [Header("Jitter Settings")]
    [Tooltip("Amplitude of the jitter effect")]
    public float amplitude = 0.1f;
    [Tooltip("Speed of the jitter effect")]
    public float speed = 10f;
    [Tooltip("LeverController script reference")]
    public LeverController leverController;

    private Vector3 startPosition;
    private bool jitterActive = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (leverController != null && leverController.IsLeverActivated() && !jitterActive)
        {
            jitterActive = true;
        }

        if (jitterActive)
        {
            Jitter();
        }
    }

    // Jitter the object on the spot
    private void Jitter()
    {
        float offsetX = Mathf.Sin(Time.time * speed) * amplitude;
        float offsetY = Mathf.Cos(Time.time * speed) * amplitude;
        float offsetZ = Mathf.Sin(Time.time * speed) * amplitude;

        transform.position = startPosition + new Vector3(offsetX, offsetY, offsetZ);
    }
}