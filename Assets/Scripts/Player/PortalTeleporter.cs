using UnityEngine;

public class PortalTeleporter : MonoBehaviour
{
    [Tooltip("Assign the opposing portal GameObject.")]
    public GameObject targetPortal;
    [Tooltip("Player tag.")]
    public string playerTag = "Player";

    private bool isTeleporting;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !isTeleporting)
        {
            isTeleporting = true;
            TeleportPlayer(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isTeleporting = false;
        }
    }

    private void TeleportPlayer(GameObject player)
    {
        Vector3 portalToPlayer = player.transform.position - transform.position;
        float dotProduct = Vector3.Dot(transform.forward, portalToPlayer);

        // Check if the player is moving toward the portal
        if (dotProduct < 0f)
        {
            float rotationDiff = -Quaternion.Angle(transform.rotation, targetPortal.transform.rotation);
            rotationDiff += 180;
            player.transform.Rotate(Vector3.up, rotationDiff);

            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
            player.transform.position = targetPortal.transform.position + positionOffset;
        }
    }
}
