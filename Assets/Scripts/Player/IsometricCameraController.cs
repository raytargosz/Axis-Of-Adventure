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

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            audioSource.PlayOneShot(surfaceAudioManager.GetJumpSound(surfaceTag), jumpAndLandVolume);
            isJumping = true;
        }
    }

    public void Land()
    {
        if (!characterController.isGrounded) return;

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

        // Store the initial position of the camera
        initialPosition = transform.position;

        characterController = GetComponent<CharacterController>();
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
        else if (isJumping && characterController.isGrounded)
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
        StartCoroutine(CameraTransition(enableFirstPerson));
    }

    IEnumerator CameraTransition(bool enableFirstPerson)
    {
    // Get the Exposure and MotionBlur settings
    //Exposure exposureSettings;
    //MotionBlur motionBlurSettings;

    //postProcessingProfile.TryGet(out exposureSettings);
    //postProcessingProfile.TryGet(out motionBlurSettings);

    // Enable the post-processing effects
    //exposureSettings.active = true;
    //motionBlurSettings.active = true;

    float transitionDuration = enableFirstPerson ? isoToFpsSpeed : fpsToIsoSpeed; // Use the appropriate speed variable
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

        disableLookAtCharacter = true;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / transitionDuration); // Use SmoothStep for easing

            transform.position = Vector3.Lerp(initialCameraPosition, targetCameraPosition, t);
            transform.rotation = Quaternion.Lerp(initialCameraRotation, targetCameraRotation, t);

            yield return null;
        }

        disableLookAtCharacter = false;

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

    // Disable the post-processing effects
    //exposureSettings.active = false;
    //motionBlurSettings.active = false;
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
}