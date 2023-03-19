using UnityEngine;
using TMPro;

public class CollectibleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI collectibleCounterText;
    [SerializeField] private int requiredPickupAmount = 20;

    public static int CollectibleCount { get; private set; }
    public int RequiredPickupAmount => requiredPickupAmount;
    public TextMeshProUGUI CollectibleCounterText => collectibleCounterText; 

    private void Start()
    {
        CollectibleCount = 0;
        UpdateCollectibleCounterText();
    }

    public void IncrementCollectibleCount()
    {
        CollectibleCount++;
        UpdateCollectibleCounterText();
    }

    private void UpdateCollectibleCounterText()
    {
        if (collectibleCounterText != null)
        {
            collectibleCounterText.text = $"{CollectibleCount}/{requiredPickupAmount}";
        }
    }
}