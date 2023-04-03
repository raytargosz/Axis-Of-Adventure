/*
 * IsometricCameraController controls the camera's position, rotation, and zoom in an isometric view.
 * This script also handles switching between the isometric camera and the first-person camera when the player enters or exits specific zones.
 * To use this script, attach it to the main camera in your scene and configure the public variables as needed.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class IsometricCameraController : MonoBehaviour
{
    [Header("Camera Swooping")]
    [Tooltip("Speed of the camera swooping in and out")]
    public float cameraSwoopSpeed = 5f;

    [Header("Camera Swooping")]
    [Tooltip("Enable or disable camera swooping")]
    public bool cameraSwoopingEnabled = true;

    [Tooltip("Distance for the camera to swoop in")]
    public float swoopInDistance = 2f;

    [Header("Surface Audio Manager")]
    public SurfaceAudioManager surfaceAudioManager;

    [Header("Transition Speeds")]
    public float fpsToIsoSpeed = 1f;
    public float isoToFpsSpeed = 1f;

    [Header("Post Processing Effects")]
    public VolumeProfile postProcessingProfile;

    private Vector3 initialPosition;

    [Header("Camera Settings")]
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

    [Header("Audio")]
    public AudioClip rotateLeftSFX;
    public AudioClip rotateRightSFX;
    public AudioClip zoomInSFX;
    public AudioClip zoomOutSFX;
    public float rotationSFXVolume = 1f;
    public float scrollSFXVolume = 1f;
    public float sfxCooldown = 0.5f;

    [Header("First Person Camera")]
    public Camera firstPersonCamera;

    private Vector3 currentVelocity;
    private Camera cam;
    private float targetRotationAngle;
    private float rotationStartTime;
    private AudioSource audioSource;
    private float lastSFXTime;
    private bool inFirstPersonZone = false;
    private bool disableLookAtCharacter = false;

    [Header("Surface Audio Settings")]
    [SerializeField] private string surfaceTag = "Default";
    [SerializeField] private float jumpAndLandVolume = 1f;

    private CharacterController characterController;
    private Vector3 velocity;
    public float jumpForce = 10f;
    private bool isJumping;

    [Header("Character Controller")]
    public CharacterController playerCharacterController;

    private Vector3 originalOffset;
    private float originalRotationSpeed;
    private float originalSmoothingSpeed;
    private float originalRotationAngle;
    private float originalVerticalAngle;
    private float originalMinDistance;
    private float originalMaxDistance;
    private float originalMinFov;
    private float originalMaxFov;
    private float originalFovSpeed;

    private void SaveOriginalSettings()
    {
        originalOffset = offset;
        originalRotationSpeed = rotationSpeed;
        originalSmoothingSpeed = smoothingSpeed;
        originalRotationAngle = rotationAngle;
        originalVerticalAngle = verticalAngle;
        originalMinDistance = minDistance;
        originalMaxDistance = maxDistance;
        originalMinFov = minFov;
        originalMaxFov = maxFov;
        originalFovSpeed = fovSpeed;
    }


    public void Jump()
    {
        if (playerCharacterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            audioSource.PlayOneShot(surfaceAudioManager.GetJumpSound(surfaceTag), jumpAndLandVolume);
            isJumping = true;
        }
    }
    public void Land()
    {
        if (!playerCharacterController.isGrounded) return;

        velocity.y = 0;
        audioSource.PlayOneShot(surfaceAudioManager.GetLandSound(surfaceTag), jumpAndLandVolume);
        isJumping = false;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        targetRotationAngle = rotationAngle;
        rotationStartTime = -rotationDuration;
        audioSource = GetComponent<AudioSource>();
        lastSFXTime = Time.time;
        SaveOriginalSettings();

        // Store the initial position of the camera
        initialPosition = transform.position;

        // Get the CharacterController component from the player character
        playerCharacterController = target.GetComponent<CharacterController>();
        isJumping = false;
    }

    void Update()
    {
        if (target == null) return;

        UpdateRotation();
        UpdateZoom();

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            Jump();
        }
        else if (isJumping && playerCharacterController.isGrounded)
        {
            Land();
        }
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
        if (!cameraSwoopingEnabled) return;

        targetRotationAngle += 90f;
        rotationStartTime = Time.time;
        PlaySFX(rotateLeftSFX, rotationSFXVolume);
    }

    private void RotateRight()
    {
        if (!cameraSwoopingEnabled) return;

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

        if (!disableLookAtCharacter)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, updatedTargetRotation, Time.deltaTime * rotationSpeed);
        }
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
        StartCoroutine(CameraTransition());
    }

    private IEnumerator CameraTransition(bool enableFirstPerson, float rotationDuration)
    {
        float elapsed = 0f;

        Vector3 startCameraPosition = cam.transform.position;
        Quaternion startCameraRotation = cam.transform.rotation;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationDuration;
            float rotationDuration

            if (cameraSwoopingEnabled)
            {
                if (inFirstPersonZone)
                {
                    yield return StartCoroutine(SwoopIn());
                }
                else
                {
                    yield return StartCoroutine(SwoopOut());
                }
            }

            cam.transform.position = Vector3.Lerp(startCameraPosition, targetCameraPosition, t);
            cam.transform.rotation = Quaternion.Lerp(startCameraRotation, targetCameraRotation, t);
            yield return null;
        }

        cam.transform.position = targetCameraPosition;
        cam.transform.rotation = targetCameraRotation;

        IEnumerator SwoopIn()
        {
            Vector3 swoopDirection = targetCameraPosition - startCameraPosition;
            float swoopProgress = Mathf.Clamp01(elapsed / swoopDuration);
            return Swoop(swoopDirection, swoopProgress);
        }

        IEnumerator SwoopOut()
        {
            Vector3 swoopDirection = startCameraPosition - targetCameraPosition;
            float swoopProgress = Mathf.Clamp01((rotationDuration - elapsed) / swoopDuration);
            return Swoop(swoopDirection, swoopProgress);
        }

        IEnumerator Swoop(Vector3 direction, float progress)
        {
            float swoopAmount = swoopCurve.Evaluate(progress) * swoopMagnitude;
            cam.transform.position += direction.normalized * swoopAmount;
            yield return null;
        }
    }

    // Swoop in the camera
    public IEnumerator SwoopIn()
    {
        disableLookAtCharacter = true;

        while (Vector3.Distance(transform.position, target.position) > swoopInDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, cameraSwoopSpeed * Time.deltaTime);
            yield return null;
        }

        disableLookAtCharacter = false;
    }

    public IEnumerator SwoopOut()
    {
        disableLookAtCharacter = true;

        while (Vector3.Distance(transform.position, initialPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, cameraSwoopSpeed * Time.deltaTime);
            yield return null;
        }

        disableLookAtCharacter = false;
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

    public void UpdateCameraSettings(Vector3 newOffset, float newRotationSpeed, float newSmoothingSpeed, float newRotationAngle, float newVerticalAngle, float newMinDistance, float newMaxDistance, float newMinFov, float newMaxFov, float newFovSpeed, float rotationDuration)
    {
        // Save the original camera settings for resetting later
        SaveOriginalSettings();

        // Apply the new settings
        offset = newOffset;
        rotationSpeed = newRotationSpeed;
        smoothingSpeed = newSmoothingSpeed;
        rotationAngle = newRotationAngle;
        verticalAngle = newVerticalAngle;
        minDistance = newMinDistance;
        maxDistance = newMaxDistance;
        minFov = newMinFov;
        maxFov = newMaxFov;
        fovSpeed = newFovSpeed;

        // Smoothly transition the camera rotationDuration
        StartCoroutine(TransitionCameraSettings(rotationDuration));
    }

    public void ResetCameraSettings(float rotationDuration)
    {
        // Restore the original camera settings
        offset = originalOffset;
        rotationSpeed = originalRotationSpeed;
        smoothingSpeed = originalSmoothingSpeed;
        rotationAngle = originalRotationAngle;
        verticalAngle = originalVerticalAngle;
        minDistance = originalMinDistance;
        maxDistance = originalMaxDistance;
        minFov = originalMinFov;
        maxFov = originalMaxFov;
        fovSpeed = originalFovSpeed;

        // Smoothly transition the camera settings
        StartCoroutine(TransitionCameraSettings(rotationDuration));
    }

    private IEnumerator TransitionCameraSettings(float rotationDuration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        float startFov = cam.fieldOfView;

        Vector3 targetPosition = target.position - Quaternion.Euler(verticalAngle, rotationAngle, 0) * Vector3.forward * distance + offset;
        Quaternion targetRotation = Quaternion.Euler(verticalAngle, rotationAngle, 0);
        float targetFov = Mathf.Clamp(cam.fieldOfView, minFov, maxFov);

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            cam.fieldOfView = Mathf.Lerp(startFov, targetFov, t);

            yield return null;
        }

        // Set the final values to ensure they match the target values
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        cam.fieldOfView = targetFov;
    }
}