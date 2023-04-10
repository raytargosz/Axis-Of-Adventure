using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Camera;

public class CameraRotate : MonoBehaviour
{
    public Transform player;
    public Camera rotatingCamera;
    public float rotationDuration = 0.5f;
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

        if ((Input.GetKeyDown(rotateRightKey) || Input.GetKeyDown(rotateLeftKey)) && !isRotating)
        {
            float rotationDirection = Input.GetKeyDown(rotateRightKey) ? 1 : -1;
            StartCoroutine(RotateCamera(rotationDirection));
        }
    }

    private IEnumerator RotateCamera(float direction)
    {
        isRotating = true;
        DisableOtherCameraScripts();
        cameraControllerHandler.DisablePositionReset = true;

        Quaternion startRotation = rotatingCamera.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0, 90 * direction, 0);

        float elapsedTime = 0;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / rotationDuration;
            rotatingCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
            rotatingCamera.transform.position = player.position - (rotatingCamera.transform.rotation * Vector3.forward * 10 + new Vector3(0, 3, 0));

            yield return null;
        }

        isRotating = false;
        cameraControllerHandler.DisablePositionReset = true;
        EnableOtherCameraScripts();
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
