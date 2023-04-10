using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Camera;


public class CameraRotate : MonoBehaviour
{
    public Transform player;
    public Camera rotatingCamera;
    public float rotationSpeed = 5.0f;
    public KeyCode rotateRightKey = KeyCode.E;
    public KeyCode rotateLeftKey = KeyCode.Q;

    public static bool DisablePositionReset;

    private CameraControllerHandler cameraControllerHandler;
    private List<MonoBehaviour> cameraScripts;
    private bool isRotating;

    private void Start()
    {
        cameraScripts = new List<MonoBehaviour>();
        cameraControllerHandler = rotatingCamera.GetComponent<CameraControllerHandler>();

        foreach (var script in rotatingCamera.GetComponents<MonoBehaviour>())
        {
            if (script != this)
            {
                cameraScripts.Add(script);
            }
        }
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player transform is not assigned. Please assign it in the inspector.");
            return;
        }

        if (rotatingCamera == null)
        {
            Debug.LogWarning("Camera is not assigned. Please assign it in the inspector.");
            return;
        }

        if (Input.GetKey(rotateRightKey) || Input.GetKey(rotateLeftKey))
        {
            if (!isRotating)
            {
                DisableOtherCameraScripts();
                cameraControllerHandler.DisablePositionReset = true;
                isRotating = true;
            }

            float rotationDirection = Input.GetKey(rotateRightKey) ? 1 : -1;
            rotatingCamera.transform.RotateAround(player.position, Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);
        }
        else if (isRotating)
        {
            EnableOtherCameraScripts();
            cameraControllerHandler.DisablePositionReset = false;
            isRotating = false;
        }
    }

    private void DisableOtherCameraScripts()
    {
        foreach (var script in cameraScripts)
        {
            script.enabled = false;
        }
    }

    private void EnableOtherCameraScripts()
    {
        foreach (var script in cameraScripts)
        {
            script.enabled = true;
        }
    }
}