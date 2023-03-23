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
    public float rotationSFXVolume = 1f;
    public float scrollSFXVolume = 1f;
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

        UpdateRotation();
        UpdateZoom();
    }

    void LateUpdate()
    {
        UpdateCameraPositionAndRotation();
    }

    private void UpdateRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetMouseButtonDown(0))
        {
            RotateLeft();
        }
        else if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetMouseButtonDown(1))
        {
            RotateRight();
        }
    }

    private void RotateLeft()
    {
        targetRotationAngle += 90f;
        rotationStartTime = Time.time;
        PlaySFX(rotateLeftSFX, rotationSFXVolume);
    }

    private void RotateRight()
    {
        targetRotationAngle -= 90f;
        rotationStartTime = Time.time;
        PlaySFX(rotateRightSFX, rotationSFXVolume);
    }

    private void UpdateZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float oldFov = cam.fieldOfView;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * fovSpeed, minFov, maxFov);
            if (cam.fieldOfView != oldFov)
            {
                PlaySFX(scroll > 0f ? zoomInSFX : zoomOutSFX, scrollSFXVolume);
            }
        }
    }

    private void PlaySFX(AudioClip clip, float volume)
    {
        if (Time.time > lastSFXTime + sfxCooldown)
        {
            audioSource.PlayOneShot(clip, volume);
            lastSFXTime = Time.time;
        }
    }

    private void UpdateCameraPositionAndRotation()
    {
        if (Time.time < rotationStartTime + rotationDuration)
        {
            rotationAngle = Mathf.LerpAngle(rotationAngle, targetRotationAngle, (Time.time - rotationStartTime) / rotationDuration);
        }

        Quaternion updatedTargetRotation = Quaternion.Euler(verticalAngle, rotationAngle, 0);
        Vector3 updatedTargetPosition = target.position - updatedTargetRotation * Vector3.forward * distance + offset;
        transform.position = Vector3.SmoothDamp(transform.position, updatedTargetPosition, ref currentVelocity, smoothingSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, updatedTargetRotation, Time.deltaTime * rotationSpeed);
    }
}