using UnityEngine;
using Opsive.UltimateCharacterController.Camera;
using Opsive.UltimateCharacterController.Character;

public class UCCMouseWheelFOV : MonoBehaviour
{
    public float zoomSpeed = 2.0f;
    public float minFOV = 30.0f;
    public float maxFOV = 80.0f;
    public float sfxCooldown = 1.0f;

    public AudioClip zoomInSFX;
    public AudioClip zoomOutSFX;

    private UnityEngine.Camera cameraComponent;
    private AudioSource audioSource;
    private float sfxTimestamp;

    private void Start()
    {
        // Get the Camera component.
        cameraComponent = FindObjectOfType<UnityEngine.Camera>();

        if (cameraComponent == null)
        {
            Debug.LogWarning("Unable to find the Camera component.");
            enabled = false;
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        sfxTimestamp = Time.time;
    }

    void Update()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {
            float newFOV = cameraComponent.fieldOfView - scrollWheel * zoomSpeed;
            newFOV = Mathf.Clamp(newFOV, minFOV, maxFOV);
            cameraComponent.fieldOfView = newFOV;

            if (Time.time > sfxTimestamp + sfxCooldown)
            {
                if (scrollWheel > 0)
                {
                    audioSource.PlayOneShot(zoomInSFX);
                }
                else
                {
                    audioSource.PlayOneShot(zoomOutSFX);
                }
                sfxTimestamp = Time.time;
            }
        }
    }
}
