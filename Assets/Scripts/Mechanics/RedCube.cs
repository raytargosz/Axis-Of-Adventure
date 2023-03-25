using System.Collections;
using UnityEngine;

public class RedCube : MonoBehaviour
{
    [Tooltip("Boost speed on the X-axis")]
    public float boostSpeed = 20f;

    [Tooltip("Boost direction")]
    public Vector3 boostDirection = new Vector3(1, 0, 0); // Default is along the X-axis

    [Tooltip("Cooldown time for the boost effect")]
    public float cooldownTime = 2f;

    [Tooltip("Rotation speed for spinning animation")]
    public float spinSpeed = 720f;

    [Tooltip("Sound effect for the boost")]
    public AudioClip boostSound;

    private AudioSource audioSource;
    private bool _isOnCooldown = false;
    private float _cooldownTimer = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

    private void OnTriggerStay(Collider other)
    {
        CombinedPlayerController player = other.GetComponent<CombinedPlayerController>();
        if (player != null && !_isOnCooldown)
        {
            Vector3 currentMoveDirection = player.MoveDirection;
            Vector3 newMoveDirection = Vector3.Scale(currentMoveDirection, boostDirection).normalized * boostSpeed;
            player.SetMoveDirection(newMoveDirection);

            _isOnCooldown = true;
            _cooldownTimer = 0f;

            if (boostSound != null)
            {
                audioSource.PlayOneShot(boostSound);
            }

            StartCoroutine(SpinAnimation());
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

        // Reset rotation to the original state
        transform.rotation = Quaternion.identity;
    }
}