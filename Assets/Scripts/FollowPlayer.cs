using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    private void Update()
    {
        if (player != null)
        {
            // Follow the player's position
            transform.position = player.position + offset;
        }
    }
}