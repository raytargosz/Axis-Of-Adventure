using UnityEngine;

public class IsometricPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Camera mainCamera;
    public float jumpForce = 5f;
    public float holdJumpForce = 7f;
    public float doubleJumpForce = 5f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool canDoubleJump;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 relativeDirection = mainCamera.transform.TransformDirection(new Vector3(horizontal, 0, 0));
        relativeDirection.y = 0;
        relativeDirection.Normalize();

        rb.velocity = new Vector3(relativeDirection.x * moveSpeed, rb.velocity.y, relativeDirection.z * moveSpeed);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            canDoubleJump = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || canDoubleJump))
        {
            float jumpForceToApply = isGrounded ? jumpForce : doubleJumpForce;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForceToApply, ForceMode.Impulse);

            if (!isGrounded)
            {
                canDoubleJump = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * holdJumpForce / jumpForce, rb.velocity.z);
        }
    }
}
