using UnityEngine;
using Opsive.UltimateCharacterController.Character;

[RequireComponent(typeof(UltimateCharacterLocomotion))]
public class CameraRelativeMovement : MonoBehaviour
{
    private UltimateCharacterLocomotion m_CharacterLocomotion;

    private void Start()
    {
        m_CharacterLocomotion = GetComponent<UltimateCharacterLocomotion>();
    }

    private void Update()
    {
        UpdateInputVector();
    }

    private void UpdateInputVector()
    {
        Camera activeCamera = GetActiveCamera();
        if (activeCamera == null)
        {
            return;
        }

        Vector3 cameraForward = Vector3.Scale(activeCamera.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(activeCamera.transform.right, new Vector3(1, 0, 1)).normalized;

        Vector2 inputVector = m_CharacterLocomotion.RawInputVector;
        Vector3 newInputVector = (cameraRight * inputVector.x) + (cameraForward * inputVector.y);

        m_CharacterLocomotion.InputVector = new Vector2(newInputVector.x, newInputVector.z);
    }

    private Camera GetActiveCamera()
    {
        Camera[] cameras = Camera.allCameras;
        foreach (var camera in cameras)
        {
            if (camera.enabled)
            {
                return camera;
            }
        }
        return null;
    }
}
