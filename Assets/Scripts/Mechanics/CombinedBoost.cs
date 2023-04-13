using System.Collections;
using UnityEngine;

public class CombinedBoost : MonoBehaviour
{
    [Tooltip("Strength of the boost applied to the player")]
    public float boostForce = 10f;

    [Tooltip("Particle System GameObject for visualizing the boost direction")]
    public GameObject boostVFX;

    [Tooltip("Cooldown time for the boost effect")]
    public float cooldownTime = 2f;

    [Tooltip("Rotation speed for spinning animation")]
    public float spinSpeed = 720f;

    [Tooltip("Sound effect for the boost")]
    public AudioClip boostSound;

    [Tooltip("Boost Direction reference object")]
    public Transform boostDirectionReference;

    private ParticleSystem _particleSystem;
    private AudioSource audioSource;
    private bool _isOnCooldown = false;
    private float _cooldownTimer = 0f;

    private void Start()
    {
        _particleSystem = boostVFX.GetComponent<ParticleSystem>();
        UpdateParticleSystemDirection();

        // Create and configure the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 3D sound
        audioSource.volume = 1.0f; // Increase the volume to the maximum
    }


    private void Update()
    {
        if (_isOnCooldown)
        {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer >= cooldownTime)
            {
                _isOnCooldown = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered");
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected");
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null && !_isOnCooldown)
            {
                Debug.Log("Applying force");
                // Use the object's local forward-axis in world space as the force direction
                Vector3 forceDirection = boostDirectionReference.forward;
                playerRigidbody.AddForce(forceDirection * boostForce, ForceMode.Impulse);

                _isOnCooldown = true;
                _cooldownTimer = 0f;

                if (boostSound != null)
                {
                    audioSource.PlayOneShot(boostSound);
                }

                StartCoroutine(SpinAnimation());
            }
            else
            {
                Debug.Log("Rigidbody is null or on cooldown");
            }
        }
    }


    void OnValidate()
    {
        if (boostVFX != null)
        {
            UpdateParticleSystemDirection();
        }
    }

    private void UpdateParticleSystemDirection()
    {
        if (_particleSystem == null)
        {
            _particleSystem = boostVFX.GetComponent<ParticleSystem>();
        }

        if (_particleSystem != null)
        {
            var main = _particleSystem.main;
            main.startSpeed = boostForce;
            _particleSystem.transform.forward = transform.right;
        }
    }

    private IEnumerator SpinAnimation()
    {
        float spinTimer = 0f;

        while (spinTimer < cooldownTime)
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
            spinTimer += Time.deltaTime;
            yield return null;
        }

        transform.rotation = Quaternion.identity;
    }
}