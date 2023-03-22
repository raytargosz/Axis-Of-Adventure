using UnityEngine;
using UnityEngine.UI;
using DistantLands.Cozy;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenuPanel; // Reference to the panel containing the options menu
    public IsometricCameraController cameraController;
    public CombinedPlayerController playerController;
    public Slider rotationSpeedSlider;
    public Slider smoothingSpeedSlider;
    public Slider fovSpeedSlider;
    public Slider moveSpeedSlider;
    public CozyWeather cozyWeather;
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider timeOfDaySpeedSlider;
    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;

    private bool isOptionsMenuActive;

    void Start()
    {
        rotationSpeedSlider.value = cameraController.rotationSpeed;
        smoothingSpeedSlider.value = cameraController.smoothingSpeed;
        fovSpeedSlider.value = cameraController.fovSpeed;
        moveSpeedSlider.value = playerController.moveSpeed;
        masterVolumeSlider.value = AudioListener.volume;
        timeOfDaySpeedSlider.value = cozyWeather.speed;

        isOptionsMenuActive = false;
        optionsMenuPanel.SetActive(isOptionsMenuActive);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleOptionsMenu();
        }
    }

    public void ToggleOptionsMenu()
    {
        isOptionsMenuActive = !isOptionsMenuActive;
        optionsMenuPanel.SetActive(isOptionsMenuActive);
        cameraController.enabled = !isOptionsMenuActive;
        playerController.enabled = !isOptionsMenuActive;

        // Show or hide the cursor based on the options menu state
        if (isOptionsMenuActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    public void OnSfxVolumeChanged(float value)
    {
        if (sfxAudioSource != null)
        {
            sfxAudioSource.volume = value;
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = value;
        }
    }

    public void OnTimeOfDaySpeedChanged(float value)
    {
        cozyWeather.speed = value;
    }

    public void OnRotationSpeedChanged(float value)
    {
        cameraController.rotationSpeed = value;
    }

    public void OnSmoothingSpeedChanged(float value)
    {
        cameraController.smoothingSpeed = value;
    }

    public void OnFovSpeedChanged(float value)
    {
        cameraController.fovSpeed = value;
    }

    public void OnMoveSpeedChanged(float value)
    {
        playerController.moveSpeed = value;
    }

    // Add methods for audio volume changes
}
