using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("The transform to follow")]
    [SerializeField] private Transform player;

    [Tooltip("The smooth speed of the camera movement")]
    [SerializeField] private float smoothSpeed = 0.125f;

    [Tooltip("The offset from the player's position")]
    [SerializeField] private Vector3 offset;

    [Tooltip("The array of world rotations to choose from")]
    [SerializeField] private WorldRotation[] worldRotations;

    private WorldRotation currentWorldRotation;

    private void Start()
    {
        currentWorldRotation = FindClosestWorldRotation();
        currentWorldRotation.SetActiveRotationCenter(currentWorldRotation.transform);
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            WorldRotation closestWorldRotation = FindClosestWorldRotation();
            if (closestWorldRotation != currentWorldRotation)
            {
                currentWorldRotation = closestWorldRotation;
                currentWorldRotation.SetActiveRotationCenter(currentWorldRotation.transform);
            }

            Transform currentRotationCenter = currentWorldRotation.rotationCenter;
            Vector3 playerPositionInWorld = player.position - currentRotationCenter.position;
            Vector3 updatedDesiredPosition = player.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, updatedDesiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Calculate the rotation to look at the player
            Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
        }
    }

    private WorldRotation FindClosestWorldRotation()
    {
        WorldRotation closestWorldRotation = worldRotations[0];
        float minDistance = Vector3.Distance(player.position, closestWorldRotation.transform.position);

        foreach (WorldRotation worldRotation in worldRotations)
        {
            float distance = Vector3.Distance(player.position, worldRotation.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWorldRotation = worldRotation;
            }
        }

        return closestWorldRotation;
    }
}
