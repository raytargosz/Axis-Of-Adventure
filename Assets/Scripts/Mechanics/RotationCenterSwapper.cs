using UnityEngine;

public class RotationCenterSwapper : MonoBehaviour
{
    [Tooltip("The WorldRotation script that controls the rotation of the world")]
    [SerializeField] private WorldRotation worldRotation;

    [Tooltip("The new rotation center that will be used when the player enters the trigger")]
    [SerializeField] private Transform newRotationCenter;

    [Tooltip("The old rotation center that will be used when the player enters the trigger again")]
    [SerializeField] private Transform oldRotationCenter;

    private bool isUsingNewRotationCenter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isUsingNewRotationCenter)
            {
                worldRotation.SetRotationCenter(oldRotationCenter);
            }
            else
            {
                worldRotation.SetRotationCenter(newRotationCenter);
            }

            isUsingNewRotationCenter = !isUsingNewRotationCenter;
        }
    }
}