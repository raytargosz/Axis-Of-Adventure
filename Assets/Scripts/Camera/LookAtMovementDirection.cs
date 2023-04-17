using UnityEngine;
using Opsive.UltimateCharacterController.Character;

[RequireComponent(typeof(UltimateCharacterLocomotion))]
public class LookAtMovementDirection : MonoBehaviour
{
    private UltimateCharacterLocomotion m_CharacterLocomotion;
    private CameraSwapZone m_CameraSwapZone;

    private void Start()
    {
        m_CharacterLocomotion = GetComponent<UltimateCharacterLocomotion>();
    }

    private void Update()
    {
        if (IsInCameraSwapZone())
        {
            RotateCharacterToFaceMovementDirection();
        }
    }

    private bool IsInCameraSwapZone()
    {
        m_CameraSwapZone = FindObjectOfType<CameraSwapZone>();
        if (m_CameraSwapZone == null)
        {
            return false;
        }

        return m_CameraSwapZone.IsInZone;
    }

    private void RotateCharacterToFaceMovementDirection()
    {
        Vector3 moveDirection = new Vector3(m_CharacterLocomotion.RawInputVector.x, 0, m_CharacterLocomotion.RawInputVector.y);
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            m_CharacterLocomotion.transform.rotation = Quaternion.Lerp(m_CharacterLocomotion.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
