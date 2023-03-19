using UnityEngine;

public class FreeGimbalCamera : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 2f;
    private Vector2 rotation;
    private Quaternion initialRotation;
    private bool wasHoldingShift;
    private bool rotateXAxis = true; // Default to X axis rotation

    private void Start()
    {
        initialRotation = transform.localRotation;
    }

    private void Update()
    {
        bool isHoldingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            rotateXAxis = !rotateXAxis;
        }

        if (isHoldingShift)
        {
            if (Input.GetMouseButton(0))
            {
                if (rotateXAxis)
                {
                    rotation.x += Input.GetAxis("Mouse X") * rotationSpeed;
                }
                else
                {
                    rotation.y -= Input.GetAxis("Mouse Y") * rotationSpeed;
                    rotation.y = Mathf.Clamp(rotation.y, -90f, 90f);
                }

                transform.localRotation = Quaternion.Euler(rotation.y, rotation.x, 0);
            }
        }
        else if (wasHoldingShift)
        {
            transform.localRotation = initialRotation;
            rotation = initialRotation.eulerAngles; // Update rotation to match initialRotation
        }

        wasHoldingShift = isHoldingShift;
    }
}
