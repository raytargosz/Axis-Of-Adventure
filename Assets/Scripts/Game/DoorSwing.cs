using System.Collections;
using UnityEngine;

public class DoorSwing : MonoBehaviour
{
    public float rotationSpeed = 2f;
    public float openAngle = 90f;
    public AudioClip doorOpenSound;
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
