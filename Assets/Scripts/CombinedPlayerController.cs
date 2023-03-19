using UnityEngine;

public class CombinedPlayerController : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float groundCheckRadius = 0.25f;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jumping")]
    public float initialJumpForce = 4f;
    public float sustainedJumpForce = 2f;
    public float maxJumpTime = 0.2f;
    public float doubleJumpForce = 5f;

    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip doubleJumpSound;
    public AudioClip landingSound;

    private AudioSource audioSource;
    private CharacterController controller;
    private Vector3 moveDirection;
    private bool isGrounded;
    private float jumpTime;
    private int remainingJumps;
    private bool wasGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = gameObject.AddComponent<AudioSource>();
        remainingJumps = 2;
        wasGrounded = true;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 relativeDirection = mainCamera.transform.TransformDirection(new Vector3(horizontal, 0, 0));
        relativeDirection.y = 0;
        relativeDirection.Normalize();

        moveDirection.x = relativeDirection.x * moveSpeed;
        moveDirection.z = relativeDirection.z * moveSpeed;

        isGrounded = IsGrounded();

        // Log if the player is grounded
        Debug.Log("Player is grounded: " + isGrounded);

        if (isGrounded && !wasGrounded)
        {
            remainingJumps = 2;
            moveDirection.y = -0.1f;
        }
        else if (!isGrounded)
        {
            moveDirection.y = Mathf.Lerp(moveDirection.y, Physics.gravity.y, Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && remainingJumps > 0)
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

        wasGrounded = isGrounded;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        Vector3 sphereCastOrigin = transform.position + controller.center + Vector3.up * (-controller.height / 2 + groundCheckRadius);
        bool result = Physics.SphereCast(sphereCastOrigin, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundLayer);

        // Visualize SphereCast in Scene view
        Debug.DrawRay(sphereCastOrigin, Vector3.down * (groundCheckDistance + groundCheckRadius), Color.red, 0.1f);

        return result;
    }
}
