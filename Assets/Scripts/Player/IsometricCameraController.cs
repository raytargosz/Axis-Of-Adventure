/*
 * IsometricCameraController controls the camera's position, rotation, and zoom in an isometric view.
 * This script also handles switching between the isometric camera and the first-person camera when the player enters or exits specific zones.
 * To use this script, attach it to the main camera in your scene and configure the public variables as needed.
 */


using System.Collections;
using UnityEngine;

public class IsometricCameraController : MonoBehaviour
{
    // Camera target and movement settings
    [Header("Camera Movement Settings")]
    public Transform target;
    public Vector3 offset;
    public float rotationSpeed = 100f;
    public float smoothingSpeed = 0.125f;
    public float rotationAngle = 45f;
    public float verticalAngle = 30f;
    public float minDistance = 5f;
    public float maxDistance = 15f;
    public float distance = 10f;

    // Camera zoom settings
    [Header("Camera Zoom Settings")]
    public float minFov = 30f;
    public float maxFov = 60f;
    public float fovSpeed = 10f;

    // Camera rotation settings
    [Header("Camera Rotation Settings")]
    public float rotationDuration = 1f;
    public AudioClip rotateLeftSFX;
    public AudioClip rotateRightSFX;
    public float rotationSFXVolume = 1f;
    public float sfxCooldown = 0.5f;

    // Camera swooping settings
    [Header("Camera Swooping")]
    [Tooltip("Enable or disable camera swooping effects")]
    public bool enableCameraSwoops = true;

    [Tooltip("Speed of the camera swooping in and out")]
    public float cameraSwoopSpeed = 5f;

    [Tooltip("Distance for the camera to swoop in")]
    public float swoopInDistance = 2f;

    // Camera scroll sound effects settings
    [Header("Camera Scroll Sound Effects")]
    public AudioClip zoomInSFX;
    public AudioClip zoomOutSFX;
    public float scrollSFXVolume = 1f;

    // First-person camera settings
    [Header("First-Person Camera")]
    public Camera firstPersonCamera;

    // Private variables
    private Vector3 initialPosition;
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

        // Store the initial position of the camera
        initialPosition = transform.position;
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
        if (!enableFirstPerson && !this.enabled)
        {
            this.enabled = true;
        }
        StartCoroutine(CameraTransition(enableFirstPerson));
    }

    IEnumerator CameraTransition(bool enableFirstPerson)
    {
        float transitionDuration = 1f;
        float elapsedTime = 0f;

        Vector3 initialCameraPosition = transform.position;
        Quaternion initialCameraRotation = transform.rotation;

        Vector3 targetCameraPosition;
        Quaternion targetCameraRotation;

        if (enableFirstPerson)
        {
            targetCameraPosition = firstPersonCamera.transform.position;
            targetCameraRotation = firstPersonCamera.transform.rotation;
        }
        else
        {
            targetCameraPosition = target.position - initialCameraRotation * Vector3.forward * distance + offset;
            targetCameraRotation = initialCameraRotation;
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition, elapsedTime / transitionDuration);
            transform.rotation = Quaternion.Lerp(initialCameraRotation, targetCameraRotation, elapsedTime / transitionDuration);

            yield return null;
        }

        inFirstPersonZone = enableFirstPerson;

        // Enable/disable the Camera components instead of the game objects
        firstPersonCamera.GetComponent<Camera>().enabled = inFirstPersonZone;
        cam.enabled = !inFirstPersonZone;

        // Disable the IsometricCameraController script when in the first-person zone
        this.enabled = !inFirstPersonZone;

        // If not in the first-person zone, move the IsoCamera back to its original position
        if (!inFirstPersonZone)
        {
            transform.position = initialPosition;
        }

        inFirstPersonZone = enableFirstPerson;

        // Enable/disable the Camera components instead of the game objects
        firstPersonCamera.GetComponent<Camera>().enabled = inFirstPersonZone;
        cam.enabled = !inFirstPersonZone;

        // Disable the IsometricCameraController script when in the first-person zone
        this.enabled = !inFirstPersonZone;

        if (!inFirstPersonZone)
        {
            // Start the swooping effect after the FPS camera is disabled
            StartCoroutine(SwoopToInitialPosition());
        }
        inFirstPersonZone = enableFirstPerson;

        // Enable/disable the Camera components instead of the game objects
        firstPersonCamera.GetComponent<Camera>().enabled = inFirstPersonZone;
        cam.enabled = !inFirstPersonZone;

        // Disable the IsometricCameraController script when in the first-person zone
        this.enabled = !inFirstPersonZone;

        if (enableCameraSwoops)
        {
            if (!inFirstPersonZone)
            {
                // Start the swooping effect after the FPS camera is disabled
                StartCoroutine(SwoopToInitialPosition());
            }
            else
            {
                // Swoop in when entering the first-person zone
                StartCoroutine(SwoopIn());
            }
        }
    }

    IEnumerator SwoopToInitialPosition()
    {
        float swoopDuration = 1f;
        float elapsedTime = 0f;

        Vector3 initialCameraPosition = transform.position;
        Vector3 targetCameraPosition = initialPosition;

        while (elapsedTime < swoopDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition, elapsedTime / swoopDuration);
            yield return null;
        }
    }


    // Swoop in the camera
    public IEnumerator SwoopIn()
    {
        while (Vector3.Distance(transform.position, target.position) > swoopInDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, cameraSwoopSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Swoop out the camera
    public IEnumerator SwoopOut()
    {
        while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, cameraSwoopSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void EnableFirstPersonCamera(bool enable)
    {
        firstPersonCamera.gameObject.SetActive(enable);
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