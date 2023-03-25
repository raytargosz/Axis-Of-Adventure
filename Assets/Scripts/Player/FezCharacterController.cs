using UnityEngine;

public class FezCharacterController : MonoBehaviour
{
    [Tooltip("The character's movement speed")]
    [SerializeField] private float moveSpeed = 5f;

    [Tooltip("The initial force applied when jumping")]
    [SerializeField] private float initialJumpForce = 4f;

    [Tooltip("The sustained force applied when holding down the jump button")]
    [SerializeField] private float sustainedJumpForce = 2f;

    [Tooltip("The maximum time the player can hold down the jump button")]
    [SerializeField] private float maxJumpTime = 0.2f;

    [Tooltip("The strength of gravity applied to the character")]
    [SerializeField] private float gravityScale = 2f;

    [Tooltip("The sound played when jumping")]
    [SerializeField] private AudioClip jumpSound;

    [Tooltip("The sound played when double jumping")]
    [SerializeField] private AudioClip doubleJumpSound;

    [Tooltip("The sound played when landing on the ground")]
    [SerializeField] private AudioClip landingSound;

    private AudioSource audioSource;
    private Rigidbody rb;
    private bool isGrounded;
    private float jumpTime;
    private int remainingJumps;
    private bool wasGrounded;
    private Vector3 moveRight;
    private Vector3 moveForward;

    public void SetMoveDirection(Vector3 right, Vector3 forward)
    {
        moveRight = new Vector3(right.x, 0, right.z).normalized;
        moveForward = new Vector3(forward.x, 0, forward.z).normalized;
    }


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();
        remainingJumps = 2;
        wasGrounded = true;
    }

    private void Update()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (isHoldingShift)
        {
            return; // Early exit from the Update method, preventing player movement and rotation
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            return; // Disable movement when holding Shift
        }

        float moveDirection = Input.GetAxis("Horizontal") * moveSpeed;
        rb.velocity = moveRight * Input.GetAxis("Horizontal") * moveSpeed + new Vector3(0, rb.velocity.y, 0);


        if (Input.GetButtonDown("Jump") && remainingJumps > 0)
        {
            float jumpForce = initialJumpForce;
            if (remainingJumps == 2)
            {
                if (jumpSound != null)
                {
                    audioSource.PlayOneShot(jumpSound);
                }
            }
            else
            {
                jumpForce *= 0.5f; // 50% of the first jump
                if (doubleJumpSound != null)
                {
                    audioSource.PlayOneShot(doubleJumpSound);
                }
            }

            rb.velocity = new Vector3(rb.velocity.x, 0, 0); // Reset the vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            remainingJumps--;
            jumpTime = maxJumpTime;
        }
        else if (Input.GetButton("Jump") && jumpTime > 0)
        {
            rb.AddForce(Vector3.up * sustainedJumpForce, ForceMode.Force);
            jumpTime -= Time.deltaTime;
        }

        if (!wasGrounded && isGrounded && landingSound != null)
        {
            audioSource.PlayOneShot(landingSound);
        }
        wasGrounded = isGrounded;
    }

    private void FixedUpdate()
    {
        rb.AddForce(Vector3.down * (gravityScale - 1) * rb.mass * Physics.gravity.y);
    }

    private void OnCollisionEnter(Collision collision)
    {
        UpdateGroundedState(collision, true);
    }

    private void OnCollisionStay(Collision collision)
    {
        UpdateGroundedState(collision, true);
    }

    private void OnCollisionExit(Collision collision)
    {
        UpdateGroundedState(collision, false);
    }

    private void UpdateGroundedState(Collision collision, bool state)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = state;
            if (isGrounded)
            {
                remainingJumps = 2;
            }
        }
    }
}