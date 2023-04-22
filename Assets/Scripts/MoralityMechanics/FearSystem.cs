using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class FearSystem : MonoBehaviour
{
    [Header("Fear UI")]
    public Slider fearSlider;
    public Image[] imagesToFade;
    public TMP_Text[] textsToFade; 
    public float maxAlpha = 1f;
    public float minAlpha = 0f;

    public GameObject player;
    public GameObject[] darkZones;
    public AudioSource stressSFX;
    public float fearIncreaseRate = 1f;
    public float fearDecreaseRate = 0.5f;
    public float stressSFXIncreaseRate = 0.05f;
    public float stressSFXDecreaseRate = 0.02f;

    private bool isPlayerInDarkZone = false;

    private void Start()
    {
        foreach (var darkZone in darkZones)
        {
            var trigger = darkZone.AddComponent<DarkZoneTrigger>();
            trigger.OnPlayerEnterDarkZone += () => isPlayerInDarkZone = true;
            trigger.OnPlayerExitDarkZone += () => isPlayerInDarkZone = false;
        }
    }

    private void Update()
    {
        if (isPlayerInDarkZone)
        {
            IncreaseFear();
            IncreaseStressSFXVolume();
        }
        else
        {
            DecreaseFear();
            DecreaseStressSFXVolume();
        }
    }

    private void IncreaseFear()
    {
        fearSlider.value += Time.deltaTime * fearIncreaseRate;
        UpdateUIElementsAlpha();
    }

    private void DecreaseFear()
    {
        fearSlider.value -= Time.deltaTime * fearDecreaseRate;
        UpdateUIElementsAlpha();
    }

    private void UpdateUIElementsAlpha()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, fearSlider.value);

        // Update the alphas for the array of images
        foreach (Image image in imagesToFade)
        {
            Color imageColor = image.color;
            imageColor.a = alpha;
            image.color = imageColor;
        }

        // Update the alphas for the array of TMP_Text elements
        foreach (TMP_Text text in textsToFade)
        {
            Color textColor = text.color;
            textColor.a = alpha;
            text.color = textColor;
        }
    }

    private void IncreaseStressSFXVolume()
    {
        stressSFX.volume = Mathf.Clamp(stressSFX.volume + Time.deltaTime * stressSFXIncreaseRate, 0, 1);
    }

    private void DecreaseStressSFXVolume()
    {
        stressSFX.volume = Mathf.Clamp(stressSFX.volume - Time.deltaTime * stressSFXDecreaseRate, 0, 1);
    }
}

public class DarkZoneTrigger : MonoBehaviour
{
    public delegate void DarkZoneEvent();
    public event DarkZoneEvent OnPlayerEnterDarkZone;
    public event DarkZoneEvent OnPlayerExitDarkZone;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnterDarkZone?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerExitDarkZone?.Invoke();
        }
    }
}
