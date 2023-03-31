using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private CombinedPlayerController playerMovement;

    [SerializeField] private float animationSmoothTime = 0.1f;

    private bool isMoving;
    private bool isRunning;
    private bool isJumping;
    private bool isFalling;

    void Update()
    {
        UpdateAnimationState();
        UpdateAnimatorParameters();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<CombinedPlayerController>();
    }

    private void UpdateAnimationState()
    {
        Vector3 moveDirection = playerMovement.MoveDirection;
        isMoving = moveDirection.magnitude > 0 && moveDirection.y == 0;
        isRunning = isMoving && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        isJumping = playerMovement.remainingJumps == 1;
        isFalling = !playerMovement.IsGrounded() && moveDirection.y < 0;
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("IsFalling", isFalling);

        animator.SetFloat("Blend", isRunning ? 1.0f : 0.0f, animationSmoothTime, Time.deltaTime);
    }
}
