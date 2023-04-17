using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;

public class CameraSwapZone : MonoBehaviour
{
    [Tooltip("The camera to activate when the player enters the zone.")]
    public Camera alternativeCamera;

    [Tooltip("The default player camera.")]
    public Camera defaultCamera;

    [Tooltip("Should the alternative camera follow the player's Y-axis rotation?")]
    public bool followPlayerRotation = false;

    [Tooltip("Reference to the player's Transform.")]
    public Transform playerTransform;

    [Header("Zoom SFX")]
    [Tooltip("SFX for zooming in.")]
    public AudioClip zoomInSFX;

    [Tooltip("SFX for zooming out.")]
    public AudioClip zoomOutSFX;

    [Tooltip("Cooldown between playing zoom SFX.")]
    public float sfxCooldown = 0.5f;

    [Tooltip("Volume level for zoom SFX.")]
    [Range(0f, 1f)]
    public float sfxVolume = 1f;

    public bool IsInZone { get; private set; } = false;

    private static List<CameraSwapZone> activeZones = new List<CameraSwapZone>();
    private bool isInZone = false;
    private float lastSFXTime;

    private void Start()
    {
        alternativeCamera.enabled = false;
        alternativeCamera.gameObject.SetActive(false);
    }

    private Camera GetActiveCamera()
    {
        Camera[] cameras = Camera.allCameras;
        foreach (var camera in cameras)
        {
            if (camera.enabled)
            {
                return camera;
            }
        }
        return null;
    }

    private void Update()
    {
        if (isInZone && followPlayerRotation)
        {
            Vector3 targetEulerAngles = new Vector3(alternativeCamera.transform.eulerAngles.x, playerTransform.eulerAngles.y, alternativeCamera.transform.eulerAngles.z);
            alternativeCamera.transform.eulerAngles = targetEulerAngles;
        }

        // Zoom SFX logic
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0 && Time.time >= lastSFXTime + sfxCooldown)
        {
            lastSFXTime = Time.time;
            AudioClip sfx = scroll > 0 ? zoomInSFX : zoomOutSFX;
            AudioSource.PlayClipAtPoint(sfx, defaultCamera.transform.position, sfxVolume);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isInZone)
            {
                isInZone = true;
                activeZones.Add(this);
            }

            UpdateActiveCamera();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwapToAlternativeCamera();
            IsInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SwapToDefaultCamera();
            IsInZone = false;
        }
    }

    private void UpdateActiveCamera()
    {
        // If no active zones, switch back to the default camera
        if (activeZones.Count == 0)
        {
            SwapToDefaultCamera();
        }
        else
        {
            // Activate the most recently entered zone's camera
            CameraSwapZone latestZone = activeZones[activeZones.Count - 1];
            if (latestZone != this)
            {
                latestZone.SwapToAlternativeCamera();
                SwapToDefaultCamera();
            }
            else
            {
                SwapToAlternativeCamera();
            }
        }
    }

    private void SwapToAlternativeCamera()
    {
        defaultCamera.enabled = false;
        defaultCamera.GetComponent<AudioListener>().enabled = false;
        alternativeCamera.gameObject.SetActive(true);
        alternativeCamera.enabled = true;
        alternativeCamera.GetComponent<AudioListener>().enabled = true;
    }

    private void SwapToDefaultCamera()
    {
        alternativeCamera.enabled = false;
        alternativeCamera.GetComponent<AudioListener>().enabled = false;
        alternativeCamera.gameObject.SetActive(false);
        defaultCamera.enabled = true;
        defaultCamera.GetComponent<AudioListener>().enabled = true;
    }
}
