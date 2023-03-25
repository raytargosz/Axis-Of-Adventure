// This script controls door swinging behavior, opening the door when the player enters its trigger area.
// To use this script:
// 1. Attach this script to a door GameObject.
// 2. Make sure the door GameObject has a Collider component set as a trigger.
// 3. Make sure the player GameObject has the "Player" tag assigned.
// 4. Set the rotationSpeed, openAngle, doorOpenSound, and openSoundVolume fields in the Unity Inspector.

using System.Collections;
using UnityEngine;

public class DoorSwing : MonoBehaviour
{
    [Tooltip("Speed of the door rotation when opening.")]
    public float rotationSpeed = 2f;

    [Tooltip("Angle in degrees the door will rotate when opening.")]
    public float openAngle = 90f;

    [Tooltip("AudioClip to play when the door opens.")]
    public AudioClip doorOpenSound;

    [Tooltip("Volume of the door open sound. Range: 0 to 1.")]
    [Range(0, 1)] public float openSoundVolume = 1f;

    private AudioSource audioSource;
    private bool isOpen = false;
    private float initialRotationY;
    private float targetRotationY;

    private void Start()
    {
        initialRotationY = transform.localEulerAngles.y;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            OpenDoor(other.transform.position);
        }
    }

    private void OpenDoor(Vector3 playerPosition)
    {
        Vector3 doorToPlayer = playerPosition - transform.position;
        Vector3 crossProduct = Vector3.Cross(transform.forward, doorToPlayer);
        float direction = Mathf.Sign(crossProduct.y);
        targetRotationY = initialRotationY + openAngle * direction;
        isOpen = true;
        StartCoroutine(RotateDoor(targetRotationY, doorOpenSound, openSoundVolume));
    }

    private IEnumerator RotateDoor(float targetRotationY, AudioClip sound, float volume)
    {
        if (audioSource != null && sound != null)
        {
            Debug.Log("Playing sound: " + sound.name + ", volume: " + volume);
            audioSource.PlayOneShot(sound, volume);
        }
        else
        {
            Debug.Log("AudioSource or AudioClip is null");
        }

        float currentRotationY = transform.localEulerAngles.y;
        float startRotationY = currentRotationY;

        while (Mathf.Abs(Mathf.DeltaAngle(currentRotationY, targetRotationY)) > 0.1f)
        {
            currentRotationY = Mathf.MoveTowardsAngle(currentRotationY, targetRotationY, rotationSpeed * Time.deltaTime);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentRotationY, transform.localEulerAngles.z);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetRotationY, transform.localEulerAngles.z);
    }
}