using UnityEngine;

public class LaserCaster : MonoBehaviour
{
    public LayerMask hitLayers;
    public float rotationSpeed = 10f;
    public TargetController targetController;

    private Camera playerCamera;
    private bool isActivated;

    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (isActivated)
        {
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.right, mouseY);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, hitLayers))
            {
                if (hit.collider.CompareTag("Target"))
                {
                    targetController.Activate();
                }
                else
                {
                    targetController.Deactivate();
                }
            }
            else
            {
                targetController.Deactivate();
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
        targetController.Deactivate();
    }
}
