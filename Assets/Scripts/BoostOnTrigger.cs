using UnityEngine;

public class BoostOnTrigger : MonoBehaviour
{
    public float boostForce = 10f;
    public Transform boostDirection;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && boostDirection != null)
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 forceDirection = boostDirection.forward;
                playerRigidbody.AddForce(forceDirection * boostForce, ForceMode.Impulse);
            }
        }
    }
}