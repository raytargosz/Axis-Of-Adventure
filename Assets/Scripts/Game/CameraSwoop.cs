using UnityEngine;
using UnityEngine.UI;

public class CameraSwoop : MonoBehaviour
{
    [Tooltip("The main camera that will be moved during the swoop")]
    [SerializeField] private Camera mainCamera;

    [Tooltip("The player transform to look at during the swoop")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("The camera offset")]
    [SerializeField] private Vector3 cameraOffset;

    [Tooltip("The starting position of the camera")]
    [SerializeField] private Vector3 startPosition;

    [Tooltip("The ending position of the camera")]
    [SerializeField] private Vector3 endPosition;

    [Tooltip("The duration of the swoop animation in seconds")]
    [SerializeField] private float swoopDuration = 5f;

    [Tooltip("The duration of the fade animation in seconds")]
    [SerializeField] private float fadeDuration = 3f;

    [Tooltip("Audio clip to play during the swoop")]
    [SerializeField] private AudioClip swoopAudioClip;

    [Tooltip("Audio source to play the swoop audio clip")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Volume of the swoop audio clip")]
    [Range(0, 1)]
    [SerializeField] private float audioVolume = 1f;

    [Tooltip("Panel to fade out during the swoop")]
    [SerializeField] private Image panel;

    private float startTime;
    private bool isSwooping;

    public bool IsSwooping
    {
        get { return isSwooping; }
    }

    private void Start()
    {
        // Set the camera's starting position and start time
        mainCamera.transform.position = startPosition + cameraOffset;
        startTime = Time.time;
        isSwooping = true;

        // Play the swoop audio clip
        audioSource.clip = swoopAudioClip;
        audioSource.volume = audioVolume;
        audioSource.Play();
    }

    private void Update()
    {
        if (isSwooping)
        {
            // Calculate the progress of the swoop animation
            float swoopProgress = (Time.time - startTime) / swoopDuration;

            // Make the camera look at the player
            mainCamera.transform.LookAt(playerTransform);

            // Fade out the panel's alpha during the swoop
            if (panel != null)
            {
                float fadeProgress = (Time.time - startTime) / fadeDuration;
                float easedProgress = CubicEaseInOut(fadeProgress);
                Color panelColor = panel.color;
                panelColor.a = Mathf.Lerp(225f / 255f, 0, easedProgress);
                panel.color = panelColor;
            }

            // Move the camera towards the end position using linear interpolation
            mainCamera.transform.position = Vector3.Lerp(startPosition + cameraOffset, endPosition + cameraOffset, swoopProgress);

            // Make the camera look at the player
            mainCamera.transform.LookAt(playerTransform);

            // Fade out the panel's alpha during the swoop
            if (panel != null)
            {
                float fadeProgress = (Time.time - startTime) / fadeDuration;
                Color panelColor = panel.color;
                panelColor.a = Mathf.Lerp(225f / 255f, 0, fadeProgress);
                panel.color = panelColor;
            }

            if (swoopProgress >= 1f)
            {
                // Swoop animation is finished
                mainCamera.transform.position = endPosition + cameraOffset;
                isSwooping = false;
            }
        }

        // Cubic ease-in-out function
        float CubicEaseInOut(float t)
        {
            if (t < 0.5f)
            {
                return 4 * t * t * t;
            }
            else
            {
                float f = (2 * t) - 2;
                return 0.5f * f * f * f + 1;
            }
        }
    }
}
