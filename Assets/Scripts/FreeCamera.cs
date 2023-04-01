using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The regular movement speed of the camera.")]
    public float moveSpeed = 5f;
    [Tooltip("The sprinting movement speed of the camera.")]
    public float sprintSpeed = 10f;

    [Header("Mouse Settings")]
    [Tooltip("The sensitivity of the mouse movement.")]
    public float mouseSensitivity = 2f;

    // Private variables to store camera rotation
    private float pitch = 0f;
    private float yaw = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Camera rotation
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Camera movement
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // Move camera up when space bar is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        }

        // Speed up camera movement when shift key is pressed
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveX *= sprintSpeed;
            moveZ *= sprintSpeed;
        }

        // Move camera based on input
        transform.Translate(new Vector3(moveX, 0, moveZ));
    }
}