/*
 * CombinedPlayerController is a script that handles player movement, jumping, gravity, and falling death for a 3D platformer game.
 * This script also manages camera switching between the isometric and first-person views.
 * To use this script, attach it to the player character in your scene and configure the public variables as needed.
 */


using System.Collections;
using UnityEngine;

public class CombinedPlayerController : MonoBehaviour
{
    [Header("Main Camera")]
    [Tooltip("Main camera object for the scene")]
    public Camera mainCamera;

    [Header("Ground Detection")]
    [Tooltip("Layer mask to define ground layers")]
    public LayerMask groundLayer;
    [Tooltip("Distance of the ground check")]
    public float groundCheckDistance = 0.1f;
    [Tooltip("Radius of the ground check sphere")]
    public float groundCheckRadius = 0.25f;

    [Header("Movement")]
    [Tooltip("Character's movement speed")]
    public float moveSpeed = 6f;
    [Tooltip("Character's sprint speed")]
    public float sprintSpeed = 9f;
    [Tooltip("Character's acceleration rate")]
    public float acceleration = 999f;
    [Tooltip("Character's deceleration rate")]
    public float deceleration = 28f;

    [Header("Jumping")]
    [Tooltip("Initial force of the first jump")]
    public float initialJumpForce = 9f;
    [Tooltip("Sustained force of the first jump while holding the jump button")]
    public float sustainedJumpForce = 6f;
    [Tooltip("Maximum duration the jump button can be held for sustained jump force")]
    public float maxJumpTime = 0.5f;
    [Tooltip("Force of the double jump")]
    public float doubleJumpForce = 10f;

    [Header("Gravity")]
    [Tooltip("Multiplier for the character's gravity")]
    public float gravityMultiplier = 2f;

    [Header("Audio")]
    [Tooltip("Audio clip for the first jump")]
    public AudioClip jumpSound;
    [Tooltip("Audio clip for the double jump")]
    public AudioClip doubleJumpSound;
    [Tooltip("Audio clip for landing")]
    public AudioClip landingSound;

    [Header("Falling Death")]
    [Tooltip("Maximum duration the character can fall before triggering death")]
    public float maxFallingTime = 3f;
    private float currentFallingTime = 0f;

    [Header("Boost")]
    [Tooltip("X-axis boost speed")]
    public float xAxisBoostSpeed = 20f;
    [Tooltip("Y-axis boost speed")]
    public float yAxisBoostSpeed = 15f;
    [Tooltip("Boost duration")]
    public float boostDuration = 0.5f;

    [Header("First Person Camera")]
    [Tooltip("Camera object for first-person view")]
    public Camera firstPersonCamera;
    [Tooltip("Mouse sensitivity for the first-person camera")]
    public float mouseSensitivity = 100f;
    private bool firstPersonMode = false;
    private float xRotation = 0f;

    [Header("Mesh Renderer")]
    [Tooltip("Player's mesh renderers")]
    public MeshRenderer[] playerMeshRenderers;

    [Header("Custom Cursor")]
    [Tooltip("Custom cursor texture")]
    public Texture2D customCursorTexture;

    [Header("Camera Controllers")]
    [Tooltip("Isometric camera controller script")]
    public IsometricCameraController isometricCameraController;

    [SerializeField]
    private PlayerDeath playerDeath;

    private AudioSource audioSource;
    private CharacterController controller;
    private Vector3 moveDirection;
    private bool isGrounded;
    private float jumpTime;
    private int remainingJumps;
    private bool wasGrounded;
    private BobbingObject bobbingObject;
    private bool isMenuActive = false;
    private bool isCursorVisible = false;

    private float boostTimer;
    private Vector3 boostDirection;

    void SetCustomCursor(Texture2D cursorTexture)
    {
        if (cursorTexture == null)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Vector2 hotSpot = new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        }
    }

    private void ToggleCursorVisibility()
    {
        isCursorVisible = !isCursorVisible;
        if (isCursorVisible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SetCustomCursor(customCursorTexture);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetCustomCursor(null);
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        remainingJumps = 2;
        wasGrounded = true;

        firstPersonCamera.enabled = false;
    }

    void Update()
    {
        // Check for ESC key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isMenuActive = !isMenuActive;
            ToggleCursorVisibility();
        }

        if (isCursorVisible && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            ToggleCursorVisibility();
        }

        if (firstPersonMode)
        {
            UpdateFirstPersonCamera();
        }

        if (isCursorVisible && Input.GetMouseButtonDown(0))
        {
            ToggleCursorVisibility();
        }

        float bobbingObjectVerticalMovement = GetBobbingObjectVerticalMovement();

        UpdateMoveDirection();
        UpdateFallingDeath();
        UpdateGroundedStatus();
        UpdateJump();

        controller.Move((moveDirection + new Vector3(0, bobbingObjectVerticalMovement, 0)) * Time.deltaTime);

        ApplyBoost();
    }

    private float GetBobbingObjectVerticalMovement()
    {
        return (isGrounded && bobbingObject != null && IsStandingOnBobbingObject())
            ? bobbingObject.GetVerticalMovement()
            : 0f;
    }

    private void UpdateMoveDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 targetDirection;

        if (firstPersonMode)
        {
            Vector3 forward = firstPersonCamera.transform.forward;
            Vector3 right = firstPersonCamera.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            targetDirection = (forward * vertical + right * horizontal).normalized;
        }
        else
        {
            Vector3 cameraRight = mainCamera.transform.right;
            cameraRight.y = 0;
            cameraRight.Normalize();

            targetDirection = cameraRight * horizontal;
        }

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? sprintSpeed : moveSpeed;

        Vector3 targetVelocity = targetDirection * targetSpeed;
        float accelerationRate = targetDirection.magnitude > 0 ? acceleration : deceleration;

        moveDirection.x = Mathf.MoveTowards(moveDirection.x, targetVelocity.x, accelerationRate * Time.deltaTime);
        moveDirection.z = Mathf.MoveTowards(moveDirection.z, targetVelocity.z, accelerationRate * Time.deltaTime);
    }


    private void UpdateFallingDeath()
    {
        if (!isGrounded && moveDirection.y < 0)
        {
            currentFallingTime += Time.deltaTime;
            if (currentFallingTime >= maxFallingTime)
            {
                playerDeath.TriggerDeathSequence();
            }
        }
        else
        {
            currentFallingTime = 0f;
        }
    }

    private void UpdateGroundedStatus()
    {
        wasGrounded = isGrounded;
        isGrounded = IsGrounded();

        if (isGrounded && !wasGrounded)
        {
            remainingJumps = 2;
            moveDirection.y = -0.1f;
        }
        else if (!isGrounded)
        {
            moveDirection.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }

    private void UpdateJump()
    {
        if (isGrounded)
        {
            remainingJumps = 2;
        }

        if (Input.GetButtonDown("Jump") && remainingJumps > 0)
        {
            PerformJump();
            jumpTime = maxJumpTime;
        }
        else if (Input.GetButton("Jump") && jumpTime > 0 && remainingJumps == 1)
        {
            moveDirection.y += sustainedJumpForce * Time.deltaTime;
            jumpTime -= Time.deltaTime;
        }

        if (!wasGrounded && isGrounded && landingSound != null)
        {
            audioSource.PlayOneShot(landingSound);
        }
    }


    private void PerformJump()
    {
        float jumpForce = (remainingJumps == 2) ? initialJumpForce : doubleJumpForce;
        moveDirection.y = jumpForce;
        remainingJumps--;

        // Play jump sound
        if (remainingJumps == 1 && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
        else if (remainingJumps == 0 && doubleJumpSound != null)
        {
            audioSource.PlayOneShot(doubleJumpSound);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 sphereCastOrigin = transform.position + controller.center + Vector3.up * (-controller.height / 2 + groundCheckRadius);
        bool result = Physics.SphereCast(sphereCastOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // Visualize SphereCast in Scene view
        Debug.DrawRay(sphereCastOrigin, Vector3.down * (groundCheckDistance + groundCheckRadius), Color.red, 0.05f);

        if (result)
        {
            BobbingObject hitBobbingObject = hit.collider.GetComponent<BobbingObject>();
            if (hitBobbingObject != null)
            {
                bobbingObject = hitBobbingObject;
            }
            else
            {
                bobbingObject = null;
            }
        }
        else
        {
            bobbingObject = null;
        }

        return result;
    }

    private bool IsStandingOnBobbingObject()
    {
        RaycastHit hit;
        Vector3 sphereCastOrigin = transform.position + controller.center + Vector3.up * (-controller.height / 2 + groundCheckRadius);
        bool result = Physics.SphereCast(sphereCastOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayer);

        if (result && hit.collider.gameObject == bobbingObject.gameObject)
        {
            return true;
        }

        return false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RedCube"))
        {
            boostDirection = moveDirection.normalized * xAxisBoostSpeed;
            boostDirection.y = 0; // Keep the current Y value
            boostTimer = boostDuration;
            remainingJumps = 2; // Reset jump count
        }
        else if (other.CompareTag("BlueCube"))
        {
            boostDirection = Vector3.up * yAxisBoostSpeed;
            boostDirection.x = 0; // Remove X momentum
            boostDirection.z = 0; // Remove Z momentum
            boostTimer = boostDuration;
            remainingJumps = 2; // Reset jump count
        }
        else if (other.CompareTag("FirstPersonZone"))
        {
            StartCoroutine(ToggleFirstPersonMode());
        }
    }

private void ApplyBoost()
    {
        if (boostTimer > 0)
        {
            controller.Move(boostDirection * Time.deltaTime);
            boostTimer -= Time.deltaTime;
        }
    }

    public Vector3 MoveDirection
    {
        get { return moveDirection; }
    }

    public void SetMoveDirection(Vector3 newMoveDirection)
    {
        moveDirection = newMoveDirection;
    }
    // New method for updating the first-person camera
    private void UpdateFirstPersonCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        firstPersonCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // New method for toggling the first-person camera mode
    IEnumerator ToggleFirstPersonMode()
    {
        //Debug.Log("Toggling first-person mode");

        firstPersonMode = !firstPersonMode;
        isometricCameraController.ToggleFirstPersonZone(firstPersonMode);

        // Enable/disable the player's mesh renderers
        foreach (MeshRenderer meshRenderer in playerMeshRenderers)
        {
            meshRenderer.enabled = !firstPersonMode;
        }

        if (firstPersonMode)
        {
            // Swoop in the isometric camera
            yield return StartCoroutine(isometricCameraController.SwoopIn());

            // Disable the isometric camera and enable the FPS camera
            isometricCameraController.enabled = false;
            firstPersonCamera.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Swoop out the isometric camera
            yield return StartCoroutine(isometricCameraController.SwoopOut());

            // Disable the FPS camera and enable the isometric camera
            isometricCameraController.enabled = true;
            firstPersonCamera.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Reset player's rotation
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FirstPersonZone"))
        {
            StartCoroutine(ToggleFirstPersonMode());
        }
    }
}