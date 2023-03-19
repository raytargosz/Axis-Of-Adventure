using System.Collections;
using UnityEngine;

public class WorldRotation : MonoBehaviour
{
    [Tooltip("The main camera that will be controlled by this script")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("The speed at which the world rotates")]
    [SerializeField] private float rotationSpeed = 2f;

    [Tooltip("The sound played when rotating the world to the right")]
    [SerializeField] private AudioClip rotateRightSound;

    [Tooltip("The sound played when rotating the world to the left")]
    [SerializeField] private AudioClip rotateLeftSound;

    [Tooltip("The speed at which the camera zooms in and out")]
    [SerializeField] private float zoomSpeed = 5f;

    [Tooltip("The minimum field of view for the camera")]
    [SerializeField] private float minFOV = 30f;

    [Tooltip("The maximum field of view for the camera")]
    [SerializeField] private float maxFOV = 90f;

    [Tooltip("The sound played when zooming in")]
    [SerializeField] private AudioClip zoomInSound;

    [Tooltip("The sound played when zooming out")]
    [SerializeField] private AudioClip zoomOutSound;

    [Tooltip("The transform that serves as the center of rotation for the world")]
    [SerializeField] public Transform rotationCenter;

    [Tooltip("An array of transforms that serve as centers of rotation for the world")]
    [SerializeField] private Transform[] rotationCenters;

    [Tooltip("The CameraSwoop component that handles camera movement at beginning")]
    [SerializeField] private CameraSwoop cameraSwoop;

    private Transform playerTransform;
    private AudioSource audioSource;
    private float targetRotationY;
    private bool isRotating;
    private float lastMouseScroll;

    public bool CanRotate { get; set; } = true;

    private void RotatePlayer(float degrees)
    {
        // Find the closest rotation center to the player
        Transform closestRotationCenter = rotationCenters[0];
        float minDistance = Vector3.Distance(playerTransform.position, closestRotationCenter.position);

        foreach (Transform rotationCenter in rotationCenters)
        {
            float distance = Vector3.Distance(playerTransform.position, rotationCenter.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRotationCenter = rotationCenter;
            }
        }

        // Calculate the vector from the closest rotation center to the player
        Vector3 centerToPlayer = playerTransform.position - closestRotationCenter.position;

        // Rotate the vector around the Y-axis
        centerToPlayer = Quaternion.Euler(0, degrees, 0) * centerToPlayer;

        // Update the player's position based on the rotated vector
        playerTransform.position = closestRotationCenter.position + centerToPlayer;

        // Rotate the player's transform around the Y-axis
        playerTransform.rotation *= Quaternion.Euler(0, degrees, 0);
    }


    public void SetActiveRotationCenter(Transform newRotationCenter)
    {
        rotationCenter = newRotationCenter;
    }

    public void SetRotationCenter(Transform newRotationCenter)
    {
        rotationCenter = newRotationCenter;
    }

    public Transform FindClosestWorldRotation()
    {
        Transform closestWorldRotation = rotationCenters[0];
        float minDistance = Vector3.Distance(transform.position, closestWorldRotation.position);

        foreach (Transform worldRotation in rotationCenters)
        {
            float distance = Vector3.Distance(transform.position, worldRotation.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWorldRotation = worldRotation;
            }
        }

        return closestWorldRotation;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lastMouseScroll = 0;
        playerTransform = FindObjectOfType<FezCharacterController>().transform;
    }

    private void Update()
    {

        if (cameraSwoop != null && cameraSwoop.IsSwooping)
        {
            return;
        }

        if (!CanRotate) return;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            return; // Disable rotation when holding Shift
        }

        if (Input.GetMouseButtonDown(0) && !isRotating)
        {
            RotateWorld(-90, rotateRightSound);
        }
        else if (Input.GetMouseButtonDown(1) && !isRotating)
        {
            RotateWorld(90, rotateLeftSound);
        }

        float scrollDirection = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDirection != 0)
        {
            float newFOV = Mathf.Clamp(mainCamera.fieldOfView - scrollDirection * zoomSpeed, minFOV, maxFOV);
            mainCamera.fieldOfView = newFOV;

            if (scrollDirection > 0 && lastMouseScroll <= 0)
            {
                PlayZoomSound(zoomInSound);
            }
            else if (scrollDirection < 0 && lastMouseScroll >= 0)
            {
                PlayZoomSound(zoomOutSound);
            }

            lastMouseScroll = scrollDirection;
        }
    }

    private void FixedUpdate()
    {
        if (isRotating)
        {
            float step = rotationSpeed * Time.fixedDeltaTime;
            float currentRotationY = rotationCenter.transform.rotation.eulerAngles.y;
            float newRotationY = Mathf.MoveTowardsAngle(currentRotationY, targetRotationY, step);
            rotationCenter.transform.rotation = Quaternion.Euler(0, newRotationY, 0);

            if (Mathf.Abs(Mathf.DeltaAngle(newRotationY, targetRotationY)) <= 0.1f)
            {
                rotationCenter.transform.rotation = Quaternion.Euler(0, targetRotationY, 0);
                isRotating = false;
            }
        }
    }

    private Transform originalParent;

    private void RotateWorld(float degrees, AudioClip sound)
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }

        // Store player's original parent
        originalParent = playerTransform.parent;

        // Find the closest rotation center to the player
        Transform closestRotationCenter = rotationCenters[0];
        float minDistance = Vector3.Distance(playerTransform.position, closestRotationCenter.position);

        foreach (Transform rotationCenter in rotationCenters)
        {
            float distance = Vector3.Distance(playerTransform.position, rotationCenter.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRotationCenter = rotationCenter;
            }
        }

        // Make the player a temporary child of the closest WorldRotationCenter
        playerTransform.SetParent(closestRotationCenter);

        float currentRotationY = closestRotationCenter.transform.rotation.eulerAngles.y;
        targetRotationY = Mathf.Round(currentRotationY / 90) * 90 + degrees;
        isRotating = true;

        // Reset player's parent after rotation
        StartCoroutine(ResetPlayerParent());
    }

    private IEnumerator ResetPlayerParent()
    {
        while (isRotating)
        {
            yield return null;
        }

        // Reset the player's parent
        playerTransform.SetParent(originalParent);
    }


    private void PlayZoomSound(AudioClip sound)
    {
        if (audioSource != null && sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }
}