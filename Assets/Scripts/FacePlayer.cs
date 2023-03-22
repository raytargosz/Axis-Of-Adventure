using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public Transform player;

    private void Update()
    {
        if (player != null)
        {
            // Face the player
            transform.LookAt(player);
        }
    }
}