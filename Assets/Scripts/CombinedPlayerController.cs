using UnityEngine;

public class CombinedPlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
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

    private float boostTimer;
    private Vector3 boostDirection;


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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        remainingJumps = 2;
        wasGrounded = true;
    }

    void Update()
    {
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
        Vector3 targetDirection = mainCamera.transform.TransformDirection(new Vector3(horizontal, 0, vertical));
        targetDirection.y = 0;
        targetDirection.Normalize();

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
        Vector3 cameraRight = mainCamera.transform.right;
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0; // Make sure the forward direction is only in the XZ plane
        cameraForward.Normalize();

        if (other.CompareTag("RedCube"))
        {
            boostDirection = -cameraRight * xAxisBoostSpeed; // Negate the direction to reverse the boost
            boostTimer = boostDuration;
        }
        else if (other.CompareTag("BlueCube"))
        {
            boostDirection = cameraUp * yAxisBoostSpeed;
            boostTimer = boostDuration;
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

    public void ApplyBoost(Vector3 boost)
    {
        moveDirection += boost;
    }

    public Vector3 MoveDirection
    {
        get { return moveDirection; }
    }

    public void SetMoveDirection(Vector3 newMoveDirection)
    {
        moveDirection = newMoveDirection;
    }
}