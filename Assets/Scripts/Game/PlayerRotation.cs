using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    [SerializeField] private float rotationSpeed = 45f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            RotatePlayer(Vector3.up);
        }
        else if (Input.GetKey(KeyCode.T))
        {
            RotatePlayer(Vector3.down);
        }
    }

    private void RotatePlayer(Vector3 axis)
    {
        float step = rotationSpeed * Time.deltaTime;
        transform.RotateAround(pivot.position, axis, step);
    }
}
