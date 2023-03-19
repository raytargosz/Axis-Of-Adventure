using System.Collections;
using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float rotationSpeed = 100f;
    public float smoothingSpeed = 0.125f;
    public float rotationAngle = 45f;
    public float verticalAngle = 30f;
    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float distance = 10f;
    public float minFov = 30f;
    public float maxFov = 60f;
    public float fovSpeed = 10f;
    public float rotationDuration = 1f;
    public AudioClip rotateLeftSFX;
    public AudioClip rotateRightSFX;
    public AudioClip zoomInSFX;
    public AudioClip zoomOutSFX;
    public float sfxCooldown = 0.5f;

    private Vector3 currentVelocity;
    private Camera cam;
    private float targetRotationAngle;
    private float rotationStartTime;
    private AudioSource audioSource;
    private float lastSFXTime;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetRotationAngle = rotationAngle;
        rotationStartTime = -rotationDuration;
        audioSource = GetComponent<AudioSource>();
        lastSFXTime = Time.time;
    }

    void Update()
    {
        if (target == null) return;

        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetMouseButtonDown(0))
        {
            targetRotationAngle += 90f;
            rotationStartTime = Time.time;
            if (Time.time > lastSFXTime + sfxCooldown)
            {
                audioSource.PlayOneShot(rotateLeftSFX);
                lastSFXTime = Time.time;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetMouseButtonDown(1))
        {
            targetRotationAngle -= 90f;
            rotationStartTime = Time.time;
            if (Time.time > lastSFXTime + sfxCooldown)
            {
                audioSource.PlayOneShot(rotateRightSFX);
                lastSFXTime = Time.time;
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float oldFov = cam.fieldOfView;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * fovSpeed, minFov, maxFov);
            if (Time.time > lastSFXTime + sfxCooldown)
            {
                if (scroll > 0f)
                {
                    audioSource.PlayOneShot(zoomInSFX);
                }
                else
                {
                    audioSource.PlayOneShot(zoomOutSFX);
                }
                lastSFXTime = Time.time;
            }
        }
    }

    void LateUpdate()
    {
        if (Time.time < rotationStartTime + rotationDuration)
        {
            rotationAngle = Mathf.LerpAngle(rotationAngle, targetRotationAngle, (Time.time - rotationStartTime) / rotationDuration);
        }

        Quaternion targetRotation = Quaternion.Euler(verticalAngle, rotationAngle, 0);
        Vector3 targetPosition = target.position - targetRotation * Vector3.forward * distance + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothingSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}