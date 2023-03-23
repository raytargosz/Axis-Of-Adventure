using UnityEngine;

public class CombinedPlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float groundCheckRadius = 0.25f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;

    [Header("Jumping")]
    public float initialJumpForce = 4f;
    public float sustainedJumpForce = 2f;
    public float maxJumpTime = 0.2f;
    public float doubleJumpForce = 5f;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    public AudioClip landingSound;

    [Header("Falling Death")]
    public float maxFallingTime = 3f;
    private float currentFallingTime = 0f;

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
    }

    private float GetBobbingObjectVerticalMovement()
    {
        return (isGrounded && bobbingObject != null && IsStandingOnBobbingObject())
            ? bobbingObject.GetVerticalMovement()
            : 0f;
    }

    private void UpdateMoveDirection()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 relativeDirection = mainCamera.transform.TransformDirection(new Vector3(horizontal, 0, 0));
        relativeDirection.y = 0;
        relativeDirection.Normalize();

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)
            ? sprintSpeed
            : moveSpeed;

        moveDirection.x = relativeDirection.x * currentSpeed;
        moveDirection.z = relativeDirection.z * currentSpeed;
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
            moveDirection.y = Mathf.Lerp(moveDirection.y, Physics.gravity.y, Time.deltaTime);
        }
    }

    private void UpdateJump()
    {
        if (Input.GetButtonDown("Jump") && remainingJumps > 0)
        {
            PerformJump();
            jumpTime = maxJumpTime;
        }
        else if (Input.GetButton("Jump") && jumpTime > 0 && !isGrounded)
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
}