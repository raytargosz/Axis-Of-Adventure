using UnityEngine;

public class LaserCaster : MonoBehaviour
{
    public LayerMask hitLayers;
    public float rotationSpeed = 10f;
    public TargetController[] targetControllers;

    private Camera playerCamera;
    private bool isActivated;

    private void Start()
    {
        playerCamera = Camera.main;

        // Disable all TargetController scripts
        foreach (TargetController target in targetControllers)
        {
            target.enabled = false;
        }
    }

    private void Update()
    {
        if (isActivated)
        {
            // Check if the player is pressing the left or right mouse button
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
                transform.Rotate(Vector3.right, mouseY);
            }

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, hitLayers))
            {
                if (hit.collider.CompareTag("Target"))
                {
                    foreach (TargetController target in targetControllers)
                    {
                        if (target.gameObject.GetInstanceID() == hit.collider.gameObject.GetInstanceID())
                        {
                            target.Activate();
                        }
                    }
                }
                else
                {
                    foreach (TargetController target in targetControllers)
                    {
                        target.Deactivate();
                    }
                }
            }
            else
            {
                foreach (TargetController target in targetControllers)
                {
                    target.Deactivate();
                }
            }
        }
    }

    public void Activate()
    {
        isActivated = true;
    }

    public void Deactivate()
    {
        isActivated = false;
        foreach (TargetController target in targetControllers)
        {
            target.Deactivate();
        }
    }
}
