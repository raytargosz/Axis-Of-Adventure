using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public float pickupDistance = 2f;
    public float holdDistance = 1f;
    public float throwForce = 500f;
    public LayerMask pickupLayer;

    private GameObject heldObject;
    private Camera playerCamera;
    private CombinedPlayerController combinedPlayerController;
    private DisplayControlsUI displayControlsUI;

    private string fpsDropText = "Press LMB or F to drop";
    private string fpsDropThrowText = "Press LMB or F to drop / RMB to throw";
    private string isoDropText = "Press LMB or F to drop";

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
        combinedPlayerController = GetComponent<CombinedPlayerController>();
        displayControlsUI = FindObjectOfType<DisplayControlsUI>();
    }

    private void Update()
    {
        if (heldObject == null)
        {
            CheckForPickup();
        }
        else
        {
            HoldObject();
            HandleObjectDropAndThrow();
        }
    }


    private void CheckForPickup()
    {
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        bool isFPSMode = combinedPlayerController.FirstPersonMode;

        if (Physics.Raycast(ray, out hit, pickupDistance, pickupLayer))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true;
                displayControlsUI.SetText(isFPSMode ? fpsDropText : isoDropText);
            }
        }
    }


    private void HoldObject()
    {
        Vector3 holdPosition = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
        Rigidbody heldObjectRigidbody = heldObject.GetComponent<Rigidbody>();
        heldObjectRigidbody.velocity = (holdPosition - heldObject.transform.position) * 5f;
    }


    public void HandleObjectDropAndThrow()
    {
        bool isFPSMode = combinedPlayerController.FirstPersonMode;

        // Drop object with left mouse button (LMB) or F key
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Drop input detected");
            DropObject();
            displayControlsUI.SetText(combinedPlayerController.FirstPersonMode ? fpsDropText : isoDropText);
        }

        // Throw object with right mouse button (RMB) in FPS mode only
        if (isFPSMode && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Throw input detected");
            ThrowObject();
            displayControlsUI.SetText(fpsDropThrowText);
        }
    }

    private void DropObject()
    {
        Rigidbody objectRigidbody = heldObject.GetComponent<Rigidbody>();
        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;
        heldObject = null;
        displayControlsUI.SetText("");
    }

    private void ThrowObject()
    {
        Rigidbody objectRigidbody = heldObject.GetComponent<Rigidbody>();
        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;
        objectRigidbody.AddForce(playerCamera.transform.forward * throwForce);
        heldObject = null;
        displayControlsUI.SetText("");
    }

}