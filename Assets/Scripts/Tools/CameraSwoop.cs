using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class CameraSwoop : MonoBehaviour
{
    [Header("Camera Settings")]
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

    [Header("Events")]
    [Tooltip("Event that is triggered when the camera swoop is completed.")]
    public UnityEvent onSwoopComplete;

    [SerializeField] private IsometricCameraController isoCamController;

    private float startTime;
    private bool isSwooping;

    public bool IsSwooping => isSwooping;

    private Quaternion startRotation;
    private Quaternion endRotation;

    private void Start()
    {
        mainCamera.transform.position = startPosition + cameraOffset;
        startTime = Time.time;
        isSwooping = true;

        startRotation = mainCamera.transform.rotation;
        endRotation = Quaternion.LookRotation(playerTransform.position - endPosition);

        audioSource.clip = swoopAudioClip;
        audioSource.volume = audioVolume;
        audioSource.Play();

        isoCamController.enabled = false;
    }

    private void Update()
    {
        if (isSwooping)
        {
            float swoopProgress = (Time.time - startTime) / swoopDuration;

            if (panel != null)
            {
                float fadeProgress = (Time.time - startTime) / fadeDuration;
                float easedProgress = Mathf.SmoothStep(0f, 1f, fadeProgress);
                Color panelColor = panel.color;
                panelColor.a = Mathf.Lerp(225f / 255f, 0, easedProgress);
                panel.color = panelColor;
            }

            mainCamera.transform.position = Vector3.Lerp(startPosition + cameraOffset, endPosition + cameraOffset, Mathf.SmoothStep(0f, 1f, swoopProgress));
            mainCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, Mathf.SmoothStep(0f, 1f, swoopProgress));

            if (swoopProgress >= 1f)
            {
                mainCamera.transform.position = endPosition + cameraOffset;
                mainCamera.transform.rotation = endRotation;
                isSwooping = false;
                isoCamController.enabled = true;

                // Invoke the event after the swoop is complete
                onSwoopComplete.Invoke();
            }
        }
    }

    // Cubic ease-in-out function
    private float CubicEaseInOut(float t)
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