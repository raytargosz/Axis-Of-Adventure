using UnityEngine;

public class BobbingObject : MonoBehaviour
{
    [Tooltip("Amplitude of the bobbing motion.")]
    [SerializeField] private float bobbingAmplitude = 1f;

    [Tooltip("Frequency of the bobbing motion.")]
    [SerializeField] private float bobbingFrequency = 1f;

    [Tooltip("Frequency of the bobbing motion when sprinting.")]
    [SerializeField] private float sprintingBobbingFrequency = 1.5f;

    [Tooltip("Toggle between local and world space for the bobbing motion.")]
    [SerializeField] private bool useLocalSpace = true;

    [Header("Rotation Settings")]
    [Tooltip("Enable or disable Y rotation.")]
    [SerializeField] private bool enableYRotation = false;

    [Tooltip("Speed of the Y rotation.")]
    [SerializeField] private float yRotationSpeed = 1f;

    [Header("Player Movement")]
    [Tooltip("Reference to the PlayerMovement script.")]
    [SerializeField] private CombinedPlayerController playerController;

    [Header("SFX Settings")]
    [Tooltip("Sound effects to play when sprinting and bobbing.")]
    [SerializeField] private AudioClip[] sprintSFX;

    private Vector3 startPosition;
    private float timeOffset;

    [Header("SFX Settings")]
    [SerializeField] private AudioClip[] sfxArray;
    [SerializeField][Range(0, 1)] private float sfxVolume = 0.5f;
    [SerializeField][Range(0.1f, 2)] private float[] sfxPitchArray;

    private AudioSource audioSource;
    private int currentSfxIndex = -1;


    private void Start()
    {
        if (useLocalSpace)
        {
            startPosition = transform.localPosition;
        }
        else
        {
            startPosition = transform.position;
        }

        timeOffset = Random.Range(0f, 2 * Mathf.PI);

        playerController = GetComponentInParent<CombinedPlayerController>();

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = sfxVolume;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void ManageSfx(bool isSprinting)
    {
        if (isSprinting)
        {
            if (!audioSource.isPlaying)
            {
                currentSfxIndex = Random.Range(0, sfxArray.Length);
                audioSource.clip = sfxArray[currentSfxIndex];
                audioSource.pitch = sfxPitchArray[currentSfxIndex];
                audioSource.volume = sfxVolume;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }


    private void FixedUpdate()
    {
        if (playerController != null)
        {
            bool isSprinting = playerController.IsSprinting();
            float currentBobbingFrequency = isSprinting ? sprintingBobbingFrequency : bobbingFrequency;
            float speedRatio = isSprinting ? sprintingBobbingFrequency / bobbingFrequency : 1f;

            Vector3 newPosition = startPosition + Vector3.up * bobbingAmplitude * Mathf.Sin((Time.time + timeOffset) * currentBobbingFrequency * speedRatio);

            if (useLocalSpace)
            {
                transform.localPosition = newPosition;
            }
            else
            {
                transform.position = newPosition;
            }

            HandleSFX(isSprinting);
        }

        if (enableYRotation)
        {
            transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0, useLocalSpace ? Space.Self : Space.World);
        }
        ManageSfx(playerController.IsSprinting());
    }

    private void HandleSFX(bool isSprinting)
    {
        if (isSprinting && !audioSource.isPlaying && sprintSFX.Length > 0)
        {
            audioSource.clip = sprintSFX[Random.Range(0, sprintSFX.Length)];
            audioSource.Play();
        }
        else if (!isSprinting && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public float GetVerticalMovement()
    {
        return transform.position.y - startPosition.y;
    }
}