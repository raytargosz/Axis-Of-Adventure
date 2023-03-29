using UnityEngine;
using DistantLands.Cozy;
using System.Collections;

public enum PadType
{
    Wind,
    Fire,
    Creatures,
    CustomSun,
    SunPressure
}

public class PressurePads : MonoBehaviour
{
    // Type of pressure pad
    [SerializeField]
    public PadType padType;

    // Audio clips to play when player steps on pressure pad
    [Tooltip("Audio clips to play when player steps on pressure pad")]
    public AudioClip[] stepOnSFX;

    // Array of game objects to enable/disable when pressure pad is activated
    [Tooltip("Array of game objects to enable/disable when pressure pad is activated")]
    public GameObject[] fxArray;

    // Array of creature prefabs to enable when pressure pad is activated
    [Tooltip("Array of creature prefabs to enable when pressure pad is activated")]
    public GameObject[] creaturePrefabs;

    // Weather controller to fast forward when pressure pad is activated
    [Tooltip("Weather controller to fast forward when pressure pad is activated")]
    public CozyWeather weatherController;

    // Minimum tick count to fast forward when pressure pad is activated
    [Tooltip("Minimum tick count to fast forward when pressure pad is activated")]
    public float minTickCount = 80;

    // Maximum tick count to fast forward when pressure pad is activated
    [Tooltip("Maximum tick count to fast forward when pressure pad is activated")]
    public float maxTickCount = 200;

    // Tick interval to fast forward when pressure pad is activated
    [Tooltip("Tick interval to fast forward when pressure pad is activated")]
    public float interval = 20;

    // Tick count speed to fast forward when pressure pad is activated
    [Tooltip("Tick count speed to fast forward when pressure pad is activated")]
    public float tickCountSpeed = 1f;

    // Audio clip to play when fxArray is activated
    [Tooltip("Audio clip to play when fxArray is activated")]
    public AudioClip fxSFX;

    // Array of audio clips to play when creaturePrefabs are activated
    [Tooltip("Array of audio clips to play when creaturePrefabs are activated")]
    public AudioClip[] sfxArray;

    // Audio clip to play when player steps off pressure pad
    [Tooltip("Audio clip to play when player steps off pressure pad")]
    public AudioClip stepOffSFX;

    // Audio source to play audio clips
    [Tooltip("Audio source to play audio clips")]
    public AudioSource audioSource;

    // Volume for audio clips
    [Range(0f, 1f)]
    [Tooltip("Volume for audio clips")]
    public float sfxVolume = 1f;

    // Time interval for activating fxArray
    [Tooltip("Time interval for activating fxArray")]
    public float activationInterval = 5f;

    // Duration of fade in for creature prefabs
    [Tooltip("Duration of fade in for creature prefabs")]
    public float fadeInDuration = 1f;

    // Tick count to fast forward for SunPressurePad
    [Tooltip("Tick count to fast forward for SunPressurePad")]
    public float ticksToFastForward = 100;

    // Whether the player is currently on the pressure pad
    private bool playerOnPad = false;

    private bool playerSteppedOn = false;
    private bool fxActivated = false;

    private void Start()
    {
        if (padType == PadType.Wind && fxArray.Length > 0)
        {
            fxArray[0].SetActive(false);
        }
        else if (padType == PadType.Fire)
        {
            foreach (GameObject fx in fxArray)
            {
                fx.SetActive(false);
            }
        }
        else if (padType == PadType.Creatures)
        {
            foreach (GameObject creature in creaturePrefabs)
            {
                creature.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (playerOnPad && weatherController != null && padType == PadType.CustomSun)
        {
            FastForwardTicks();
        }
        else if (playerOnPad && weatherController != null && padType == PadType.SunPressure)
        {
            FastForwardTicksSun();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerSteppedOn)
        {
            playerSteppedOn = true;
            if (padType == PadType.Wind)
            {
                PlayRandomSFX();
                EnableWindFX();
            }
            else if (padType == PadType.Fire)
            {
                StartCoroutine(ActivateFXAtRandomIntervals());
                PlaySFX(stepOnSFX[0]);
            }
            else if (padType == PadType.Creatures)
            {
                EnableCreaturesAndPlaySFX();
            }
            else if (padType == PadType.CustomSun)
            {
                playerOnPad = true;
                PlayRandomSFX();
            }
            else if (padType == PadType.SunPressure)
            {
                playerOnPad = true;
                PlayRandomSFX();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerSteppedOn = false;
            if (padType == PadType.CustomSun || padType == PadType.SunPressure)
            {
                playerOnPad = false;
                PlaySFX(stepOffSFX);
            }
        }
    }

    private IEnumerator ActivateFXAtRandomIntervals()
    {
        fxActivated = true;
        float startTime = Time.time;
        while (Time.time - startTime < activationInterval)
        {
            int randomIndex = Random.Range(0, fxArray.Length);
            fxArray[randomIndex].SetActive(true);
            PlaySFX(fxSFX);
            yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        }

        // Ensure all FX are enabled at the end of the activation interval
        foreach (GameObject fx in fxArray)
        {
            fx.SetActive(true);
        }
    }

    private void PlayRandomSFX()
    {
        if (audioSource != null && stepOnSFX.Length > 0)
        {
            int randomIndex = Random.Range(0, stepOnSFX.Length);
            AudioClip selectedSFX = stepOnSFX[randomIndex];

            if (selectedSFX != null)
            {
                audioSource.PlayOneShot(selectedSFX, sfxVolume);
            }
        }
    }

    private void EnableWindFX()
    {
        if (fxArray != null && fxArray.Length > 0)
        {
            fxArray[0].SetActive(true);
        }
    }

    private void EnableCreaturesAndPlaySFX()
    {
        // Enable and fade in all creature prefabs
        foreach (GameObject creature in creaturePrefabs)
        {
            creature.SetActive(true);
            StartCoroutine(FadeInCreature(creature));
        }

        // Play all sound effects
        StartCoroutine(PlaySFXArray());
    }

    private void FastForwardTicks()
    {
        if (weatherController.currentTicks < minTickCount) return;

        float targetTicks = Mathf.Min(weatherController.currentTicks + interval, maxTickCount);
        float tickIncrement = tickCountSpeed * Time.deltaTime;

        if (weatherController.currentTicks < targetTicks)
        {
            weatherController.currentTicks = Mathf.Min(weatherController.currentTicks + tickIncrement, targetTicks);
        }
    }

    private void FastForwardTicksSun()
    {
        float tickIncrement = tickCountSpeed * Time.deltaTime;
        weatherController.currentTicks = Mathf.Min(weatherController.currentTicks + tickIncrement, maxTickCount);
    }

    private void PlaySFX(AudioClip sfx)
    {
        if (audioSource != null && sfx != null)
        {
            audioSource.PlayOneShot(sfx, sfxVolume);
        }
    }


    private IEnumerator FadeInCreature(GameObject creature)
    {
        Renderer renderer = creature.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Renderer component not found on creature.");
            yield break;
        }

        Material material = renderer.material;
        Color initialColor = material.color;
        initialColor.a = 0f;
        material.color = initialColor;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            initialColor.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            material.color = initialColor;
            yield return null;
        }

        initialColor.a = 1f;
        material.color = initialColor;
    }

    private IEnumerator PlaySFXArray()
    {
        foreach (AudioClip sfx in sfxArray)
        {
            if (audioSource != null && sfx != null)
            {
                audioSource.PlayOneShot(sfx, sfxVolume);
                yield return new WaitForSeconds(sfx.length);
            }
        }
    }
}