using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 180.0f;

    private Animator animator;
    private Vector3 moveDirection;
    private Transform characterTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterTransform = transform;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontal, 0, vertical);
        moveDirection.Normalize();

        animator.SetFloat("Speed", moveDirection.magnitude);

        if (moveDirection != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            characterTransform.rotation = Quaternion.RotateTowards(
                characterTransform.rotation,
                Quaternion.Euler(0, targetAngle, 0),
                rotationSpeed * Time.deltaTime
            );
        }

        characterTransform.position += moveDirection * speed * Time.deltaTime;
    }
}