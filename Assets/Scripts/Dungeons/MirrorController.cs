using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script controls the behavior of mirror objects, allowing the player to interact with them.
[RequireComponent(typeof(BoxCollider))]
public class MirrorController : MonoBehaviour
{
    [Header("Mirror Settings")]
    [Tooltip("Rotation speed of the mirror when controlled by the player.")]
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Interaction Settings")]
    [Tooltip("The distance at which the player can interact with the mirror.")]
    [SerializeField] private float interactionDistance = 2f;

    [Space]
    [Tooltip("The layer of the player used to detect interaction.")]
    [SerializeField] private LayerMask playerLayer;

    private Camera mainCamera;
    private bool isInteracting;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandlePlayerInteraction();
    }

    // Detect player interaction and rotate the mirror accordingly.
    private void HandlePlayerInteraction()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button pressed.
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, interactionDistance, playerLayer))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    isInteracting = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // Left mouse button released.
        {
            isInteracting = false;
        }

        if (isInteracting)
        {
            float rotationInput = Input.GetAxis("Horizontal");
            RotateMirror(rotationInput);
        }
    }

    // Rotate the mirror based on player input.
    private void RotateMirror(float rotationInput)
    {
        transform.Rotate(Vector3.up, rotationSpeed * rotationInput * Time.deltaTime);
    }
}
