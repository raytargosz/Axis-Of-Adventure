/*
 * IsometricCameraController controls the camera's position, rotation, and zoom in an isometric view.
 * This script also handles switching between the isometric camera and the first-person camera when the player enters or exits specific zones.
 * To use this script, attach it to the main camera in your scene and configure the public variables as needed.
 */


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
    public Camera firstPersonCamera;

    private Vector3 currentVelocity;
    private Camera cam;
    private float targetRotationAngle;
    private float rotationStartTime;
    private AudioSource audioSource;
    private float lastSFXTime;
    private bool inFirstPersonZone = false;


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

    public void ToggleIsometricCameraMode()
    {
        // Enable or disable the Isometric Camera Controller script
        this.enabled = !this.enabled;
    }


    public void ToggleFirstPersonZone(bool enableFirstPerson)
    {
        inFirstPersonZone = enableFirstPerson;
        firstPersonCamera.gameObject.SetActive(inFirstPersonZone);
        cam.gameObject.SetActive(!inFirstPersonZone);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FirstPersonZone"))
        {
            ToggleFirstPersonZone(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FirstPersonZone"))
        {
            ToggleFirstPersonZone(false);
        }
    }
}