using UnityEngine;

public class BoostOnTrigger : MonoBehaviour
{
    [Tooltip("Strength of the boost applied to the player")]
    public float boostForce = 10f;

    [Tooltip("Particle System GameObject for visualizing the boost direction")]
    public GameObject boostVFX;

    private ParticleSystem _particleSystem;

    void Start()
    {
        // Get the ParticleSystem component from the boostVFX GameObject
        _particleSystem = boostVFX.GetComponent<ParticleSystem>();

        // Update the direction of the Particle System to match the boost direction
        UpdateParticleSystemDirection();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider is tagged "Player"
        if (other.CompareTag("Player"))
        {
            // Get the Rigidbody component of the player
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();

            // Apply the boost force to the player's Rigidbody if it exists
            if (playerRigidbody != null)
            {
                // Use the object's local X-axis in world space as the force direction
                Vector3 forceDirection = transform.right;
                playerRigidbody.AddForce(forceDirection * boostForce, ForceMode.Impulse);
            }
        }
    }

    // This method is called whenever a value is changed in the Inspector
    void OnValidate()
    {
        // Update the Particle System direction if the boostVFX is assigned
        if (boostVFX != null)
        {
            UpdateParticleSystemDirection();
        }
    }

    // Update the Particle System's direction to match the boost direction
    private void UpdateParticleSystemDirection()
    {
        // Get the ParticleSystem component if it's not already assigned
        if (_particleSystem == null)
        {
            _particleSystem = boostVFX.GetComponent<ParticleSystem>();
        }

        // Update the Particle System direction if it exists
        if (_particleSystem != null)
        {
            var main = _particleSystem.main;
            main.startSpeed = boostForce;
            _particleSystem.transform.forward = transform.right;
        }
    }
}
