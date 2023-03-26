using UnityEngine;
using DistantLands.Cozy;

public class TickController : MonoBehaviour
{
    [Tooltip("Number of collectibles to collect before increasing tick count")]
    [SerializeField] private int collectiblesRequired = 5;

    [Tooltip("Amount to increase tick count after required number of collectibles collected")]
    [SerializeField] private float tickIncrease = 10f;

    private int collectiblesCollected = 0;

    private void Start()
    {
        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        if (collectibleManager != null)
        {
            collectibleManager.OnCollectiblePicked += IncrementCollectiblesCollected;
        }
    }

    private void IncrementCollectiblesCollected()
    {
        collectiblesCollected++;
        if (collectiblesCollected % collectiblesRequired == 0)
        {
            IncreaseTickCount();
        }
    }

    private void IncreaseTickCount()
    {
        CozyWeather time = FindObjectOfType<CozyTime>();
        if (time != null)
        {
            time.currentTicks += tickIncrease;
        }
    }

    private void OnDestroy()
    {
        CollectibleManager collectibleManager = FindObjectOfType<CollectibleManager>();
        if (collectibleManager != null)
        {
            collectibleManager.OnCollectiblePicked -= IncrementCollectiblesCollected;
        }
    }
}
