using UnityEngine;

public class PickupController : MonoBehaviour
{
    public float pickupDistance = 2f;
    public float holdDistance = 1f;
    public LayerMask pickupLayer;
    public DisplayControlsUI displayControlsUI;
    public AudioClip pickupSFX;
    public AudioClip dropSFX;
    public GameObject pickupVFXPrefab;
    public AudioSource audioSource;

    private GameObject heldObject;
    private Camera playerCamera;
    private SphereCollider pickupTrigger;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();

        // Create and configure the pickup trigger
        pickupTrigger = gameObject.AddComponent<SphereCollider>();
        pickupTrigger.isTrigger = true;
        pickupTrigger.radius = pickupDistance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            displayControlsUI.SetText("Press F to pick up");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            displayControlsUI.ClearText();
        }
    }

    void Update()
    {
        if (heldObject == null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, pickupDistance, pickupLayer);

                if (colliders.Length > 0)
                {
                    heldObject = colliders[0].gameObject;
                    heldObject.GetComponent<Rigidbody>().isKinematic = true;

                    // Reset rotation
                    heldObject.transform.rotation = Quaternion.identity;

                    // Play pickup SFX
                    audioSource.PlayOneShot(pickupSFX);

                    // Instantiate VFX
                    Instantiate(pickupVFXPrefab, heldObject.transform.position, Quaternion.identity);
                }
            }
        }
        else
        {
            HoldObject();

            if (Input.GetKeyDown(KeyCode.F))
            {
                DropObject();
            }
        }
    }

    private void HoldObject()
    {
        Vector3 holdPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
        heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, holdPosition, Time.deltaTime * 5f);
    }

    private void DropObject()
    {
        Rigidbody objectRigidbody = heldObject.GetComponent<Rigidbody>();
        objectRigidbody.isKinematic = false;

        // Play drop SFX
        audioSource.PlayOneShot(dropSFX);

        heldObject = null;
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

    public void HandleObjectDropAndThrow()
    {
        // This method is now empty, as the drop functionality is already handled in the Update method.
    }
}