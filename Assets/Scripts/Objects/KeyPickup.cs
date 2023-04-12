using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    [Tooltip("Does the player have the key?")]
    public bool HasKey;

    // You can add OnTriggerEnter or other methods here to pick up the key in the game, e.g.:
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            HasKey = true;
            Destroy(other.gameObject);
        }
    }
}
